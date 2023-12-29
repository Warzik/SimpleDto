using IncrementalGenerator.Attributes;
using System.Collections.Generic;

namespace ConsoleApp;

[DtoFrom(typeof(Entity))]
[DtoMemberIgnore(typeof(IEnumerable<>))]
[DtoMemberIgnore(typeof(CustomEnum?))]
public sealed partial class EntityDto
{
}
