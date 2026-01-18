// Copyright © WireMock.Net

using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WireMock.OpenTelemetry;

/// <summary>
/// Extension methods for configuring OpenTelemetry tracing for WireMock.Net.
/// </summary>
public static class WireMockOpenTelemetryExtensions
{
    private const string ServiceName = "WireMock.Net";
    private const string WireMockActivitySourceName = "WireMock.Net";

    /// <summary>
    /// Adds OpenTelemetry tracing to the WireMock server with instrumentation and OTLP exporter.
    /// This configures:
    /// - WireMock.Net ActivitySource instrumentation (custom WireMock traces with mapping details)
    /// - ASP.NET Core instrumentation (standard HTTP server traces)
    /// - OTLP exporter to send traces to a collector
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The OpenTelemetry options containing exporter configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWireMockOpenTelemetry(
        this IServiceCollection services,
        OpenTelemetryOptions? options)
    {
        if (options is null)
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: ServiceName,
                    serviceVersion: typeof(WireMockOpenTelemetryExtensions).Assembly.GetName().Version?.ToString() ?? "unknown"
                );
            })
            .WithTracing(tracing =>
            {
                // Add WireMock-specific traces
                tracing.AddSource(WireMockActivitySourceName);

                // Add ASP.NET Core instrumentation for standard HTTP server traces
                tracing.AddAspNetCoreInstrumentation(aspNetOptions =>
                {
                    // Filter out admin requests if configured
                    if (options.ExcludeAdminRequests)
                    {
                        aspNetOptions.Filter = context =>
                        {
                            var path = context.Request.Path.Value ?? string.Empty;
                            return !path.StartsWith("/__admin", StringComparison.OrdinalIgnoreCase);
                        };
                    }
                });

                // Add OTLP exporter - automatically reads OTEL_EXPORTER_OTLP_ENDPOINT from environment
                // If explicit endpoint is specified in options, use that instead
                var otlpEndpoint = options.OtlpExporterEndpoint;
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(exporterOptions =>
                    {
                        exporterOptions.Endpoint = new Uri(otlpEndpoint);
                    });
                }
                else
                {
                    // Use default - reads from OTEL_EXPORTER_OTLP_ENDPOINT env var
                    tracing.AddOtlpExporter();
                }
            });

        return services;
    }

    /// <summary>
    /// Configures OpenTelemetry tracing builder with WireMock.Net ActivitySource and ASP.NET Core instrumentation.
    /// Use this method when you want more control over the TracerProvider configuration.
    /// </summary>
    /// <param name="tracing">The TracerProviderBuilder to configure.</param>
    /// <param name="options">The OpenTelemetry options (optional).</param>
    /// <returns>The TracerProviderBuilder for chaining.</returns>
    public static TracerProviderBuilder AddWireMockInstrumentation(
        this TracerProviderBuilder tracing,
        OpenTelemetryOptions? options = null)
    {
        // Add WireMock-specific traces
        tracing.AddSource(WireMockActivitySourceName);

        // Add ASP.NET Core instrumentation for standard HTTP server traces
        tracing.AddAspNetCoreInstrumentation(aspNetOptions =>
        {
            // Filter out admin requests if configured
            if (options?.ExcludeAdminRequests == true)
            {
                aspNetOptions.Filter = context =>
                {
                    var path = context.Request.Path.Value ?? string.Empty;
                    return !path.StartsWith("/__admin", StringComparison.OrdinalIgnoreCase);
                };
            }
        });

        return tracing;
    }
}
