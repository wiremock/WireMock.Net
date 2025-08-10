// Copyright © WireMock.Net

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class StringExtensions
{
    public static string ToMatchFullStringRegex(this string regex) =>
        $"^{regex}$";
}
