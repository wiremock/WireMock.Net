// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Owin.Mappers;

namespace WireMock.Owin;

internal class GlobalExceptionMiddleware
{
    private readonly IWireMockMiddlewareOptions _options;
    private readonly IOwinResponseMapper _responseMapper;
    private readonly IResponseMessageBuilder _responseMessageBuilder;

    public GlobalExceptionMiddleware(RequestDelegate next, IWireMockMiddlewareOptions options, IOwinResponseMapper responseMapper, IResponseMessageBuilder responseMessageBuilder)
    {
        Next = next;
        _options = Guard.NotNull(options);
        _responseMapper = Guard.NotNull(responseMapper);
        _responseMessageBuilder = Guard.NotNull(responseMessageBuilder);
    }

    public RequestDelegate Next { get; }

    public Task Invoke(HttpContext ctx)
    {
        return InvokeInternalAsync(ctx);
    }

    private async Task InvokeInternalAsync(HttpContext ctx)
    {
        try
        {
            await Next.Invoke(ctx).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _options.Logger.Error("HttpStatusCode set to 500 {0}", ex);
            await _responseMapper.MapAsync(_responseMessageBuilder.Create(500, JsonConvert.SerializeObject(ex)), ctx.Response).ConfigureAwait(false);
        }
    }
}