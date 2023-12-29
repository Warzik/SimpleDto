using IncrementalGenerator.Attributes;

namespace ConsoleApp;

[DtoFrom(typeof(Entity))]
[DtoMemberIgnore(typeof(BaseEntity))]
public sealed partial record EntityRecordDto
{
}

