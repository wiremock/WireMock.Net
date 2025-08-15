// Copyright © WireMock.Net

namespace WireMock.Net.Extensions.Routing.Delegates;

/// <summary>
/// Represents a handler for processing WireMock.Net HTTP requests and returning a response asynchronously.
/// </summary>
/// <param name="requestMessage">The incoming request message.</param>
/// <returns>A task that resolves to a <see cref="ResponseMessage"/>.</returns>
public delegate Task<ResponseMessage> WireMockHttpRequestHandler(IRequestMessage requestMessage);
