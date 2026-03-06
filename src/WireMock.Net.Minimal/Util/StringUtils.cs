// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using WireMock.Matchers;

namespace WireMock.Util;

internal static class StringUtils
{
    private static readonly string[] ValidUriSchemes =
    [
        "ftp://",
        "http://",
        "https://"
    ];

    private static readonly Func<string, (bool IsConverted, object ConvertedValue)>[] ConversionsFunctions =
    [
        s => bool.TryParse(s, out var result) ? (true, result) : (false, s),
        s => int.TryParse(s, out var result) ? (true, result) : (false, s),
        s => long.TryParse(s, out var result) ? (true, result) : (false, s),
        s => double.TryParse(s, out var result) ? (true, result) : (false, s),
        s => Guid.TryParseExact(s, "D", out var result) ? (true, result) : (false, s),
        s => TimeSpan.TryParse(s, out var result) ? (true, result) : (false, s),
        s => DateTime.TryParse(s, out var result) ? (true, result) : (false, s),
        s =>
        {
            if (ValidUriSchemes.Any(u => s.StartsWith(u, StringComparison.OrdinalIgnoreCase)) &&
                Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var uri))
            {
                return (true, uri);
            }

            return (false, s);
        }
    ];

    public static (bool IsConverted, object ConvertedValue) TryConvertToKnownType(string value)
    {
        foreach (var func in ConversionsFunctions)
        {
            var result = func(value);
            if (result.IsConverted)
            {
                return result;
            }
        }

        return (false, value);
    }

    public static MatchOperator ParseMatchOperator(string? value)
    {
        return value != null && Enum.TryParse<MatchOperator>(value, out var matchOperator)
            ? matchOperator
            : MatchOperator.Or;
    }

    public static bool TryParseQuotedString(string? value, [NotNullWhen(true)] out string? result, out char quote)
    {
        result = null;
        quote = '\0';

        if (value == null || value.Length < 2)
        {
            return false;
        }

        quote = value[0]; // This can be single or a double quote
        if (quote != '"' && quote != '\'')
        {
            return false;
        }

        if (value.Last() != quote)
        {
            return false;
        }

        try
        {
            result = Regex.Unescape(value.Substring(1, value.Length - 2));
            return true;
        }
        catch
        {
            // Ignore Exception, just continue and return false.
        }

        return false;
    }
}