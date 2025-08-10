namespace WireMock.Net.Extensions.Routing.Delegates;

/// <summary>
/// Represents a middleware component for WireMock HTTP request handling.
/// </summary>
/// <param name="next">The next request handler in the middleware pipeline.</param>
/// <returns>A <see cref="WireMockHttpRequestHandler"/> that processes the request.</returns>
public delegate WireMockHttpRequestHandler WireMockMiddleware(WireMockHttpRequestHandler next);
