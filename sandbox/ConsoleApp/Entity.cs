﻿using System.Collections.Generic;

namespace ConsoleApp;

internal sealed class Entity : BaseEntity
{
    public List<string>? Strings { get; set; }
    public NestedEntity? Nested { get; set; }
    public string? Description { get; set; }
    public CustomEnum CustomEnum { get; set; }
    public CustomEnum? NullableCustomEnum { get; set; }
}
