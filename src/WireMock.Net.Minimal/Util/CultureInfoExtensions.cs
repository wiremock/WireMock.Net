// Copyright © WireMock.Net

using System;
using System.Globalization;

namespace WireMock.Util;

internal static class CultureInfoUtils
{
    public static readonly CultureInfo CultureInfoEnUS = new("en-US");

    public static CultureInfo Parse(string? value)
    {
        if (value is null)
        {
            return CultureInfo.CurrentCulture;
        }

        try
        {
//#if !NETSTANDARD1_3
            if (int.TryParse(value, out var culture))
            {
                return new CultureInfo(culture);
            }
//#endif
            if (string.Equals(value, nameof(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.CurrentCulture;
            }

            if (string.Equals(value, nameof(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.InvariantCulture;
            }

            return new CultureInfo(value);
        }
        catch
        {
            return CultureInfo.CurrentCulture;
        }
    }
}