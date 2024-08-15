using System.Collections.Generic;
using SimpleDto.Generator.Attributes;

namespace ConsoleApp;

[DtoFrom(typeof(Entity))]
[DtoMemberIgnore(typeof(IEnumerable<>))]
[DtoMemberIgnore(nameof(Entity.Description))]
[DtoMemberIgnore(typeof(BaseEntity))]
public sealed partial class EntityDto
{
}
