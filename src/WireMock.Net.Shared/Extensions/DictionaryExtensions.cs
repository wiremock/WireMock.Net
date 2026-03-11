// Copyright © WireMock.Net

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Stef.Validation;

namespace WireMock.Extensions;

internal static class DictionaryExtensions
{
    public static bool TryGetStringValue(this IDictionary dictionary, string key, [NotNullWhen(true)] out string? value)
    {
        Guard.NotNull(dictionary);
        
        if (dictionary[key] is string valueIsString)
        {
            value = valueIsString;
            return true;
        }

        var valueToString = dictionary[key]?.ToString();
        if (valueToString != null)
        {
            value = valueToString;
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetValue<T>(this IDictionary<object, object?> dictionary, string key, [NotNullWhen(true)] out T? value)
    {
        Guard.NotNull(dictionary);

        if (dictionary[key] is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }
}