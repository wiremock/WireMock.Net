// Copyright © WireMock.Net

using System.Globalization;

namespace System;

internal static class StringExtensions
{
    // See https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
    public static string GetDeterministicHashCodeAsString(this string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                {
                    break;
                }

                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            int result = hash1 + hash2 * 1566083941;

            return result.ToString(CultureInfo.InvariantCulture).Replace('-', '_');
        }
    }

#if !NET8_0_OR_GREATER
    public static string Replace(
            this string source,
            string oldValue,
            string? newValue,
            StringComparison comparisonType)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (string.IsNullOrEmpty(oldValue))
        {
            throw new ArgumentException("oldValue cannot be null or empty.", nameof(oldValue));
        }

        newValue ??= string.Empty;

        int pos = 0;
        int index = source.IndexOf(oldValue, pos, comparisonType);

        if (index < 0)
        {
            return source; // nothing to replace
        }

        var result = new System.Text.StringBuilder(source.Length);

        while (index >= 0)
        {
            // append unchanged part
            result.Append(source, pos, index - pos);

            // append replacement
            result.Append(newValue);

            pos = index + oldValue.Length;

            index = source.IndexOf(oldValue, pos, comparisonType);
        }

        // append remainder
        result.Append(source, pos, source.Length - pos);

        return result.ToString();
    }
#endif
}