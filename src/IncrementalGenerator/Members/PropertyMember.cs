using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncrementalGenerator.Descriptors;

internal sealed class PropertyMember
{
    public PropertyMember(IPropertySymbol propertySymbol)
    {
        Name = propertySymbol.Name.ToString();
        Type = propertySymbol.Type.ToString();
    }

    public string Type { get; }
    public string Name { get; }
}
