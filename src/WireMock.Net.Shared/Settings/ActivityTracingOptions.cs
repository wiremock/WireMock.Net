// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.Settings;

/// <summary>
/// Options for controlling activity tracing in WireMock.Net.
/// These options control the creation of System.Diagnostics.Activity objects
/// but do not require any OpenTelemetry exporter dependencies.
/// </summary>
/// <remarks>
/// To export traces to an OpenTelemetry collector, install the WireMock.Net.OpenTelemetry package
/// and configure the exporter using the provided extension methods.
/// </remarks>
[PublicAPI]
public class ActivityTracingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to exclude admin interface requests from activity tracing.
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
