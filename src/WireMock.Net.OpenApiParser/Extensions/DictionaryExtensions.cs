// Copyright © WireMock.Net

#if NETSTANDARD2_0
using System.Collections.Generic;

namespace WireMock.Net.OpenApiParser.Extensions;

internal static class DictionaryExtensions
{
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue>? dictionary, TKey key, TValue value)
    {
        if (dictionary is null || dictionary.ContainsKey(key))
        {
            return false;
        }

        dictionary[key] = value;

        return true;
    }
}
#endif