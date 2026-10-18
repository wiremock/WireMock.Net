// Copyright © WireMock.Net

using System.Globalization;

namespace WireMock.Net.OpenApiParser.Utils;

internal static class DateTimeUtils
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

    public static string ToRfc3339DateTime(DateTime dateTime)
    {
        return dateTime.ToString(DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
    }

    public static string ToRfc3339Date(DateTime dateTime)
    {
        return dateTime.ToString(DateFormat, DateTimeFormatInfo.InvariantInfo);
    }
}