using IncrementalGenerator.Common;
using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Parsers;
using IncrementalGenerator.Strategies.Abstractions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalGenerator.Resolvers.Abstractions;
internal abstract class BasePropertiesResolver : IPropertiesResolver
{
    public abstract IEnumerable<PropertyMember> ExtractProperties(DtoTypeDescriptor typeDescriptor);

    protected IEnumerable<IPropertySymbol> ExtractAll(INamedTypeSymbol classSymbol)
    {
        if (classSymbol.BaseType is not null)
        {
            foreach (var property in ExtractAll(classSymbol.BaseType))
            {
                yield return property;
            }
        }

        foreach (var property in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            yield return property;
        }
    }
}
