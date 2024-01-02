using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDto.Generator.Common;

public static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor PropertyTypeInconsistentAccessibility { get; } = new DiagnosticDescriptor(
        "SD1001",
        "Property type inconsistent accessibility.",
        "Property type '{0}' is less accessible than property '{1}'.",
        DiagnosticCategories.SimpleDto,
        DiagnosticSeverity.Warning,
        true
    );
}