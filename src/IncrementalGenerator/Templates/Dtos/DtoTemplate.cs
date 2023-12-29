using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Parsers;
using IncrementalGenerator.Strategies.Abstractions;
using IncrementalGenerator.Templates.Abstractions;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace IncrementalGenerator.Templates.Classes;

internal sealed class DtoTemplate : BaseDtoTemplate
{
    public DtoTemplate(DtoTypeDescriptor dtoClass, IPropertiesResolver propertiesResolver)
    {
        TemplateFileName = "DtoTemplate.cs.sbncs";
        Namespace = GetNamespaceFrom(dtoClass.DtoSyntax);

        Name = dtoClass.DtoSyntax.Identifier.ToString();

        Properties = propertiesResolver.ExtractProperties(dtoClass)
            .ToArray();

        TypeModifiers = dtoClass.DtoSyntax.Modifiers
            .Select(x => x.ToString())
            .ToArray();

        IsRecord = dtoClass.DtoSymbol.IsRecord;
    }

    public string Name { get; }
    public PropertyMember[] Properties { get; }
    public string[] TypeModifiers { get; }
    public bool IsRecord { get; }

    public override string Namespace { get; }
    protected override string TemplateFileName { get; }
}
