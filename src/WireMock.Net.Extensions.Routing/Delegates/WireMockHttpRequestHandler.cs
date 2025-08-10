// Copyright © WireMock.Net

namespace WireMock.Net.Extensions.Routing.Delegates;

/// <summary>
/// Represents a handler for processing WireMock HTTP requests and returning a response asynchronously.
/// </summary>
/// <param name="requests">The incoming request message.</param>
/// <returns>A task that resolves to a <see cref="ResponseMessage"/>.</returns>
public delegate Task<ResponseMessage> WireMockHttpRequestHandler(IRequestMessage requests);
