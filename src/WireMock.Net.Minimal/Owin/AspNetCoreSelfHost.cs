// Copyright © WireMock.Net

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Owin.Mappers;
using WireMock.Serialization;
using WireMock.Services;
using WireMock.Util;

#if NET10_0
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
#endif

namespace WireMock.Owin;

internal partial class AspNetCoreSelfHost
{
    private readonly CancellationTokenSource _cts = new();
    private readonly IWireMockMiddlewareOptions _wireMockMiddlewareOptions;
    private readonly IWireMockLogger _logger;
    private readonly HostUrlOptions _urlOptions;

#if NET10_0
    private Microsoft.Extensions.Hosting.IHost _host = null!;
#else
    private IWebHost _host = null!;
#endif

    public bool IsStarted { get; private set; }

    public List<string> Urls { get; } = [];

    public List<int> Ports { get; } = [];

    public Exception? RunningException { get; private set; }

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
#if NET10_0
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            // Workaround for https://github.com/wiremock/WireMock.Net/issues/292
            // On some platforms, AppContext.BaseDirectory is null, which causes WebHostBuilder to fail if ContentRoot is not
            // specified (even though we don't actually use that base path mechanism, since we have our own way of configuring
            // a filesystem handler).
            ContentRootPath = string.IsNullOrEmpty(AppContext.BaseDirectory) ? Directory.GetCurrentDirectory() : null
        });

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddEnvironmentVariables();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new WireMockAspNetCoreLoggerProvider(_logger));
        builder.Logging.SetMinimumLevel(LogLevel.Warning);
        builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

        // The default ConsoleLifetime cancels Ctrl+C and shuts down the host itself, which prevents the consuming application from controlling the process lifetime.
        builder.Services.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, NoopHostLifetime>();
        ConfigureServices(builder.Services);
        builder.WebHost.ConfigureKestrel(options =>
        {
            SetKestrelOptionsLimits(options);
            SetHttpsAndUrls(options, _wireMockMiddlewareOptions, _urlOptions.GetDetails());
        });

        var app = builder.Build();
        ConfigureApplication(app);
        _host = app;
#else
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
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(new WireMockAspNetCoreLoggerProvider(_logger));
                logging.SetMinimumLevel(LogLevel.Warning);
            })
            .ConfigureServices(ConfigureServices)
            .Configure(ConfigureApplication)
            .UseKestrel(options =>
            {
                SetKestrelOptionsLimits(options);
                SetHttpsAndUrls(options, _wireMockMiddlewareOptions, _urlOptions.GetDetails());
            })
            .ConfigureKestrelServerOptions()
            .Build();
#endif

        return RunHost(_cts.Token);
    }

    #if NET10_0
    private sealed class NoopHostLifetime : Microsoft.Extensions.Hosting.IHostLifetime
    {
        public Task WaitForStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
#endif

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_wireMockMiddlewareOptions);
        services.AddSingleton(_wireMockMiddlewareOptions.AdminPaths);
        services.AddSingleton<IMappingMatcher, MappingMatcher>();
        services.AddSingleton<IRandomizerDoubleBetween0And1, RandomizerDoubleBetween0And1>();
        services.AddSingleton<IOwinRequestMapper, OwinRequestMapper>();
        services.AddSingleton<IOwinResponseMapper, OwinResponseMapper>();
        services.AddSingleton<IGuidUtils, GuidUtils>();
        services.AddSingleton<IDateTimeUtils, DateTimeUtils>();
        services.AddSingleton<IResponseMessageBuilder, ResponseMessageBuilder>();
        services.AddSingleton<LogEntryMapper>();
        services.AddSingleton<IWireMockMiddlewareLogger, WireMockMiddlewareLogger>();

#if NET8_0_OR_GREATER
        AddCors(services);
#endif
        _wireMockMiddlewareOptions.AdditionalServiceRegistration?.Invoke(services);
    }

    private void ConfigureApplication(IApplicationBuilder appBuilder)
    {
        appBuilder.UseMiddleware<GlobalExceptionMiddleware>();

#if NET8_0_OR_GREATER
        UseCors(appBuilder);

        var webSocketOptions = new WebSocketOptions();
        if (_wireMockMiddlewareOptions.WebSocketSettings?.KeepAliveIntervalSeconds != null)
        {
            webSocketOptions.KeepAliveInterval = TimeSpan.FromSeconds(_wireMockMiddlewareOptions.WebSocketSettings.KeepAliveIntervalSeconds);
        }

        appBuilder.UseWebSockets(webSocketOptions);
#endif
        _wireMockMiddlewareOptions.PreWireMockMiddlewareInit?.Invoke(appBuilder);

        appBuilder.UseMiddleware<WireMockMiddleware>();

        _wireMockMiddlewareOptions.PostWireMockMiddlewareInit?.Invoke(appBuilder);
    }

    private Task RunHost(CancellationToken token)
    {
        try
        {
#if NET8_0_OR_GREATER
            var appLifetime = _host.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
#else
            var appLifetime = _host.Services.GetRequiredService<IApplicationLifetime>();
#endif
            appLifetime.ApplicationStarted.Register(() =>
            {
#if NET10_0
                var addresses = _host.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
                    .Features
                    .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()!
                    .Addresses
                    .ToArray();
#else
                var addresses = _host.ServerFeatures
                    .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()!
                    .Addresses
                    .ToArray();
#endif

                if (_urlOptions.Urls == null)
                {
                    foreach (var address in addresses)
                    {
                        PortUtils.TryExtract(address, out _, out _, out var scheme, out var host, out var port);

                        var replacedHost = ReplaceHostWithLocalhost(host!);
                        var newUrl = $"{scheme}://{replacedHost}:{port}";
                        Urls.Add(newUrl);
                        Ports.Add(port);
                    }
                }
                else
                {
                    var urlOptions = _urlOptions.Urls?.ToArray() ?? [];

                    for (int i = 0; i < urlOptions.Length; i++)
                    {
                        PortUtils.TryExtract(urlOptions[i], out _, out _, out var originalScheme, out _, out _);
                        if (originalScheme!.StartsWith("grpc", StringComparison.OrdinalIgnoreCase))
                        {
                            // Always replace "grpc" with "http" in the scheme because GrpcChannel needs http or https.
                            originalScheme = originalScheme.Replace("grpc", "http", StringComparison.OrdinalIgnoreCase);
                        }

                        PortUtils.TryExtract(addresses[i], out _, out _, out _, out var realHost, out var realPort);

                        var replacedHost = ReplaceHostWithLocalhost(realHost!);
                        var newUrl = $"{originalScheme}://{replacedHost}:{realPort}";

                        Urls.Add(newUrl);
                        Ports.Add(realPort);
                    }
                }

                IsStarted = true;
            });

#if NET10_0
            _logger.Info("Server using .NET 10.0");
#elif NET8_0
            _logger.Info("Server using .NET 8.0");
#else
            _logger.Info("Server using .NET Standard 2.0");
#endif

#if NET10_0
            return Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(_host, token);
#else
            return _host.RunAsync(token);
#endif
        }
        catch (Exception ex)
        {
            RunningException = ex;
            _logger.Error("Error while RunAsync", ex);

            IsStarted = false;

            return Task.CompletedTask;
        }
    }

    public Task StopAsync()
    {
        _cts.Cancel();

        IsStarted = false;
        return _host.StopAsync();
    }

    private static string ReplaceHostWithLocalhost(string host)
    {
        return host.Replace("0.0.0.0", "localhost").Replace("[::]", "localhost");
    }
}