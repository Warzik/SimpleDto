using System;

namespace IncrementalGenerator.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DtoFromAttribute(Type type) : Attribute
{
    public Type EntityType { get; } = type;
}