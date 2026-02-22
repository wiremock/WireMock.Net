// Copyright © WireMock.Net

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Owin.Mappers;
using WireMock.Serialization;
using WireMock.Services;
using WireMock.Util;

namespace WireMock.Owin;

internal partial class AspNetCoreSelfHost
{
    private readonly CancellationTokenSource _cts = new();
    private readonly IWireMockMiddlewareOptions _wireMockMiddlewareOptions;
    private readonly IWireMockLogger _logger;
    private readonly HostUrlOptions _urlOptions;

    private IWebHost _host = null!;

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
                services.AddSingleton<IDateTimeUtils, DateTimeUtils>();
                services.AddSingleton<LogEntryMapper>();
                services.AddSingleton<IWireMockMiddlewareLogger, WireMockMiddlewareLogger>();

#if NET8_0_OR_GREATER
                AddCors(services);
#endif
                _wireMockMiddlewareOptions.AdditionalServiceRegistration?.Invoke(services);
            })
            .Configure(appBuilder =>
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
            })
            .UseKestrel(options =>
            {
                SetKestrelOptionsLimits(options);

                SetHttpsAndUrls(options, _wireMockMiddlewareOptions, _urlOptions.GetDetails());
            })
            .ConfigureKestrelServerOptions()
            .Build();

        return RunHost(_cts.Token);
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
                var addresses = _host.ServerFeatures
                    .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()!
                    .Addresses
                    .ToArray();

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

#if NET8_0
            _logger.Info("Server using .NET 8.0");
#else
            _logger.Info("Server using .NET Standard 2.0");
#endif

            return _host.RunAsync(token);
        }
        catch (Exception e)
        {
            RunningException = e;
            _logger.Error(e.ToString());

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