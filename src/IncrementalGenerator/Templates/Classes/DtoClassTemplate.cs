using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Templates.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IncrementalGenerator.Templates.Classes;

internal sealed class DtoClassTemplate : BaseClassTemplate
{
    public DtoClassTemplate(ClassDeclarationSyntax dto, INamedTypeSymbol entity)
    {
        TemplateFileName = "DtoClassTemplate.cs.sbncs";

        ClassName = dto.Identifier.ToString();

        ClassProperties = entity.GetMembers()
            .OfType<IPropertySymbol>()
            .Select(symbol => new PropertyDescriptor(symbol))
            .ToArray();

        ClassModifiers = dto.Modifiers
            .Select(x => x.ToString())
            .ToArray();

        Namespace = GetNamespaceFrom(dto);
    }

    public string ClassName { get; }
    public PropertyDescriptor[] ClassProperties { get; }
    public string[] ClassModifiers { get; }
    public override string Namespace { get; } 

    protected override string TemplateFileName { get; }
}
