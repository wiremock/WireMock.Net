// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using WireMock.Models;

namespace WireMock.Extensions;

/// <summary>
/// Some extensions for AnyOf.
/// </summary>
public static class AnyOfExtensions
{
    /// <summary>
    /// Gets the pattern as string value.
    /// </summary>
    /// <param name="value">AnyOf type</param>
    /// <returns>The string value</returns>
    public static string GetPattern(this AnyOf<string, StringPattern> value)
    {
        return value.IsFirst ? value.First : value.Second.Pattern;
    }

    /// <summary>
    /// Gets the patterns.
    /// </summary>
    /// <param name="values">AnyOf types</param>
    /// <returns>string values</returns>
    public static string[] GetPatterns(this AnyOf<string, StringPattern>[] values)
    {
        return values.Select(GetPattern).ToArray();
    }

    /// <summary>
    /// Converts a string-patterns to AnyOf patterns.
    /// </summary>
    /// <param name="patterns">The string patterns</param>
    /// <returns>The AnyOf patterns</returns>
    public static AnyOf<string, StringPattern>[] ToAnyOfPatterns(this IEnumerable<string> patterns)
    {
        return patterns.Select(p => p.ToAnyOfPattern()).ToArray();
    }

    /// <summary>
    /// Converts a string-pattern to AnyOf pattern.
    /// </summary>
    /// <param name="pattern">The string pattern</param>
    /// <returns>The AnyOf pattern</returns>
    public static AnyOf<string, StringPattern> ToAnyOfPattern(this string pattern)
    {
        return new AnyOf<string, StringPattern>(pattern);
    }
}