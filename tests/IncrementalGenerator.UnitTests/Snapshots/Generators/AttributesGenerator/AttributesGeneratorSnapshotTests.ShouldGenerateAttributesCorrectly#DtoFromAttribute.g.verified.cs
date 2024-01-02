﻿//HintName: DtoFromAttribute.g.cs
// <auto-generated/>
#nullable enable

using System;

namespace SimpleDto.Generator.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class DtoFromAttribute : Attribute
    {
        public DtoFromAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; }
    }
}
