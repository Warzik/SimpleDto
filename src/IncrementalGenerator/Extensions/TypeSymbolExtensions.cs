using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace SimpleDto.Generator.Extensions;

internal static class TypeSymbolExtensions
{
    public static bool IsExportable(this ITypeSymbol typeSymbol)
    {
        if (!typeSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public) &&
            typeSymbol.TypeKind is not TypeKind.Array)
        {
            return false;
        }

        if (typeSymbol.ContainingType != null && !typeSymbol.ContainingType.IsExportable())
        {
            return false;
        }

        var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

        if (namedTypeSymbol is { IsGenericType: true })
        {
            foreach (var typeArgument in namedTypeSymbol.TypeArguments)
            {
                if (!typeArgument.IsExportable())
                {
                    return false;
                }
            }
        }

        return true;
    }
}
