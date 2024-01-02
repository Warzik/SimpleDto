using Microsoft.CodeAnalysis;
using Scriban.Parsing;
using SimpleDto.Generator.Common;
using SimpleDto.Generator.Extensions;
using SimpleDto.Generator.Members;
using SimpleDto.Generator.Parsers;
using SimpleDto.Generator.Resolvers.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleDto.Generator.Resolvers;

internal sealed class SimplePropertiesResolver : BasePropertiesResolver
{
    private readonly Compilation _compilation;
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly CancellationToken _cancellationToken;

    public SimplePropertiesResolver(
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        _compilation = compilation;
        _reportDiagnostic = reportDiagnostic;
        _cancellationToken = cancellationToken;
    }

    public override IEnumerable<PropertyMember> ExtractProperties(DtoTypeDescriptor typeDescriptor)
    {
        foreach (var property in ExtractAll(typeDescriptor.EntitySymbol))
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

            if (property.Type.IsExportable())
            {
                yield return new PropertyMember(property);
            }
            else
            {
                _reportDiagnostic(Diagnostic.Create(
                 DiagnosticDescriptors.PropertyTypeInconsistentAccessibility,
                 property.Locations.FirstOrDefault(),
                 property.Type, property));
            }
        }
    }

    private static bool IsTypeIgnored(ITypeSymbol type, IEnumerable<ITypeSymbol> ignoredTypes)
    {
        foreach (var ignored in ignoredTypes)
        {
            if (IsTypeIgnored(type, ignored))
            {
                return true;
            }

            if (ignored is INamedTypeSymbol ignoredOpenType && ignoredOpenType.IsGenericType && ignoredOpenType.IsUnboundGenericType &&
              type is INamedTypeSymbol closedType && closedType.IsGenericType &&
              IsTypeIgnored(type, ignored.OriginalDefinition))
            {
                return true;
            }
        }

        return false;

        static bool IsTypeIgnored(ITypeSymbol type, ITypeSymbol ignoredType)
        {
            if (SymbolEqualityComparer.Default.Equals(type, ignoredType) ||
                SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, ignoredType))
            {
                return true;
            }

            foreach (var interfaceType in type.AllInterfaces)
            {
                if (IsTypeIgnored(interfaceType, ignoredType))
                {
                    return true;
                }
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
}
