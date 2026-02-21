// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;

namespace WireMock.Owin.Mappers;

/// <summary>
/// IOwinRequestMapper
/// </summary>
internal interface IOwinRequestMapper
{
    /// <summary>
    /// MapAsync IRequest to RequestMessage
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <param name="options">The WireMockMiddlewareOptions</param>
    /// <returns>RequestMessage</returns>
    Task<RequestMessage> MapAsync(HttpContext context, IWireMockMiddlewareOptions options);
}