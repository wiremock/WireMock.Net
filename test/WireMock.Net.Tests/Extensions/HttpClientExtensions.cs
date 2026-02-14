// Copyright © WireMock.Net

#if !NET5_0_OR_GREATER
namespace System.Net.Http;

/// <summary>
/// Extension methods for HttpClient to provide CancellationToken support in frameworks before .NET 5.0.
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Sends a GET request to the specified Uri as an asynchronous operation.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<HttpResponseMessage> GetAsync(this HttpClient client, Uri requestUri, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return client.GetAsync(requestUri);
    }

    /// <summary>
    /// Sends a GET request to the specified Uri and return the response body as a string.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<string> GetStringAsync(this HttpClient client, string requestUri, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return client.GetStringAsync(requestUri);
    }

    /// <summary>
    /// Sends a GET request to the specified Uri and return the response body as a string.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<string> GetStringAsync(this HttpClient client, Uri requestUri, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return client.GetStringAsync(requestUri);
    }

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return client.PostAsync(requestUri, content);
    }

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<HttpResponseMessage> PostAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return client.PostAsync(requestUri, content);
    }

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return client.SendAsync(request);
    }
}

/// <summary>
/// Extension methods for HttpContent to provide CancellationToken support in frameworks before .NET 5.0.
/// </summary>
internal static class HttpContentExtensions
{
    /// <summary>
    /// Serialize the HTTP content to a string as an asynchronous operation.
    /// This extension is only used in frameworks prior to .NET 5.0 where CancellationToken is not supported.
    /// </summary>
    public static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken _)
    {
        // In older frameworks, we ignore the cancellation token since it's not supported
        return content.ReadAsStringAsync();
    }
}
#endif