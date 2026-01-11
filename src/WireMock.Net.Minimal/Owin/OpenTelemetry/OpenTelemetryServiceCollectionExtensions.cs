// Copyright © WireMock.Net

#if OPENTELEMETRY_SUPPORTED
using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WireMock.Settings;

namespace WireMock.Owin.OpenTelemetry;

/// <summary>
/// Extension methods for configuring OpenTelemetry in WireMock's ASP.NET Core host.
/// </summary>
internal static class OpenTelemetryServiceCollectionExtensions
{
    private const string ServiceName = "WireMock.Net";

    /// <summary>
    /// Adds OpenTelemetry tracing to the WireMock server if OpenTelemetryOptions is provided.
    /// This configures both the WireMock-specific ActivitySource and ASP.NET Core instrumentation.
    /// </summary>
    public static IServiceCollection AddWireMockOpenTelemetry(
        this IServiceCollection services, 
        IWireMockMiddlewareOptions options)
    {
        var otelOptions = options.OpenTelemetryOptions;
        if (otelOptions is null)
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: ServiceName,
                    serviceVersion: typeof(WireMockActivitySource).Assembly.GetName().Version?.ToString() ?? "unknown"
                );
            })
            .WithTracing(tracing =>
            {
                // Add WireMock-specific traces
                tracing.AddSource(WireMockActivitySource.SourceName);

                // Add ASP.NET Core instrumentation for standard HTTP server traces
                tracing.AddAspNetCoreInstrumentation(aspNetOptions =>
                {
                    // Filter out admin requests if configured
                    if (otelOptions.ExcludeAdminRequests == true)
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
                var otlpEndpoint = otelOptions.OtlpExporterEndpoint;
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
}
#else
using System;
using Microsoft.Extensions.DependencyInjection;

namespace WireMock.Owin.OpenTelemetry;

/// <summary>
/// Stub extension methods for platforms where OpenTelemetry is not supported.
/// </summary>
internal static class OpenTelemetryServiceCollectionExtensions
{
    /// <summary>
    /// Throws an exception if OpenTelemetryOptions is provided, as OpenTelemetry is not supported on this framework.
    /// </summary>
    public static IServiceCollection AddWireMockOpenTelemetry(
        this IServiceCollection services,
        IWireMockMiddlewareOptions options)
    {
        if (options.OpenTelemetryOptions is not null)
        {
            throw new InvalidOperationException("OpenTelemetry is not supported on this target framework. It requires .NET 5.0 or higher.");
        }

        return services;
    }
}
#endif
