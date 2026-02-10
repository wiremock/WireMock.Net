// Copyright © WireMock.Net

using JetBrains.Annotations;
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
    /// Send a specific text message in response to any received message
    /// </summary>
    /// <param name="text">The text message to send</param>
    [PublicAPI]
    IWebSocketBuilder WithText(string text);

    /// <summary>
    /// Send specific binary data in response to any received message
    /// </summary>
    /// <param name="bytes">The binary data to send</param>
    [PublicAPI]
    IWebSocketBuilder WithBytes(byte[] bytes);

    /// <summary>
    /// Send a JSON object in response to any received message
    /// </summary>
    /// <param name="data">The object to serialize and send as JSON</param>
    [PublicAPI]
    IWebSocketBuilder WithJson(object data);

    /// <summary>
    /// Handle incoming WebSocket messages
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithMessageHandler(Func<WebSocketMessage, IWebSocketContext, Task> handler);

    /// <summary>
    /// Define a sequence of messages to send
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithMessageSequence(Action<IWebSocketMessageSequenceBuilder> configure);

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

    /// <summary>
    /// Enable transformer support (Handlebars/Scriban)
    /// </summary>
    [PublicAPI]
    IWebSocketBuilder WithTransformer(
        TransformerType transformerType = TransformerType.Handlebars,
        bool useTransformerForBodyAsFile = false,
        ReplaceNodeOptions transformerReplaceNodeOptions = ReplaceNodeOptions.EvaluateAndTryToConvert);
}