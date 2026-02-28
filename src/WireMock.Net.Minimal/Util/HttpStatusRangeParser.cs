// Copyright © WireMock.Net

using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace WireMock.Util;

/// <summary>
/// Based on https://github.com/tmenier/Flurl/blob/129565361e135e639f1d44a35a78aea4302ac6ca/src/Flurl.Http/HttpStatusRangeParser.cs
/// </summary>
internal static class HttpStatusRangeParser
{
    /// <summary>
    /// Determines whether the specified pattern is match.
    /// </summary>
    /// <param name="pattern">The pattern. (Can be null, in that case it's allowed.)</param>
    /// <param name="httpStatusCode">The value.</param>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is invalid.</exception>
    public static bool IsMatch(string? pattern, object? httpStatusCode)
    {
        return httpStatusCode switch
        {
            int statusCodeAsInteger => IsMatch(pattern, statusCodeAsInteger),
            string statusCodeAsString => IsMatch(pattern, int.Parse(statusCodeAsString)),
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the specified pattern is match.
    /// </summary>
    /// <param name="pattern">The pattern. (Can be null, in that case it's allowed.)</param>
    /// <param name="httpStatusCode">The value.</param>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is invalid.</exception>
    public static bool IsMatch(string pattern, HttpStatusCode httpStatusCode)
    {
        return IsMatch(pattern, (int)httpStatusCode);
    }

    /// <summary>
    /// Determines whether the specified pattern is match.
    /// </summary>
    /// <param name="pattern">The pattern. (Can be null, in that case it's allowed.)</param>
    /// <param name="httpStatusCode">The value.</param>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is invalid.</exception>
    public static bool IsMatch(string? pattern, int httpStatusCode)
    {
        if (pattern == null)
        {
            return true;
        }

        foreach (var range in pattern.Split(',').Select(p => p.Trim()))
        {
            switch (range)
            {
                case "":
                    continue;

                case "*":
                    return true; // special case - allow everything
            }

            string[] bounds = range.Split('-');
            int lower = 0;
            int upper = 0;

            bool valid =
                bounds.Length <= 2 &&
                int.TryParse(Regex.Replace(bounds.First().Trim(), "[*xX]", "0"), out lower) &&
                int.TryParse(Regex.Replace(bounds.Last().Trim(), "[*xX]", "9"), out upper);

            if (!valid)
            {
                throw new ArgumentException($"Invalid range pattern: \"{pattern}\". Examples of allowed patterns: \"400\", \"4xx\", \"300,400-403\", \"*\".");
            }

            if (httpStatusCode >= lower && httpStatusCode <= upper)
            {
                return true;
            }
        }

        return false;
    }
}