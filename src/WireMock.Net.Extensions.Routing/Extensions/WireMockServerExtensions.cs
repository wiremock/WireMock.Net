// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Net.Extensions.Routing.Delegates;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Extensions.Routing.Extensions;

/// <summary>
/// Provides extension methods for mapping HTTP requests to handlers in <see cref="WireMockServer"/>.
/// </summary>
public static class WireMockServerExtensions
{
    /// <summary>
    /// Maps a request to a WireMock.Net server using the specified method, path matcher, and request handler.
    /// </summary>
    /// <param name="source">The WireMock.Net server to extend.</param>
    /// <param name="method">The HTTP method to match.</param>
    /// <param name="pathMatcher">The matcher for the request path.</param>
    /// <param name="httpRequestHandler">The handler to process the request.</param>
    /// <returns>The current <see cref="WireMockServer"/> instance.</returns>
    public static WireMockServer Map(
        this WireMockServer source,
        string method,
        IStringMatcher pathMatcher,
        WireMockHttpRequestHandler httpRequestHandler)
    {
        source
            .Given(Request.Create().WithPath(pathMatcher).UsingMethod(method))
            .RespondWith(Response.Create().WithCallback(req => httpRequestHandler(req)));
        return source;
    }
}
