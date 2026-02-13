// Copyright © WireMock.Net

using System.Globalization;
using System.Text.RegularExpressions;
using WireMock.Constants;

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
    public static string Replace(this string text, string oldValue, string newValue, StringComparison stringComparison)
    {
        var options = stringComparison == StringComparison.OrdinalIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        return Regex.Replace(text, oldValue, newValue, options, RegexConstants.DefaultTimeout);
    }
#endif
}