// Copyright © WireMock.Net

#if NETSTANDARD2_0
namespace System.Collections.Generic;

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