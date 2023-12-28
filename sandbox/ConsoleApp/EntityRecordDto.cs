using IncrementalGenerator.Attributes;

namespace ConsoleApp;

[DtoFrom(typeof(Entity))]
public sealed partial record EntityRecordDto
{
}

