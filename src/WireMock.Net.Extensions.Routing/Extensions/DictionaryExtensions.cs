using System.Collections.Generic;
using System.Collections.Immutable;

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class DictionaryExtensions
{
    public static IDictionary<TKey, TValue> AddIf<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        bool condition,
        TKey key,
        TValue value,
        IEqualityComparer<TKey>? keyComparer = null)
        where TKey : notnull =>
        condition
            ? source.ToImmutableDictionary(keyComparer).Add(key, value)
            : source;
}
