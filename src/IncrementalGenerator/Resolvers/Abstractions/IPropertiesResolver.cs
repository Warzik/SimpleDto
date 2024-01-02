using SimpleDto.Generator.Members;
using SimpleDto.Generator.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDto.Generator.Resolvers.Abstractions;

internal interface IPropertiesResolver
{
    IEnumerable<PropertyMember> ExtractProperties(DtoTypeDescriptor typeDescriptor);
}
