// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket Message Builder interface for building individual messages with optional delays
/// </summary>
public interface IWebSocketMessageBuilder
{
    /// <summary>
    /// Echo all received messages back to client
    /// </summary>
    [PublicAPI]
    IWebSocketMessageBuilder WithEcho();

    /// <summary>
    /// Send a specific text message
    /// </summary>
    /// <param name="text">The text message to send</param>
    [PublicAPI]
    IWebSocketMessageBuilder WithText(string text);

    /// <summary>
    /// Send specific binary data
    /// </summary>
    /// <param name="bytes">The binary data to send</param>
    [PublicAPI]
    IWebSocketMessageBuilder WithBinary(byte[] bytes);

    /// <summary>
    /// Set a delay before sending the message (using TimeSpan)
    /// </summary>
    /// <param name="delay">The delay before sending the message</param>
    [PublicAPI]
    IWebSocketMessageBuilder WithDelay(TimeSpan delay);

    /// <summary>
    /// Set a delay before sending the message (using milliseconds)
    /// </summary>
    /// <param name="delayInMilliseconds">The delay in milliseconds before sending the message</param>
    [PublicAPI]
    IWebSocketMessageBuilder WithDelay(int delayInMilliseconds);

    /// <summary>
    /// Close the WebSocket connection after this message
    /// </summary>
    [PublicAPI]
    IWebSocketMessageBuilder AndClose();

    /// <summary>
    /// Close the WebSocket connection.
    /// </summary>
    [PublicAPI]
    IWebSocketMessageBuilder Close();
}