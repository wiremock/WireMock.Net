// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket Messages Builder interface for building multiple messages
/// </summary>
public interface IWebSocketMessagesBuilder
{
    /// <summary>
    /// Add a message to the sequence
    /// </summary>
    /// <param name="configure">Action to configure the message</param>
    [PublicAPI]
    IWebSocketMessagesBuilder AddMessage(Action<IWebSocketMessageBuilder> configure);
}