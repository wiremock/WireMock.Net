// Copyright © WireMock.Net


using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;

internal static class DictionaryExtensions
{
#if NETSTANDARD2_0
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue>? dictionary, TKey key, TValue value)
    {
        if (dictionary is null || dictionary.ContainsKey(key))
        {
            return false;
        }

        dictionary[key] = value;

        return true;
    }
#endif

    public static bool TryGetFirstValue<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary, out TValue? value)
    {
        if (dictionary != null && dictionary.Count > 0)
        {
            value = dictionary.First().Value;
            return true;
        }

        value = default;
        return false;
    }
}