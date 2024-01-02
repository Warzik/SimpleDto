using SimpleDto.Generator.Common;
using SimpleDto.Generator.Templates.Abstractions;
using System;

namespace SimpleDto.Generator.Templates.Attributes;

internal sealed class DtoFromAttributeTemplate : BaseAttributeTemplate
{
    public override string Namespace { get; } = Constants.AttributesNamespace;
    public override string AttributeFullName => $"{Namespace}.{AttributeName}";
    public override string AttributeName { get; } = "DtoFromAttribute";
    public override AttributeTargets AttributeTarget { get; } = AttributeTargets.Class;
    public override bool AllowMultiple { get; } = false;

    protected override string TemplateFileName { get; } = $"{nameof(DtoFromAttributeTemplate)}.cs.sbncs";
}