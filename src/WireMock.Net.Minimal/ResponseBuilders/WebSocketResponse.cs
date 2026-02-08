// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using Stef.Validation;
using WireMock.Models;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// Implementation of IWebSocketResponse
/// </summary>
public class WebSocketResponse : IWebSocketResponse
{
    private readonly List<IWebSocketMessage> _messages = [];

    /// <inheritdoc />
    public IReadOnlyList<IWebSocketMessage> Messages => _messages.AsReadOnly();

    /// <inheritdoc />
    public bool UseTransformer { get; set; }

    /// <inheritdoc />
    public Types.TransformerType? TransformerType { get; set; }

    /// <inheritdoc />
    public int? CloseCode { get; set; }

    /// <inheritdoc />
    public string? CloseMessage { get; set; }

    /// <inheritdoc />
    public string? Subprotocol { get; set; }

    /// <inheritdoc />
    public int? AutoCloseDelayMs { get; set; }

    /// <summary>
    /// Add a message to the response
    /// </summary>
    internal void AddMessage(IWebSocketMessage message)
    {
        Guard.NotNull(message);
        _messages.Add(message);
    }
}
