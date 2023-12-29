using IncrementalGenerator.Attributes;
using System.Collections.Generic;

namespace ConsoleApp;

[DtoFrom(typeof(Entity))]
[DtoMemberIgnore(typeof(IEnumerable<>))]
[DtoMemberIgnore(typeof(CustomEnum?))]
[DtoMemberIgnore(typeof(BaseEntity))]
[DtoMemberIgnore(nameof(Entity.Description))]
public sealed partial class EntityDto
{
}
