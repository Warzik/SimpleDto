using IncrementalGenerator.Common;
using System;

namespace IncrementalGenerator.Templates;

internal sealed class DtoFromAttributeTemplate : BaseAttributeTemplate
{
    protected override string TemplateFileName => "DtoFromAttributeTemplate.cs.sbncs";
    protected override string TargetNamespace => Constants.AttributesTargetDir;

    public override string AttributeName => "DtoFromAttribute";
    public override AttributeTargets AttributeTarget => AttributeTargets.Class;
    public override bool AllowMultiple => false;
}