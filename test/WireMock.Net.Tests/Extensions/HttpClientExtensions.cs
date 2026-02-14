// Copyright © WireMock.Net

#if !NET5_0_OR_GREATER
namespace System.Net.Http;

/// <summary>
/// Extension methods for HttpClient to provide CancellationToken support in frameworks before .NET 5.0.
/// </summary>
internal static class HttpClientExtensions
{
    public static Task<Stream> GetStreamAsync(this HttpClient client, Uri requestUri, CancellationToken _)
    {
        return client.GetStreamAsync(requestUri);
    }

    public static Task<HttpResponseMessage> GetAsync(this HttpClient client, Uri requestUri, CancellationToken _)
    {
        return client.GetAsync(requestUri);
    }

    public static Task<string> GetStringAsync(this HttpClient client, string requestUri, CancellationToken _)
    {
        return client.GetStringAsync(requestUri);
    }

    public static Task<string> GetStringAsync(this HttpClient client, Uri requestUri, CancellationToken _)
    {
        return client.GetStringAsync(requestUri);
    }

    public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken _)
    {
        return client.PostAsync(requestUri, content);
    }

    public static Task<HttpResponseMessage> PostAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken _)
    {
        return client.PostAsync(requestUri, content);
    }

    public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request, CancellationToken _)
    {
        return client.SendAsync(request);
    }
}
#endif