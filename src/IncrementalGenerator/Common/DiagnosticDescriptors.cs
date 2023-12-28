using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalGenerator.Common;

public static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor PropertyInconsistentAccessibility { get; } = new DiagnosticDescriptor(
        "SD1001",
        "Inconsistent accessibility.",
        "Property type '{0}' is less accessible than property '{1}'.",
        DiagnosticCategories.SimpleDto,
        DiagnosticSeverity.Warning,
        true
    );
}