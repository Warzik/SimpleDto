using System.Collections.Generic;
using System.Reflection;

namespace SimpleDto.Generator.Templates.Abstractions;

internal abstract class BaseTemplate
{
    public abstract string Namespace { get; }

    public List<string> Usings = [];

    public abstract string Render();
}