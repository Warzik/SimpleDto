using Microsoft.CodeAnalysis;
using SimpleDto.Generator.Members;
using SimpleDto.Generator.Parsers;
using SimpleDto.Generator.Resolvers.Abstractions;
using SimpleDto.Generator.Templates.Abstractions;
using System;
using System.Linq;

namespace SimpleDto.Generator.Templates.Dtos;

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
