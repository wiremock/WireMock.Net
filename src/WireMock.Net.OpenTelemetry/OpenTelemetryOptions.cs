// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.OpenTelemetry;

/// <summary>
/// OpenTelemetry exporter configuration options for WireMock.Net.
/// These options control the export of traces to an OTLP endpoint.
/// For controlling what data is recorded in traces, configure ActivityTracingOptions in WireMockServerSettings.
/// </summary>
public class OpenTelemetryOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to exclude admin interface requests from ASP.NET Core instrumentation.
    /// Default is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// This controls the ASP.NET Core HTTP server instrumentation filter.
    /// To also exclude admin requests from WireMock's own activity tracing,
    /// set <c>ActivityTracingOptions.ExcludeAdminRequests</c> in WireMockServerSettings.
    /// </remarks>
    [PublicAPI]
    public bool ExcludeAdminRequests { get; set; } = true;

    /// <summary>
    /// Gets or sets the OTLP exporter endpoint URL.
    /// When set, traces will be exported to this endpoint using the OTLP protocol.
    /// Example: "http://localhost:4317" for gRPC or "http://localhost:4318" for HTTP.
    /// If not set, the OTLP exporter will use the <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> environment variable,
    /// or fall back to the default endpoint (<c>http://localhost:4317</c> for gRPC).
    /// </summary>
    [PublicAPI]
    public string? OtlpExporterEndpoint { get; set; }
}
