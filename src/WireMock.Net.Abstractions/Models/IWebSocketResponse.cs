// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Types;

namespace WireMock.Models;

/// <summary>
/// Represents the WebSocket response to send to clients
/// </summary>
public interface IWebSocketResponse
{
    /// <summary>
    /// Gets the collection of messages to send in order
    /// </summary>
    IReadOnlyList<IWebSocketMessage> Messages { get; }

    /// <summary>
    /// Gets a value indicating whether to apply transformers (e.g., Handlebars, Scriban)
    /// </summary>
    bool UseTransformer { get; }

    /// <summary>
    /// Gets the transformer type to use if UseTransformer is true
    /// </summary>
    TransformerType? TransformerType { get; }

    /// <summary>
    /// Gets the close code (e.g., 1000 for normal closure)
    /// </summary>
    int? CloseCode { get; }

    /// <summary>
    /// Gets the close message
    /// </summary>
    string? CloseMessage { get; }

    /// <summary>
    /// Gets the subprotocol to negotiate with client
    /// </summary>
    string? Subprotocol { get; }

    /// <summary>
    /// Gets the delay in milliseconds before automatically closing the connection after all messages
    /// </summary>
    int? AutoCloseDelayMs { get; }
}
