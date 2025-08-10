using System.Collections.Generic;
using System.Linq;
using WireMock.Net.Extensions.Routing.Delegates;

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class WireMockHttpRequestHandlerExtensions
{
    public static WireMockHttpRequestHandler UseMiddleware(
        this WireMockHttpRequestHandler handler, WireMockMiddleware middleware) =>
        middleware(handler);

    public static WireMockHttpRequestHandler UseMiddlewareCollection(
        this WireMockHttpRequestHandler handler,
        IReadOnlyCollection<WireMockMiddleware> middlewareCollection) =>
        middlewareCollection.Aggregate(handler, UseMiddleware);
}
