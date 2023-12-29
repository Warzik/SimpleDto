using IncrementalGenerator.Common;
using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Parsers;
using IncrementalGenerator.Resolvers.Abstractions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IncrementalGenerator.Resolvers;

internal sealed class SimplePropertiesResolver : BasePropertiesResolver
{
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly CancellationToken _cancellationToken;

    public SimplePropertiesResolver(
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        _reportDiagnostic = reportDiagnostic;
        _cancellationToken = cancellationToken;
    }

    public override IEnumerable<PropertyMember> ExtractProperties(DtoTypeDescriptor typeDescriptor)
    {
        foreach (var property in base.ExtractAll(typeDescriptor.EntitySymbol))
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (IsPropertyIgnored(property, typeDescriptor.IgnoredProperties))
            {
                continue;
            }

            if (IsTypeIgnored(property.Type, typeDescriptor.IgnoredTypes))
            {
                continue;
            }

            if (IsTypeExportable(property.Type))
            {
                yield return new PropertyMember(property);
            }
            else
            {
                _reportDiagnostic((Diagnostic.Create(
                 DiagnosticDescriptors.PropertyTypeInconsistentAccessibility,
                 property.Locations.FirstOrDefault(),
                 property.Type, property)));
            }
        }
    }

    private static bool IsTypeIgnored(ITypeSymbol type, IEnumerable<ITypeSymbol> ignoredTypes)
    {
        foreach (var ignored in ignoredTypes)
        {
            if (IsOpenGenericCompatible(type, ignored))
            {
                return true;
            }

            if (IsTypeIgnored(type, ignored))
            {
                return true;
            }
        }

        return false;

        static bool IsOpenGenericCompatible(ITypeSymbol closedType, ITypeSymbol openType)
        {
            if (openType is INamedTypeSymbol namedOpenType &&
                namedOpenType.IsGenericType &&
                namedOpenType.IsUnboundGenericType &&
                closedType is INamedTypeSymbol namedClosedType &&
                namedClosedType.IsGenericType)
            {
                // Check if the closed type is the same as the open type definition.
                return IsTypeIgnored(closedType.OriginalDefinition, openType.OriginalDefinition);
            }

            return false;
        }

        static bool IsTypeIgnored(ITypeSymbol type, ITypeSymbol ignoredType)
        {
            if (SymbolEqualityComparer.Default.Equals(type, ignoredType))
            {
                return true;
            }

            if (type.AllInterfaces.Any(i => 
                SymbolEqualityComparer.Default.Equals(i, ignoredType) ||
                SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, ignoredType)))
            {
                return true;
            }

            if (type.BaseType is { SpecialType: not SpecialType.System_Object })
            {
                return IsTypeIgnored(type.BaseType, ignoredType);
            }

            return false;
        }
    }

    private static bool IsPropertyIgnored(IPropertySymbol property, IEnumerable<string> ignoredProperties)
    {
        foreach (var ignored in ignoredProperties)
        {
            if (property.Name == ignored)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsTypeExportable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return false;
        }

        if (namedTypeSymbol.IsGenericType)
        {
            // Check if all generic arguments are exportable
            foreach (var typeArgument in namedTypeSymbol.TypeArguments)
            {
                if (!IsTypeExportable(typeArgument))
                {
                    return false;
                }
            }
        }

        if (namedTypeSymbol.DeclaredAccessibility is Accessibility.Public)
        {
            return true;
        }

        return false;
    }
}
