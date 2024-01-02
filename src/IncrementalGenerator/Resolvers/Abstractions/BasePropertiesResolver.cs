using Microsoft.CodeAnalysis;
using SimpleDto.Generator.Members;
using SimpleDto.Generator.Parsers;
using System.Collections.Generic;

namespace SimpleDto.Generator.Resolvers.Abstractions;
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
