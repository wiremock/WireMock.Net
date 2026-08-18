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
/// BodyData
/// </summary>
public class BodyData : IBodyData
{
    /// <inheritdoc />
    public Encoding? Encoding { get; set; }

    /// <inheritdoc />
    public string? BodyAsString { get; set; }

    /// <inheritdoc />
    public IDictionary<string, WireMockList<string>>? BodyAsFormUrlEncoded { get; set; }

    /// <inheritdoc />
    public object? BodyAsJson { get; set; }

    /// <inheritdoc />
    public byte[]? BodyAsBytes { get; set; }
    
    /// <inheritdoc />
    public bool? BodyAsJsonIndented { get; set; }

    /// <inheritdoc />
    public string? BodyAsFile { get; set; }

    /// <inheritdoc />
    public bool? BodyAsFileIsCached { get; set; }

    /// <inheritdoc />
    public BodyType? DetectedBodyType { get; set; }

    /// <inheritdoc />
    public BodyType? DetectedBodyTypeFromContentType { get; set; }

    /// <inheritdoc />
    public string? DetectedCompression { get; set; }

    /// <inheritdoc />
    public string? IsFuncUsed { get; set; }

    #region ProtoBuf
    /// <inheritdoc />
    public Func<IdOrTexts>? ProtoDefinition { get; set; }

    /// <inheritdoc />
    public string? ProtoBufMessageType { get; set; }
    #endregion

    /// <inheritdoc />
    public IBlockingQueue<string?>? SseStringQueue { get; set; }

    /// <inheritdoc />
    public Task? BodyAsSseStringTask { get; set; }
}