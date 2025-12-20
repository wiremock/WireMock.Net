// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.Types;

// ReSharper disable once CheckNamespace
namespace WireMock.Util;

/// <summary>
/// IBodyData
/// </summary>
public interface IBodyData
{
    /// <summary>
    /// The body (as byte array).
    /// </summary>
    byte[]? BodyAsBytes { get; set; }

    /// <summary>
    /// Gets or sets the body as a file.
    /// </summary>
    string? BodyAsFile { get; set; }

    /// <summary>
    /// Is the body as file cached?
    /// </summary>
    bool? BodyAsFileIsCached { get; set; }

    /// <summary>
    /// The body (as JSON object).
    /// Also used for ProtoBuf.
    /// </summary>
    object? BodyAsJson { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Json Body String needs to be indented.
    /// </summary>
    bool? BodyAsJsonIndented { get; set; }

    /// <summary>
    /// The body as string, this is defined when BodyAsString or BodyAsJson are not null.
    /// </summary>
    string? BodyAsString { get; set; }

    /// <summary>
    /// The body as Form UrlEncoded dictionary.
    /// </summary>
    IDictionary<string, string>? BodyAsFormUrlEncoded { get; set; }

    /// <summary>
    /// The detected body type (detection based on body content).
    /// </summary>
    BodyType? DetectedBodyType { get; set; }

    /// <summary>
    /// The detected body type (detection based on Content-Type).
    /// </summary>
    BodyType? DetectedBodyTypeFromContentType { get; set; }

    /// <summary>
    /// The detected compression.
    /// </summary>
    string? DetectedCompression { get; set; }

    /// <summary>
    /// The body encoding.
    /// </summary>
    Encoding? Encoding { get; set; }

    /// <summary>
    /// Defines if this BodyData is the result of a dynamically created response-string.
    /// </summary>
    public string? IsFuncUsed { get; set; }

    #region ProtoBuf
    /// <summary>
    /// Gets or sets the proto definition.
    /// </summary>
    public Func<IdOrTexts>? ProtoDefinition { get; set; }

    /// <summary>
    /// Gets or sets the full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".
    /// </summary>
    public string? ProtoBufMessageType { get; set; }
    #endregion

    /// <summary>
    /// Defines the queue to use for Server-Sent Events (string).
    /// </summary>
    public IBlockingQueue<string?>? SseStringQueue { get; set; }

    /// <summary>
    /// Defines if the body is using Server-Sent Events (string).
    /// </summary>
    public Task? BodyAsSseStringTask { get; set; }
}