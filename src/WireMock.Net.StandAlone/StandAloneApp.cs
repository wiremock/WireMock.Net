// Copyright © WireMock.Net

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Exceptions;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;
#if OPENTELEMETRY_SUPPORTED
using WireMock.OpenTelemetry;
#endif

namespace WireMock.Net.StandAlone;

/// <summary>
/// The StandAloneApp
/// </summary>
public static class StandAloneApp
{
    private static readonly string Version = typeof(StandAloneApp).GetTypeInfo().Assembly.GetName().Version!.ToString();

    /// <summary>
    /// Start WireMock.Net standalone Server based on the WireMockServerSettings.
    /// </summary>
    /// <param name="settings">The WireMockServerSettings</param>
    [PublicAPI]
    public static WireMockServer Start(WireMockServerSettings settings)
    {
        Guard.NotNull(settings);

        var server = WireMockServer.Start(settings);

        settings.Logger?.Info("Version [{0}]", Version);
        settings.Logger?.Info("Server listening at {0}", string.Join(",", server.Urls));

        return server;
    }

#if OPENTELEMETRY_SUPPORTED
    /// <summary>
    /// Start WireMock.Net standalone Server based on the WireMockServerSettings with OpenTelemetry tracing.
    /// </summary>
    /// <param name="settings">The WireMockServerSettings</param>
    /// <param name="openTelemetryOptions">The OpenTelemetry options for exporting traces.</param>
    [PublicAPI]
    public static WireMockServer Start(WireMockServerSettings settings, OpenTelemetryOptions? openTelemetryOptions)
    {
        Guard.NotNull(settings);

        // Wire up OpenTelemetry OTLP exporter if options are provided
        if (openTelemetryOptions is not null)
        {
            // Enable activity tracing in settings so middleware creates activities
            // Only set ExcludeAdminRequests if not already configured
            settings.ActivityTracingOptions ??= new ActivityTracingOptions
            {
                ExcludeAdminRequests = openTelemetryOptions.ExcludeAdminRequests
            };

            var existingRegistration = settings.AdditionalServiceRegistration;
            settings.AdditionalServiceRegistration = services =>
            {
                existingRegistration?.Invoke(services);
                services.AddWireMockOpenTelemetry(openTelemetryOptions);
            };
        }

        return Start(settings);
    }
#endif

    /// <summary>
    /// Start WireMock.Net standalone Server based on the commandline arguments.
    /// </summary>
    /// <param name="args">The commandline arguments</param>
    /// <param name="logger">The logger</param>
    [PublicAPI]
    public static WireMockServer Start(string[] args, IWireMockLogger? logger = null)
    {
        Guard.NotNull(args);

        if (TryStart(args, out var server, logger))
        {
            return server;
        }

        throw new WireMockException($"Unable start start {nameof(WireMockServer)}.");
    }

    /// <summary>
    /// Try to start WireMock.Net standalone Server based on the commandline arguments.
    /// </summary>
    /// <param name="args">The commandline arguments</param>
    /// <param name="logger">The logger</param>
    /// <param name="server">The WireMockServer</param>
    [PublicAPI]
    public static bool TryStart(string[] args, [NotNullWhen(true)] out WireMockServer? server, IWireMockLogger? logger = null)
    {
        Guard.NotNull(args);

        if (WireMockServerSettingsParser.TryParseArguments(args, Environment.GetEnvironmentVariables(), out var settings, logger))
        {
            settings.Logger?.Info("Version [{0}]", Version);
            settings.Logger?.Debug("Server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

#if OPENTELEMETRY_SUPPORTED
            // Parse OpenTelemetry options separately using the OTEL project parser
            OpenTelemetryOptionsParser.TryParseArguments(args, Environment.GetEnvironmentVariables(), out var openTelemetryOptions);
            server = Start(settings, openTelemetryOptions);
#else
            server = Start(settings);
#endif
            return true;
        }

        server = null;
        return false;
    }
}