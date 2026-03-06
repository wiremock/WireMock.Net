// Copyright © WireMock.Net

#if NET8_0
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Types;

namespace WireMock.Owin;

internal partial class AspNetCoreSelfHost
{
    private const string CorsPolicyName = "WireMock.Net - Policy";

    public void AddCors(IServiceCollection services)
    {
        if (_wireMockMiddlewareOptions.CorsPolicyOptions > CorsPolicyOptions.None)
        {
            /* https://stackoverflow.com/questions/31942037/how-to-enable-cors-in-asp-net-core */
            /* Enable Cors */
            services.AddCors(corsOptions => corsOptions
                .AddPolicy(CorsPolicyName,
                    corsPolicyBuilder =>
                    {
                        if (_wireMockMiddlewareOptions.CorsPolicyOptions.Value.HasFlag(CorsPolicyOptions.AllowAnyHeader))
                        {
                            corsPolicyBuilder.AllowAnyHeader();
                        }

                        if (_wireMockMiddlewareOptions.CorsPolicyOptions.Value.HasFlag(CorsPolicyOptions.AllowAnyMethod))
                        {
                            corsPolicyBuilder.AllowAnyMethod();
                        }

                        if (_wireMockMiddlewareOptions.CorsPolicyOptions.Value.HasFlag(CorsPolicyOptions.AllowAnyOrigin))
                        {
                            corsPolicyBuilder.AllowAnyOrigin();
                        }
                    }));
        }
    }

    public void UseCors(IApplicationBuilder appBuilder)
    {
        if (_wireMockMiddlewareOptions.CorsPolicyOptions > CorsPolicyOptions.None)
        {
            /* Use Cors */
            appBuilder.UseCors(CorsPolicyName);
        }
    }
}
#endif