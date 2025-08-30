// Copyright © WireMock.Net

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
//#if !USE_ASPNETCORE
//using IRequest = Microsoft.Owin.IOwinRequest;
//#else
//using IRequest = Microsoft.AspNetCore.Http.HttpRequest;
//#endif

namespace WireMock.Owin.Mappers;

/// <summary>
/// IOwinRequestMapper
/// </summary>
internal interface IOwinRequestMapper
{
    /// <summary>
    /// MapAsync IRequest to RequestMessage
    /// </summary>
    /// <param name="request">The HttpRequest</param>
    /// <param name="options">The WireMockMiddlewareOptions</param>
    /// <returns>RequestMessage</returns>
    Task<RequestMessage> MapAsync(HttpRequest request, IWireMockMiddlewareOptions options);
}