// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Models;
using WireMock.ResponseProviders;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The WebSocket Response Builder interface - allows chainable building of WebSocket responses.
/// Implements IResponseProvider to integrate with the response builder chain.
/// </summary>
public interface IWebSocketResponseBuilder : IResponseProvider
{
    /// <summary>
    /// Add a text message to the WebSocket response.
    /// </summary>
    /// <param name="message">The message text</param>
    /// <param name="delayMs">Delay in milliseconds before sending</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithMessage(string message, int delayMs = 0);

    /// <summary>
    /// Add a JSON message to the WebSocket response.
    /// </summary>
    /// <param name="message">The object to serialize as JSON</param>
    /// <param name="delayMs">Delay in milliseconds before sending</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithJsonMessage(object message, int delayMs = 0);

    /// <summary>
    /// Add a binary message to the WebSocket response.
    /// </summary>
    /// <param name="message">The binary data</param>
    /// <param name="delayMs">Delay in milliseconds before sending</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithBinaryMessage(byte[] message, int delayMs = 0);

    /// <summary>
    /// Enable template transformation (Handlebars/Scriban) for messages.
    /// </summary>
    /// <param name="transformerType">The transformer type to use (default: Handlebars)</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithTransformer(TransformerType transformerType = TransformerType.Handlebars);

    /// <summary>
    /// Set the close code and message for connection closure.
    /// </summary>
    /// <param name="code">The close code (e.g., 1000)</param>
    /// <param name="message">The close message</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithClose(int code, string? message = null);

    /// <summary>
    /// Set the subprotocol to negotiate with client.
    /// </summary>
    /// <param name="subprotocol">The subprotocol name</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithSubprotocol(string subprotocol);

    /// <summary>
    /// Set automatic connection closure after all messages are sent.
    /// </summary>
    /// <param name="delayMs">Delay in milliseconds after last message</param>
    /// <returns>The response builder for chaining</returns>
    [PublicAPI]
    IResponseBuilder WithAutoClose(int delayMs = 0);

    /// <summary>
    /// Get the built WebSocket response.
    /// </summary>
    IWebSocketResponse? Build();
}