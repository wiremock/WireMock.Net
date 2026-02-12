// Copyright © WireMock.Net

using JetBrains.Annotations;
using WireMock.Matchers;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket Response Builder interface
/// </summary>
public interface IWebSocketBuilder
{
    /// <summary>
    /// Accept the WebSocket with a specific protocol
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithAcceptProtocol(string protocol);

    /// <summary>
    /// Echo all received messages back to client
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithEcho();

    /// <summary>
    /// Configure and send a single message in response to any received message
    /// </summary>
    /// <param name="configure">Action to configure the message</param>
    [PublicAPI]
    IWebSocketBuilder SendMessage(Action<IWebSocketMessageBuilder> configure);

    /// <summary>
    /// Configure and send multiple messages in response to any received message
    /// </summary>
    /// <param name="configure">Action to configure the messages</param>
    [PublicAPI]
    IWebSocketBuilder SendMessages(Action<IWebSocketMessagesBuilder> configure);

    /// <summary>
    /// Configure message sending based on message content matching
    /// </summary>
    /// <param name="condition">String to match in message text</param>
    [PublicAPI]
    IWebSocketMessageConditionBuilder WhenMessage(string condition);

    /// <summary>
    /// Configure message sending based on message content matching
    /// </summary>
    /// <param name="condition">Bytes to match in message</param>
    [PublicAPI]
    IWebSocketMessageConditionBuilder WhenMessage(byte[] condition);

    /// <summary>
    /// Configure message sending based on IMatcher
    /// </summary>
    /// <param name="matcher">IMatcher to match the message</param>
    [PublicAPI]
    IWebSocketMessageConditionBuilder WhenMessage(IMatcher matcher);

    /// <summary>
    /// Handle incoming WebSocket messages
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithMessageHandler(Func<WebSocketMessage, IWebSocketContext, Task> handler);

    /// <summary>
    /// Enable broadcast mode for this mapping
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithBroadcast();

    /// <summary>
    /// Proxy to another WebSocket server
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithProxy(ProxyAndRecordSettings settings);

    /// <summary>
    /// Set close timeout (default: 10 minutes)
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithCloseTimeout(TimeSpan timeout);

    /// <summary>
    /// Set maximum message size in bytes (default: 1 MB)
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithMaxMessageSize(int sizeInBytes);

    /// <summary>
    /// Set receive buffer size (default: 4096 bytes)
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithReceiveBufferSize(int sizeInBytes);

    /// <summary>
    /// Set keep-alive interval (default: 30 seconds)
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithKeepAliveInterval(TimeSpan interval);
}