using IncrementalGenerator.Common;
using IncrementalGenerator.Templates.Abstractions;
using System;

namespace IncrementalGenerator.Templates.Attributes;

internal sealed class DtoFromAttributeTemplate : BaseAttributeTemplate
{
    public override string Namespace { get; } = Constants.AttributesNamespace;
    public override string AttributeFullName => $"{Namespace}.{AttributeName}";
    public override string AttributeName { get; } = "DtoFromAttribute";
    public override AttributeTargets AttributeTarget { get; } = AttributeTargets.Class;
    public override bool AllowMultiple { get; } = false;

    protected override string TemplateFileName { get; } = "DtoFromAttributeTemplate.cs.sbncs";
}