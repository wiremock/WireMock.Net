// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;

namespace WireMock.Owin.Mappers;

/// <summary>
/// IOwinResponseMapper
/// </summary>
internal interface IOwinResponseMapper
{
    /// <summary>
    /// Map ResponseMessage to IResponse.
    /// </summary>
    /// <param name="responseMessage">The ResponseMessage</param>
    /// <param name="response">The HttpResponse</param>
    Task MapAsync(IResponseMessage? responseMessage, HttpResponse response);
}