// Copyright © WireMock.Net

namespace WireMock.Net.Testcontainers.Utils;

internal static class CombineUtils
{
    internal static List<T> Combine<T>(List<T> oldValue, List<T> newValue)
    {
        return oldValue.Union(newValue).ToList();
    }

    internal static Dictionary<TKey, TValue> Combine<TKey, TValue>(Dictionary<TKey, TValue> oldValue, Dictionary<TKey, TValue> newValue)
        where TKey : notnull
    {
        return oldValue.Union(newValue).ToDictionary(item => item.Key, item => item.Value);
    }
}