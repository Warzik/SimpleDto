using IncrementalGenerator.Common;
using IncrementalGenerator.Templates.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalGenerator.Templates.Attributes;

internal sealed class DtoMemberIgnoreAttributeTemplate : BaseAttributeTemplate
{
    public override string Namespace { get; } = Constants.AttributesNamespace;
    public override string AttributeFullName => $"{Namespace}.{AttributeName}";
    public override string AttributeName { get; } = "DtoMemberIgnoreAttribute";
    public override AttributeTargets AttributeTarget { get; } = AttributeTargets.Class;
    public override bool AllowMultiple { get; } = true;

    protected override string TemplateFileName { get; } = $"{nameof(DtoMemberIgnoreAttributeTemplate)}.cs.sbncs";
}