// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket Message Condition Builder interface for building conditional message responses
/// </summary>
public interface IWebSocketMessageConditionBuilder
{
    /// <summary>
    /// Configure and send a message when the condition matches
    /// </summary>
    /// <param name="configure">Action to configure the message</param>
    [PublicAPI]
    IWebSocketBuilder SendMessage(Action<IWebSocketMessageBuilder> configure);

    /// <summary>
    /// Configure and send multiple messages when the condition matches
    /// </summary>
    /// <param name="configure">Action to configure the messages</param>
    [PublicAPI]
    IWebSocketBuilder SendMessages(Action<IWebSocketMessagesBuilder> configure);
}
