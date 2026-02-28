// Copyright © WireMock.Net

using WireMock.Models;

namespace WireMock.Extensions;

internal static class TimeSettingsExtensions
{
    public static bool IsValid(this ITimeSettings? settings)
    {
        if (settings == null)
        {
            return true;
        }

        var now = DateTime.Now;
        var start = settings.Start ?? now;
        DateTime end;

        if (settings.End != null)
        {
            end = settings.End.Value;
        }
        else if (settings.TTL != null)
        {
            end = start.AddSeconds(settings.TTL.Value);
        }
        else
        {
            end = DateTime.MaxValue;
        }

        return now >= start && now <= end;
    }
}