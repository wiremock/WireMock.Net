// Copyright © WireMock.Net

using Stef.Validation;

namespace WireMock.Util;

/// <summary>
/// Some IDictionary Extensions
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Loops the dictionary and executes the specified action.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary to loop (can be null).</param>
    /// <param name="action">The action.</param>
    public static void Loop<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary, Action<TKey, TValue> action)
        where TKey : notnull
    {
        Guard.NotNull(action);

        if (dictionary != null)
        {
            foreach (var entry in dictionary)
            {
                action(entry.Key, entry.Value);
            }
        }
    }
}