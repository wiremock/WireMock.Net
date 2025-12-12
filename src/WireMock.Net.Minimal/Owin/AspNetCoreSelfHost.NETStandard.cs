// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CertificateLoader = WireMock.HttpsCertificate.CertificateLoader;

namespace WireMock.Owin;

internal partial class AspNetCoreSelfHost
{
    private static void SetKestrelOptionsLimits(KestrelServerOptions options)
    {
        options.Limits.MaxRequestBodySize = null; // https://stackoverflow.com/questions/46738364/increase-upload-request-length-limit-in-kestrel
        options.Limits.MaxRequestBufferSize = null;
        options.Limits.MaxRequestHeaderCount = 100;
        options.Limits.MaxResponseBufferSize = null;
    }

    private static void SetHttpsAndUrls(KestrelServerOptions kestrelOptions, IWireMockMiddlewareOptions wireMockMiddlewareOptions, IEnumerable<HostUrlDetails> urlDetails)
    {
        foreach (var urlDetail in urlDetails)
        {
            if (urlDetail.IsHttps)
            {
                Listen(kestrelOptions, urlDetail, listenOptions =>
                {
                    listenOptions.UseHttps(options =>
                    {
                        if (wireMockMiddlewareOptions.CustomCertificateDefined)
                        {
                            options.ServerCertificate = CertificateLoader.LoadCertificate(wireMockMiddlewareOptions, urlDetail.Host);
                        }

                        options.ClientCertificateMode = wireMockMiddlewareOptions.ClientCertificateMode;
                        if (wireMockMiddlewareOptions.AcceptAnyClientCertificate)
                        {
                            options.ClientCertificateValidation = (_, _, _) => true;
                        }
                    });

                    if (urlDetail.IsHttp2)
                    {
                        SetHttp2AsProtocolsOnListenOptions(listenOptions);
                    }
                });
                continue;
            }

            if (urlDetail.IsHttp2)
            {
                Listen(kestrelOptions, urlDetail, SetHttp2AsProtocolsOnListenOptions);
                continue;
            }

            Listen(kestrelOptions, urlDetail, _ => { });
        }
    }

    private static void SetHttp2AsProtocolsOnListenOptions(ListenOptions listenOptions)
    {
#if NET8_0_OR_GREATER
        listenOptions.Protocols = HttpProtocols.Http2;
#else
        throw new NotSupportedException("HTTP/2 is only supported in .NET 8 or greater.");
#endif
    }

    private static void Listen(KestrelServerOptions kestrelOptions, HostUrlDetails urlDetail, Action<ListenOptions> configure)
    {
        // Listens on any IP with the given port.
        if (urlDetail is { Port: > 0, Host: "0.0.0.0" })
        {
            kestrelOptions.ListenAnyIP(urlDetail.Port, configure);
            return;
        }

        // Listens on ::1 and 127.0.0.1 with the given port.
        if (urlDetail is { Port: > 0, Host: "localhost" or "127.0.0.1" or "::1" })
        {
            kestrelOptions.ListenLocalhost(urlDetail.Port, configure);
            return;
        }

        // Try to parse the host as a valid IP address and bind to the given IP address and port.
        if (IPAddress.TryParse(urlDetail.Host, out var ipAddress))
        {
            kestrelOptions.Listen(ipAddress, urlDetail.Port, configure);
            return;
        }

        // Otherwise, listen on all IPs.
        kestrelOptions.ListenAnyIP(urlDetail.Port, configure);
    }
}

internal static class IWebHostBuilderExtensions
{
    internal static IWebHostBuilder ConfigureAppConfigurationUsingEnvironmentVariables(this IWebHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration(config =>
        {
            config.AddEnvironmentVariables();
        });
    }

    internal static IWebHostBuilder ConfigureKestrelServerOptions(this IWebHostBuilder builder)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.Configure<KestrelServerOptions>(context.Configuration.GetSection("Kestrel"));
        });
    }
}