// Copyright © WireMock.Net

namespace WireMock.Util;

internal interface IDateTimeUtils
{
    DateTime UtcNow { get; }
}

internal class DateTimeUtils : IDateTimeUtils
{
    public DateTime UtcNow => DateTime.UtcNow;
}