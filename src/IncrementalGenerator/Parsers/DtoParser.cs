using IncrementalGenerator.Common;
using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Extensions;
using IncrementalGenerator.Templates.Attributes;
using IncrementalGenerator.Templates.Classes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace IncrementalGenerator.Parsers;

internal class DtoParser
{
    private static string DtoFromAttribute = new DtoFromAttributeTemplate().AttributeFullName;
    private static string DtoIgnoreAttribute = new DtoMemberIgnoreAttributeTemplate().AttributeFullName;

    private readonly CancellationToken _cancellationToken;
    private readonly Compilation _compilation;
    private readonly Action<Diagnostic> _reportDiagnostic;

    public DtoParser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
    {
        _compilation = compilation;
        _cancellationToken = cancellationToken;
        _reportDiagnostic = reportDiagnostic;
    }

    public IEnumerable<DtoClass> GetDtoTypes(IEnumerable<TypeDeclarationSyntax> classes)
    {
        var dtoFromAttribute = _compilation.GetBestTypeByMetadataName(DtoFromAttribute);
        if (dtoFromAttribute == null)
        {
            // nothing to do if this type isn't available
            yield break;
        }

        var dtoIgnoreAttribute = _compilation.GetBestTypeByMetadataName(DtoIgnoreAttribute);
        if (dtoIgnoreAttribute == null)
        {
            // nothing to do if this type isn't available
            yield break;
        }

        // we enumerate by syntax tree, to minimize the need to instantiate semantic models (since they're expensive)
        foreach (IGrouping<SyntaxTree, TypeDeclarationSyntax> group in classes.GroupBy(x => x.SyntaxTree))
        {
            var syntaxTree = group.Key;
            var semanticModel = _compilation.GetSemanticModel(syntaxTree);

            foreach (TypeDeclarationSyntax classDeclaration in group)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                INamedTypeSymbol classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(classDeclaration, _cancellationToken)!;
               
                INamedTypeSymbol? entitySymbol = null;
                List<string> ignoredProperties = new();

                foreach (AttributeListSyntax classAttributeList in classDeclaration.AttributeLists)
                {
                    foreach (AttributeSyntax classAttribute in classAttributeList.Attributes)
                    {
                        var attributeCtorSymbol = semanticModel.GetSymbolInfo(classAttribute, _cancellationToken).Symbol as IMethodSymbol;
                        if (attributeCtorSymbol == null || !dtoFromAttribute.Equals(attributeCtorSymbol.ContainingType, SymbolEqualityComparer.Default))
                        {
                            // badly formed attribute definition, or not the right attribute
                            continue;
                        }

                        var boundAttributes = classSymbol.GetAttributes();

                        if (boundAttributes.Length == 0)
                        {
                            continue;
                        }

                        entitySymbol = GetEntity(boundAttributes, dtoFromAttribute);
                        ignoredProperties.AddRange(GetIgnoredProperties(boundAttributes, dtoIgnoreAttribute));
                    }
                }

                if(entitySymbol is not null)
                {
                    yield return new DtoClass(entitySymbol, classDeclaration, classSymbol, ignoredProperties);
                }
            }
        }
    }

    private static INamedTypeSymbol? GetEntity(ImmutableArray<AttributeData> boundAttributes, INamedTypeSymbol dtoFromAttribute)
    {
        foreach (AttributeData attributeData in boundAttributes)
        {
            if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, dtoFromAttribute))
            {
                continue;
            }

            // supports: [DtoFrom(typeof(Entity))]
            if (attributeData.ConstructorArguments.Any())
            {
                switch (attributeData.ConstructorArguments.Length)
                {
                    // DtoFrom(Type type)
                    case 1:
                        return (INamedTypeSymbol)attributeData.ConstructorArguments[0].Value!;

                    default:
                        Debug.Assert(false, "Unexpected number of arguments in attribute constructor.");
                        break;
                }
            }
        }

        return null;
    }

    private static IEnumerable<string> GetIgnoredProperties(ImmutableArray<AttributeData> boundAttributes, INamedTypeSymbol dtoIgnoreAttribute)
    {
        foreach (AttributeData attributeData in boundAttributes)
        {
            if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, dtoIgnoreAttribute))
            {
                continue;
            }

            // supports: [DtoMemberIgnore("Name")]
            if (attributeData.ConstructorArguments.Any())
            {
                switch (attributeData.ConstructorArguments.Length)
                {
                    // DtoMemberIgnore(string propertyName)
                    case 1:
                        if(attributeData.ConstructorArguments[0].Value is string stringValue)
                        {
                            yield return stringValue;
                        } 
                        break;

                    default:
                        Debug.Assert(false, "Unexpected number of arguments in attribute constructor.");
                        break;
                }
            }
        }
    }
}

public class DtoClass
{
    public DtoClass(
        INamedTypeSymbol entity,
        TypeDeclarationSyntax dtoSyntax,
        INamedTypeSymbol dtoSymbol,
        List<string> ignoredProperties)
    {
        EntitySymbol = entity;
        DtoSyntax = dtoSyntax;
        DtoSymbol = dtoSymbol;
        IgnoredProperties = ignoredProperties;
    }

    public TypeDeclarationSyntax DtoSyntax { get; set; }
    public INamedTypeSymbol DtoSymbol { get; set; }
    public INamedTypeSymbol EntitySymbol { get; }
    public List<string> IgnoredProperties { get; }
}
