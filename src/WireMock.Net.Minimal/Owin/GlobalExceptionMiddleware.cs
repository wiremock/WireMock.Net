// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.Owin.Mappers;
using Stef.Validation;
using Microsoft.AspNetCore.Http;
//#if !USE_ASPNETCORE
//using Microsoft.Owin;
//using IContext = Microsoft.Owin.IOwinContext;
//using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
//using Next = Microsoft.Owin.OwinMiddleware;
//#else
//using OwinMiddleware = System.Object;
//using IContext = Microsoft.AspNetCore.Http.HttpContext;
//using Next = Microsoft.AspNetCore.Http.RequestDelegate;
//#endif


namespace WireMock.Owin;

internal class GlobalExceptionMiddleware //: OwinMiddleware
{
    private readonly IWireMockMiddlewareOptions _options;
    private readonly IOwinResponseMapper _responseMapper;

//#if !USE_ASPNETCORE
//        public GlobalExceptionMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinResponseMapper responseMapper) : base(next)
//        {
//            _options = Guard.NotNull(options);
//            _responseMapper = Guard.NotNull(responseMapper);;
//        }
//#else
    public GlobalExceptionMiddleware(RequestDelegate next, IWireMockMiddlewareOptions options, IOwinResponseMapper responseMapper)
    {
        Next = next;
        _options = Guard.NotNull(options);
        _responseMapper = Guard.NotNull(responseMapper);
    }
//#endif

//#if USE_ASPNETCORE
    public RequestDelegate? Next { get; }
//#endif

//#if !USE_ASPNETCORE
//        public override Task Invoke(IContext ctx)
//#else
    public Task Invoke(HttpContext ctx)
//#endif
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