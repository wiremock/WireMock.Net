// Copyright © WireMock.Net

using System.Text;

namespace WireMock.Util;

internal static class StreamUtils
{
    public static Stream CreateStream(string s)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(s));
    }
}