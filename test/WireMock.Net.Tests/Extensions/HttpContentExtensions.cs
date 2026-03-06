// Copyright © WireMock.Net

#if !NET5_0_OR_GREATER
namespace System.Net.Http;

/// <summary>
/// Extension methods for HttpContent to provide CancellationToken support in frameworks before .NET 5.0.
/// </summary>
internal static class HttpContentExtensions
{
    public static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken _)
    {
        return content.ReadAsStringAsync();
    }

    public static Task<string> ReadAsStringAsync(this StringContent content, CancellationToken _)
    {
        return content.ReadAsStringAsync();
    }

    public static Task<byte[]> ReadAsByteArrayAsync(this HttpContent content, CancellationToken _)
    {
        return content.ReadAsByteArrayAsync();
    }

    public static Task<byte[]> ReadAsByteArrayAsync(this ByteArrayContent content, CancellationToken _)
    {
        return content.ReadAsByteArrayAsync();
    }
}
#endif