﻿using SimpleDto.Generator.Common;
using SimpleDto.Generator.Extensions;
using SimpleDto.Generator.Templates.Abstractions;
using System;
using System.Linq;

namespace SimpleDto.Generator.Templates.Attributes;

internal sealed class DtoFromAttributeTemplate : BaseAttributeTemplate
{
    public override string Namespace { get; } = Constants.AttributesNamespace;
    public override string AttributeFullName => $"{Namespace}.{AttributeName}";
    public override string AttributeName { get; } = "DtoFromAttribute";
    public override AttributeTargets AttributeTarget { get; } = AttributeTargets.Class;
    public override bool AllowMultiple { get; } = false;

    public override string Render()
    {
        return $$"""
            // <auto-generated/>
            #nullable enable
            {{Usings.Select(x => $"using {x};{"\n"}").Join()}}
            namespace {{Namespace}}
            {
            {{Constants.Tabulator}}[AttributeUsage(AttributeTargets.{{AttributeTarget}}, AllowMultiple = {{AllowMultiple.ToString().ToLower()}})]
            {{Constants.Tabulator}}internal sealed class DtoFromAttribute : Attribute
            {{Constants.Tabulator}}{
            {{Constants.Tabulator}}{{Constants.Tabulator}}public DtoFromAttribute(Type entityType)
            {{Constants.Tabulator}}{{Constants.Tabulator}}{
            {{Constants.Tabulator}}{{Constants.Tabulator}}{{Constants.Tabulator}}EntityType = entityType;
            {{Constants.Tabulator}}{{Constants.Tabulator}}}

            {{Constants.Tabulator}}{{Constants.Tabulator}}public Type EntityType { get; }
            {{Constants.Tabulator}}}
            }

            """;
    }
}