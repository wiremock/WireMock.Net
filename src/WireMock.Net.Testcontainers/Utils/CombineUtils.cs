// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Net.Testcontainers.Utils;

internal static class CombineUtils
{
    internal static List<T> Combine<T>(List<T> oldValue, List<T> newValue)
    {
        return oldValue.Concat(newValue).ToList();
    }

    internal static Dictionary<TKey, TValue> Combine<TKey, TValue>(Dictionary<TKey, TValue> oldValue, Dictionary<TKey, TValue> newValue)
    {
        return newValue
            .Concat(oldValue.Where(item => !newValue.Keys.Contains(item.Key)))
            .ToDictionary(item => item.Key, item => item.Value);
    }
}