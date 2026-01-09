// Copyright © WireMock.Net

#if USE_ASPNETCORE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Owin.Mappers;
using WireMock.Owin.OpenTelemetry;
using WireMock.Services;
using WireMock.Util;

namespace WireMock.Owin;

internal partial class AspNetCoreSelfHost : IOwinSelfHost
{
    private const string CorsPolicyName = "WireMock.Net - Policy";

    private readonly CancellationTokenSource _cts = new();
    private readonly IWireMockMiddlewareOptions _wireMockMiddlewareOptions;
    private readonly IWireMockLogger _logger;
    private readonly HostUrlOptions _urlOptions;

    private Exception _runningException;
    private IWebHost _host;

    public bool IsStarted { get; private set; }

    public List<string> Urls { get; } = new();

    public List<int> Ports { get; } = new();

    public Exception RunningException => _runningException;

    public AspNetCoreSelfHost(IWireMockMiddlewareOptions wireMockMiddlewareOptions, HostUrlOptions urlOptions)
    {
        Guard.NotNull(wireMockMiddlewareOptions);
        Guard.NotNull(urlOptions);

        _logger = wireMockMiddlewareOptions.Logger ?? new WireMockConsoleLogger();

        _wireMockMiddlewareOptions = wireMockMiddlewareOptions;
        _urlOptions = urlOptions;
    }

    public Task StartAsync()
    {
        var builder = new WebHostBuilder();

        // Workaround for https://github.com/wiremock/WireMock.Net/issues/292
        // On some platforms, AppContext.BaseDirectory is null, which causes WebHostBuilder to fail if ContentRoot is not
        // specified (even though we don't actually use that base path mechanism, since we have our own way of configuring
        // a filesystem handler).
        if (string.IsNullOrEmpty(AppContext.BaseDirectory))
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
        }

        _host = builder
            .UseSetting("suppressStatusMessages", "True") // https://andrewlock.net/suppressing-the-startup-and-shutdown-messages-in-asp-net-core/
            .ConfigureAppConfigurationUsingEnvironmentVariables()
            .ConfigureServices(services =>
            {
                services.AddSingleton(_wireMockMiddlewareOptions);
                services.AddSingleton<IMappingMatcher, MappingMatcher>();
                services.AddSingleton<IRandomizerDoubleBetween0And1, RandomizerDoubleBetween0And1>();
                services.AddSingleton<IOwinRequestMapper, OwinRequestMapper>();
                services.AddSingleton<IOwinResponseMapper, OwinResponseMapper>();
                services.AddSingleton<IGuidUtils, GuidUtils>();

#if NETCOREAPP3_1 || NET5_0_OR_GREATER
                AddCors(services);
#endif

                // Configure OpenTelemetry tracing if enabled (throws on unsupported frameworks)
                services.AddWireMockOpenTelemetry(_wireMockMiddlewareOptions);

                _wireMockMiddlewareOptions.AdditionalServiceRegistration?.Invoke(services);
            })
            .Configure(appBuilder =>
            {
                appBuilder.UseMiddleware<GlobalExceptionMiddleware>();

#if NETCOREAPP3_1 || NET5_0_OR_GREATER
                UseCors(appBuilder);
#endif
                _wireMockMiddlewareOptions.PreWireMockMiddlewareInit?.Invoke(appBuilder);

                appBuilder.UseMiddleware<WireMockMiddleware>();

                _wireMockMiddlewareOptions.PostWireMockMiddlewareInit?.Invoke(appBuilder);
            })
            .UseKestrel(options =>
            {
                SetKestrelOptionsLimits(options);

                SetHttpsAndUrls(options, _wireMockMiddlewareOptions, _urlOptions.GetDetails());
            })
            .ConfigureKestrelServerOptions()

#if NETSTANDARD1_3
            .UseUrls(_urlOptions.GetDetails().Select(u => u.Url).ToArray())
#endif
            .Build();

        return RunHost(_cts.Token);
    }

        private Task RunHost(CancellationToken token)
        {
            try
            {
#if NETCOREAPP3_1 || NET5_0_OR_GREATER
                var appLifetime = _host.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
#else
                var appLifetime = _host.Services.GetRequiredService<IApplicationLifetime>();
#endif
                appLifetime.ApplicationStarted.Register(() =>
                {
                    var addresses = _host.ServerFeatures
                        .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()!
                        .Addresses;

                    foreach (var address in addresses)
                    {
                        Urls.Add(address.Replace("0.0.0.0", "localhost").Replace("[::]", "localhost"));

                        PortUtils.TryExtract(address, out _, out _, out _, out _, out var port);
                        Ports.Add(port);
                    }

                IsStarted = true;
            });

#if NETSTANDARD1_3
            _logger.Info("Server using netstandard1.3");
#elif NETSTANDARD2_0
            _logger.Info("Server using netstandard2.0");
#elif NETSTANDARD2_1
            _logger.Info("Server using netstandard2.1");
#elif NETCOREAPP3_1
            _logger.Info("Server using .NET Core App 3.1");
#elif NET5_0
            _logger.Info("Server using .NET 5.0");
#elif NET6_0
            _logger.Info("Server using .NET 6.0");
#elif NET7_0
            _logger.Info("Server using .NET 7.0");
#elif NET8_0
            _logger.Info("Server using .NET 8.0");
#elif NET46
            _logger.Info("Server using .NET Framework 4.6.1 or higher");
#endif

#if NETSTANDARD1_3
            return Task.Run(() =>
            {
                _host.Run(token);
            });
#else
            return _host.RunAsync(token);
#endif
        }
        catch (Exception e)
        {
            _runningException = e;
            _logger.Error(e.ToString());

            IsStarted = false;

            return Task.CompletedTask;
        }
    }

    public Task StopAsync()
    {
        _cts.Cancel();

        IsStarted = false;
#if NETSTANDARD1_3
        return Task.CompletedTask;
#else
        return _host.StopAsync();
#endif
    }
}
#endif