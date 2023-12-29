using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalGenerator.Strategies.Abstractions;

internal interface IPropertiesResolver
{
    IEnumerable<PropertyMember> ExtractProperties(DtoTypeDescriptor typeDescriptor);
}
