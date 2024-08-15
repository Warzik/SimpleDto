using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDto.Generator.Extensions;
internal static class EnumerableExtensions
{
    internal static string Join(this IEnumerable<string> strings)
    {
        return string.Join(string.Empty, strings);
    }

    internal static string Join(this IEnumerable<string> strings, string separator)
    {
        return string.Join(separator, strings);
    }

    internal static string JoinWithSpace(this IEnumerable<string> strings)
    {
        return string.Join(" ", strings);
    }

    internal static string JoinWithNewLine(this IEnumerable<string> strings)
    {
        return string.Join("\n", strings);
    }
}
