// Copyright © WireMock.Net

using System;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.ResponseBuilders;

/// <summary>
/// Implementation of IWebSocketMessage
/// </summary>
public class WebSocketMessage : IWebSocketMessage
{
    private string? _bodyAsString;
    private byte[]? _bodyAsBytes;

    /// <inheritdoc />
    public int DelayMs { get; set; }

    /// <inheritdoc />
    public string? BodyAsString
    {
        get => _bodyAsString;
        set
        {
            _bodyAsString = value;
            _bodyAsBytes = null;
        }
    }

    /// <inheritdoc />
    public byte[]? BodyAsBytes
    {
        get => _bodyAsBytes;
        set
        {
            _bodyAsBytes = value;
            _bodyAsString = null;
        }
    }

    /// <inheritdoc />
    public bool IsText => _bodyAsBytes == null;

    /// <inheritdoc />
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <inheritdoc />
    public string? CorrelationId { get; set; }
}
