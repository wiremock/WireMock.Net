// Copyright © WireMock.Net

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

//#if !USE_ASPNETCORE
//using IResponse = Microsoft.Owin.IOwinResponse;
//#else
//using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
//#endif

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