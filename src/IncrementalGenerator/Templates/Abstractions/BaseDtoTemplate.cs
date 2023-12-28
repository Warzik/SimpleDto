using IncrementalGenerator.Common;
using IncrementalGenerator.Descriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace IncrementalGenerator.Templates.Abstractions;

internal abstract class BaseDtoTemplate : BaseTemplate
{
    public BaseDtoTemplate(Action<Diagnostic> reportDiagnostic)
    {
        TemplatesDir = $"{base.TemplatesDir}.Dtos";
        ReportDiagnostic = reportDiagnostic;
    }

    public Action<Diagnostic> ReportDiagnostic { get; }

    public abstract string Name { get; }
    public abstract PropertyMember[] Properties { get; }
    public abstract string[] TypeModifiers { get; }
    public abstract bool IsRecord { get; }
    protected override string TemplatesDir { get; }

    protected static string GetNamespaceFrom(SyntaxNode node) => node.Parent switch
    {
        NamespaceDeclarationSyntax namespaceDeclarationSyntax => namespaceDeclarationSyntax.Name.ToString(),
        FileScopedNamespaceDeclarationSyntax namespaceDeclarationSyntax => namespaceDeclarationSyntax.Name.ToString(),
        null => throw new ArgumentException("Namespace declaration syntax not found.", nameof(node)),
        _ => GetNamespaceFrom(node.Parent)
    };

    protected IEnumerable<PropertyMember> GetProperties(INamedTypeSymbol classSymbol)
    {
        if (classSymbol.BaseType is not null)
        {
            foreach (var property in GetProperties(classSymbol.BaseType))
            {
                yield return property;
            }
        }

        // TODO strategies
        foreach (var property in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            // Ignore user defined type except enum
            if (property.Type is { SpecialType: not SpecialType.None } or 
                { TypeKind: TypeKind.Enum or TypeKind.Struct or TypeKind.Structure })
            {
                // TODO handle nullable structs 
                if (property.Type.DeclaredAccessibility is Accessibility.Public)
                {
                    yield return new PropertyMember(property);
                }
                else
                {
                    ReportDiagnostic((Diagnostic.Create(
                     DiagnosticDescriptors.PropertyInconsistentAccessibility,
                     property.Locations.FirstOrDefault(),
                     property.Type, property)));
                }
            }
        }
    }
}
