// Copyright © WireMock.Net

using System.Text.Json;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// Generic AbstractSystemTextJsonPartialMatcher - uses System.Text.Json instead of Newtonsoft.Json.
/// </summary>
public abstract class AbstractSystemTextJsonPartialMatcher : SystemTextJsonMatcher
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSystemTextJsonPartialMatcher"/> class.
    /// </summary>
    protected AbstractSystemTextJsonPartialMatcher(string value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSystemTextJsonPartialMatcher"/> class.
    /// </summary>
    protected AbstractSystemTextJsonPartialMatcher(object value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSystemTextJsonPartialMatcher"/> class.
    /// </summary>
    protected AbstractSystemTextJsonPartialMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
        : base(matchBehaviour, value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    protected override bool IsMatch(JsonElement value, JsonElement? input)
    {
        if (input == null)
        {
            return false;
        }

        var inputElement = input.Value;

        // Regex on a string value
        if (Regex && value.ValueKind == JsonValueKind.String)
        {
            var valueAsString = value.GetString()!;
            var inputAsString = GetStringValue(inputElement);

            var (valid, result) = RegexUtils.MatchRegex(valueAsString, inputAsString);
            if (valid)
            {
                return result;
            }
        }

        // Guid comparison: both strings, both parseable as Guid
        if (value.ValueKind == JsonValueKind.String && inputElement.ValueKind == JsonValueKind.String)
        {
            var valueStr = value.GetString()!;
            var inputStr = inputElement.GetString()!;
            if (Guid.TryParse(valueStr, out var vg) && Guid.TryParse(inputStr, out var ig))
            {
                return IsMatch(vg.ToString(), ig.ToString());
            }
        }

        // Type mismatch (after regex/guid checks)
        if (value.ValueKind != inputElement.ValueKind)
        {
            return false;
        }

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
            {
                var nestedValues = value.EnumerateObject().ToArray();
                if (nestedValues.Length == 0)
                {
                    return true;
                }

                return nestedValues.All(pair =>
                {
                    var selected = SelectElement(inputElement, pair.Name);
                    return selected != null && IsMatch(pair.Value, selected.Value);
                });
            }

            case JsonValueKind.Array:
            {
                var valuesArray = value.EnumerateArray().ToArray();
                if (valuesArray.Length == 0)
                {
                    return true;
                }

                var tokenArray = inputElement.EnumerateArray().ToArray();
                if (tokenArray.Length == 0)
                {
                    return false;
                }

                return valuesArray.All(subFilter => tokenArray.Any(subToken => IsMatch(subFilter, subToken)));
            }

            default:
                return IsMatch(GetStringValue(value), GetStringValue(inputElement));
        }
    }

    /// <summary>
    /// Check if two strings are a match (matching can be done exact or wildcard).
    /// </summary>
    protected abstract bool IsMatch(string value, string input);

    /// <summary>
    /// Selects a <see cref="JsonElement"/> from an object using a key that may be a plain property name,
    /// a dotted path (e.g. "a.b.c") or bracket notation (e.g. "['name.with.dot']"),
    /// mirroring Newtonsoft's <c>SelectToken</c> + direct indexer fallback.
    /// </summary>
    private static JsonElement? SelectElement(JsonElement input, string key)
    {
        if (input.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        // Direct property access (also handles keys containing colons or dots that are literal property names)
        if (input.TryGetProperty(key, out var direct))
        {
            return direct;
        }

        // Bracket notation: ['property.name.with.dots']
        if (key.StartsWith("['") && key.EndsWith("']"))
        {
            var propertyName = key.Substring(2, key.Length - 4);
            return input.TryGetProperty(propertyName, out var bracketValue) ? bracketValue : null;
        }

        // Dotted path: a.b.c
        if (key.Contains('.'))
        {
            var parts = key.Split('.');
            var current = input;
            foreach (var part in parts)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out var next))
                {
                    return null;
                }

                current = next;
            }

            return current;
        }

        return null;
    }

    private static string GetStringValue(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.String
            ? element.GetString()!
            : element.GetRawText();
    }
}
