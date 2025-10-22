// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.Owin.Mappers;
using Stef.Validation;
using Microsoft.AspNetCore.Http;

namespace WireMock.Owin;

internal class GlobalExceptionMiddleware
{
    private readonly IWireMockMiddlewareOptions _options;
    private readonly IOwinResponseMapper _responseMapper;

    public GlobalExceptionMiddleware(RequestDelegate next, IWireMockMiddlewareOptions options, IOwinResponseMapper responseMapper)
    {
        Next = next;
        _options = Guard.NotNull(options);
        _responseMapper = Guard.NotNull(responseMapper);
    }

    public RequestDelegate? Next { get; }

    public Task Invoke(HttpContext ctx)
    {
        return InvokeInternalAsync(ctx);
    }

    private async Task InvokeInternalAsync(HttpContext ctx)
    {
        try
        {
            if (Next != null)
            {
                await Next.Invoke(ctx).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _options.Logger.Error("HttpStatusCode set to 500 {0}", ex);
            await _responseMapper.MapAsync(ResponseMessageBuilder.Create(500, JsonConvert.SerializeObject(ex)), ctx.Response).ConfigureAwait(false);
        }
    }
}