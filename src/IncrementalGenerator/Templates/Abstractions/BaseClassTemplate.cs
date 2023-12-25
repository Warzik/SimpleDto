using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace IncrementalGenerator.Templates.Abstractions;

internal abstract class BaseClassTemplate : BaseTemplate
{
    public BaseClassTemplate()
    {
        Usings.Add(nameof(System));

        TemplatesDir = $"{base.TemplatesDir}.Classes";
    }

    protected override string TemplatesDir { get; }

    protected static string GetNamespaceFrom(SyntaxNode node) => node.Parent switch
    {
        NamespaceDeclarationSyntax namespaceDeclarationSyntax => namespaceDeclarationSyntax.Name.ToString(),
        FileScopedNamespaceDeclarationSyntax namespaceDeclarationSyntax => namespaceDeclarationSyntax.Name.ToString(),
        null => throw new ArgumentException("Namespace declaration syntax not found.", nameof(node)),
        _ => GetNamespaceFrom(node.Parent)
    };
}
