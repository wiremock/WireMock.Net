// Copyright © WireMock.Net

#if ACTIVITY_TRACING_SUPPORTED

namespace WireMock.Owin.ActivityTracing;

/// <summary>
/// Options for controlling activity tracing in WireMock.Net middleware.
/// These options control the creation of System.Diagnostics.Activity objects
/// but do not require any OpenTelemetry exporter dependencies.
/// </summary>
public class ActivityTracingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to exclude admin interface requests from tracing.
    /// Default is <c>true</c>.
    /// </summary>
    public bool ExcludeAdminRequests { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to record request body in trace attributes.
    /// Default is <c>false</c> due to potential PII concerns.
    /// </summary>
    public bool RecordRequestBody { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to record response body in trace attributes.
    /// Default is <c>false</c> due to potential PII concerns.
    /// </summary>
    public bool RecordResponseBody { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to record mapping match details in trace attributes.
    /// Default is <c>true</c>.
    /// </summary>
    public bool RecordMatchDetails { get; set; } = true;
}
#endif
