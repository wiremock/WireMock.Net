// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.Admin.Mappings;
using WireMock.Types;

namespace WireMock.Admin.Requests;

/// <summary>
/// Response MessageModel
/// </summary>
public class LogResponseModel
{
    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    public object? StatusCode { get; set; }

    /// <summary>
    /// Gets the headers.
    /// </summary>
    public IDictionary<string, WireMockList<string>>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the body destination (SameAsSource, String or Bytes).
    /// </summary>
    public string? BodyDestination { get; set; }

    /// <summary>
    /// The body (as string).
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// The body (as JSON object).
    /// </summary>
    public object? BodyAsJson { get; set; }

    /// <summary>
    /// The body (as bytearray).
    /// </summary>
    public byte[]? BodyAsBytes { get; set; }

    /// <summary>
    /// Gets or sets the body as file.
    /// </summary>
    public string? BodyAsFile { get; set; }

    /// <summary>
    /// Is the body as file cached?
    /// </summary>
    public bool? BodyAsFileIsCached { get; set; }

    /// <summary>
    /// Gets or sets the original body.
    /// </summary>
    public string? BodyOriginal { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    public EncodingModel? BodyEncoding { get; set; }

    /// <summary>
    /// The detected body type (detection based on body content).
    /// </summary>
    public string? DetectedBodyType { get; set; }

    /// <summary>
    /// The detected body type (detection based on Content-Type).
    /// </summary>
    public string? DetectedBodyTypeFromContentType { get; set; }

    /// <summary>
    /// The FaultType.
    /// </summary>
    public string? FaultType { get; set; }

    /// <summary>
    /// Gets or sets the Fault percentage.
    /// </summary>
    public double? FaultPercentage { get; set; }
}