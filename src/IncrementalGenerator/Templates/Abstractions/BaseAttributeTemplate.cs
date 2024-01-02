using System;

namespace SimpleDto.Generator.Templates.Abstractions;

internal abstract class BaseAttributeTemplate : BaseTemplate
{
    public BaseAttributeTemplate()
    {
        Usings.Add(nameof(System));

        TemplatesDir = $"{base.TemplatesDir}.Attributes";
    }

    public abstract string AttributeFullName { get; }
    public abstract string AttributeName { get; }
    public abstract AttributeTargets AttributeTarget { get; }
    public abstract bool AllowMultiple { get; }

    protected override string TemplatesDir { get; }
}