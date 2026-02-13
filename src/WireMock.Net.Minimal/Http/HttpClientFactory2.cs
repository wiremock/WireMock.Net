// Copyright © WireMock.Net

using System.Linq;
using System.Net.Http;

namespace WireMock.Http;

internal static class HttpClientFactory2
{
    public static HttpClient Create(params DelegatingHandler[] handlers)
    {
        var handler = CreateHandlerPipeline(new HttpClientHandler(), handlers);
        return new HttpClient(handler);
    }

    public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
        var handler = CreateHandlerPipeline(innerHandler, handlers);
        return new HttpClient(handler);
    }

    private static HttpMessageHandler CreateHandlerPipeline(HttpMessageHandler handler, params DelegatingHandler[] delegatingHandlers)
    {
        if (delegatingHandlers.Length == 0)
        {
            return handler;
        }

        var next = handler;

        foreach (var delegatingHandler in Enumerable.Reverse(delegatingHandlers))
        {
            delegatingHandler.InnerHandler = next;
            next = delegatingHandler;
        }

        return next;
    }
}