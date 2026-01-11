// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.Settings;

/// <summary>
/// OpenTelemetry configuration options for WireMock.Net.
/// When this options object is provided to WireMockServerSettings, OpenTelemetry tracing is automatically enabled.
/// </summary>
public class OpenTelemetryOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to record request body in trace attributes.
    /// Default is <c>false</c> due to potential PII concerns.
    /// </summary>
    [PublicAPI]
    public bool RecordRequestBody { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to record response body in trace attributes.
    /// Default is <c>false</c> due to potential PII concerns.
    /// </summary>
    [PublicAPI]
    public bool RecordResponseBody { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to exclude admin interface requests from tracing.
    /// Default is <c>true</c>.
    /// </summary>
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
