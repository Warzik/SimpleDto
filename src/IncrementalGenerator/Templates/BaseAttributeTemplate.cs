using System;

namespace IncrementalGenerator.Templates;

internal abstract class BaseAttributeTemplate : BaseTemplate
{
    public BaseAttributeTemplate()
    {
        Usings.Add(nameof(System));
    }

    public abstract string AttributeName { get; }
    public abstract AttributeTargets AttributeTarget { get; }
    public abstract bool AllowMultiple { get; }
}
