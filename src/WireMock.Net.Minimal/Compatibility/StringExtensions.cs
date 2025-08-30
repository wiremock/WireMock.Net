// Copyright © WireMock.Net

#if NET451 || NET452 || NET46 || NET461 || NET48 || NETSTANDARD1_3 || NETSTANDARD2_0
using System.Text.RegularExpressions;
using WireMock.Constants;

// ReSharper disable once CheckNamespace
namespace System;

internal static class StringExtensions
{
    public static string Replace(this string text, string oldValue, string newValue, StringComparison stringComparison)
    {
        var options = stringComparison == StringComparison.OrdinalIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        return Regex.Replace(text, oldValue, newValue, options, RegexConstants.DefaultTimeout);
    }
}
#endif