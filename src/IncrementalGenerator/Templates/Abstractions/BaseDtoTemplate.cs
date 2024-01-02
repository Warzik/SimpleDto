using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace SimpleDto.Generator.Templates.Abstractions;

internal abstract class BaseDtoTemplate : BaseTemplate
{
    public BaseDtoTemplate()
    {
        TemplatesDir = $"{base.TemplatesDir}.Dtos";
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
