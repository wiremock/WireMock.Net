// Copyright © WireMock.Net

#if NET8_0_OR_GREATER
using System;
using System.Net.Http;
using WireMock.Server;

namespace WireMock.Http;

internal class WireMockHttpClientFactory(WireMockServer server, params DelegatingHandler[] handlers) : IHttpClientFactory
{
    private readonly Lazy<HttpClient> _lazyHttpClient = new(() => server.CreateClient());

    public HttpClient CreateClient(string name)
    {
        return handlers.Length > 0 ? server.CreateClient(handlers) : _lazyHttpClient.Value;
    }
}
#endif