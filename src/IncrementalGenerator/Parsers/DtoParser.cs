using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimpleDto.Generator.Extensions;
using SimpleDto.Generator.Templates.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SimpleDto.Generator.Parsers;

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

    public IEnumerable<DtoTypeDescriptor> GetDtoTypes(IEnumerable<TypeDeclarationSyntax> classes)
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
        foreach (var group in classes.GroupBy(x => x.SyntaxTree))
        {
            var syntaxTree = group.Key;
            var semanticModel = _compilation.GetSemanticModel(syntaxTree);

            foreach (var classDeclaration in group)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var classSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(classDeclaration, _cancellationToken)!;

                INamedTypeSymbol? entitySymbol = null;
                List<string> ignoredProperties = new();
                List<ITypeSymbol> ignoredTypes = new();

                foreach (var classAttributeList in classDeclaration.AttributeLists)
                {
                    foreach (var classAttribute in classAttributeList.Attributes)
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
                        PopulateIgnoredProperties(
                            ignoredProperties,
                            ignoredTypes,
                            boundAttributes,
                            dtoIgnoreAttribute);
                    }
                }

                if (entitySymbol is not null)
                {
                    yield return new DtoTypeDescriptor(
                        entitySymbol,
                        classDeclaration,
                        classSymbol,
                        ignoredProperties,
                        ignoredTypes);
                }
            }
        }
    }

    private static INamedTypeSymbol? GetEntity(ImmutableArray<AttributeData> boundAttributes, INamedTypeSymbol dtoFromAttribute)
    {
        foreach (var attributeData in boundAttributes)
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

    private static void PopulateIgnoredProperties(
        ICollection<string> ignoredProperties,
        ICollection<ITypeSymbol> ignoredTypes,
        ImmutableArray<AttributeData> boundAttributes,
        INamedTypeSymbol dtoIgnoreAttribute)
    {
        foreach (var attributeData in boundAttributes)
        {
            if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, dtoIgnoreAttribute))
            {
                continue;
            }

            // supports: [DtoMemberIgnore("Name")]
            // supports: [DtoMemberIgnore(typeof(string))]
            if (attributeData.ConstructorArguments.Any())
            {
                switch (attributeData.ConstructorArguments.Length)
                {
                    // DtoMemberIgnore(string propertyName)
                    // DtoMemberIgnore(Type propertyType)
                    case 1:
                        if (attributeData.ConstructorArguments[0].Value is string stringValue)
                        {
                            ignoredProperties.Add(stringValue);
                        }
                        else if (attributeData.ConstructorArguments[0].Value is ITypeSymbol type)
                        {
                            ignoredTypes.Add(type);
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

public class DtoTypeDescriptor
{
    public DtoTypeDescriptor(
        INamedTypeSymbol entity,
        TypeDeclarationSyntax dtoSyntax,
        INamedTypeSymbol dtoSymbol,
        List<string> ignoredProperties,
        List<ITypeSymbol> ignoredTypes)
    {
        EntitySymbol = entity;
        DtoSyntax = dtoSyntax;
        DtoSymbol = dtoSymbol;
        IgnoredProperties = ignoredProperties;
        IgnoredTypes = ignoredTypes;
    }

    public TypeDeclarationSyntax DtoSyntax { get; set; }
    public INamedTypeSymbol DtoSymbol { get; set; }
    public INamedTypeSymbol EntitySymbol { get; }
    public List<string> IgnoredProperties { get; }
    public List<ITypeSymbol> IgnoredTypes { get; }
}
