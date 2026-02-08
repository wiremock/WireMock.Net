// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.ResponseProviders;
using WireMock.Settings;

namespace WireMock.WebSockets.ResponseProviders;

/// <summary>
/// Response provider for handling WebSocket connections.
/// </summary>
internal class WebSocketResponseProvider : IResponseProvider
{
    private readonly Func<WebSocketHandlerContext, Task>? _handler;
    private readonly Func<WebSocketMessage, Task<WebSocketMessage?>>? _messageHandler;
    private readonly TimeSpan? _keepAliveInterval;
    private readonly TimeSpan? _timeout;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketResponseProvider"/> class.
    /// </summary>
    /// <param name="handler">The WebSocket connection handler.</param>
    /// <param name="messageHandler">The message handler for message-based routing.</param>
    /// <param name="keepAliveInterval">The keep-alive interval.</param>
    /// <param name="timeout">The connection timeout.</param>
    public WebSocketResponseProvider(
        Func<WebSocketHandlerContext, Task>? handler = null,
        Func<WebSocketMessage, Task<WebSocketMessage?>>? messageHandler = null,
        TimeSpan? keepAliveInterval = null,
        TimeSpan? timeout = null)
    {
        _handler = handler;
        _messageHandler = messageHandler;
        _keepAliveInterval = keepAliveInterval ?? TimeSpan.FromSeconds(30);
        _timeout = timeout ?? TimeSpan.FromMinutes(5);
    }

    /// <inheritdoc/>
    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        // This provider is used in middleware context, not via normal HTTP response path
        // The actual WebSocket handling happens in HandleWebSocketAsync
        // For now, return null - the middleware will handle the WebSocket directly
        return (null!, null);
    }

    /// <summary>
    /// Handles the WebSocket connection.
    /// </summary>
    /// <param name="webSocket">The WebSocket instance.</param>
    /// <param name="requestMessage">The request message.</param>
    /// <param name="subProtocol">The negotiated subprotocol.</param>
    public async Task HandleWebSocketAsync(WebSocket webSocket, IRequestMessage requestMessage, string? subProtocol = null)
    {
        Guard.NotNull(webSocket);
        Guard.NotNull(requestMessage);

        var headers = requestMessage.Headers != null
            ? new Dictionary<string, string[]>(
                requestMessage.Headers.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToArray() ?? Array.Empty<string>()))
            : new Dictionary<string, string[]>();

        var context = new WebSocketHandlerContext
        {
            WebSocket = webSocket,
            RequestMessage = requestMessage,
            Headers = headers,
            SubProtocol = subProtocol
        };

        try
        {
            if (_handler != null)
            {
                await _handler(context).ConfigureAwait(false);
            }
            else if (_messageHandler != null)
            {
                await HandleMessagesAsync(webSocket, _messageHandler).ConfigureAwait(false);
            }
            else
            {
                // Default: echo handler
                await EchoAsync(webSocket).ConfigureAwait(false);
            }
        }
        catch (WebSocketException) when (webSocket.State == WebSocketState.Closed)
        {
            // Connection already closed, ignore
        }
        catch (OperationCanceledException)
        {
            // Timeout or cancellation, ignore
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.CloseSent)
            {
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).ConfigureAwait(false);
                }
                catch
                {
                    // Ignore errors when closing
                }
            }
        }
    }

    private async Task HandleMessagesAsync(WebSocket webSocket, Func<WebSocketMessage, Task<WebSocketMessage?>> messageHandler)
    {
        var buffer = new byte[1024 * 4];
        var timeoutMs = (int)(_timeout?.TotalMilliseconds ?? 300000);
        
        using (var cts = new CancellationTokenSource(timeoutMs))
        {
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cts.Token).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            result.CloseStatusDescription ?? string.Empty,
                            cts.Token).ConfigureAwait(false);
                        break;
                    }

                    // Parse incoming message
                    var incomingMessage = new WebSocketMessage
                    {
                        IsBinary = result.MessageType == WebSocketMessageType.Binary,
                        RawData = buffer.Take(result.Count).ToArray(),
                        TextData = result.MessageType == WebSocketMessageType.Text
                            ? Encoding.UTF8.GetString(buffer, 0, result.Count)
                            : null,
                        Timestamp = DateTime.UtcNow
                    };

                    // Handle the message
                    var responseMessage = await messageHandler(incomingMessage).ConfigureAwait(false);

                    // Send response if provided
                    if (responseMessage != null)
                    {
                        var responseData = responseMessage.IsBinary && responseMessage.RawData != null
                            ? responseMessage.RawData
                            : Encoding.UTF8.GetBytes(responseMessage.TextData ?? string.Empty);

                        await webSocket.SendAsync(
                            new ArraySegment<byte>(responseData),
                            responseMessage.IsBinary ? WebSocketMessageType.Binary : WebSocketMessageType.Text,
                            true,
                            cts.Token).ConfigureAwait(false);
                    }

                    // Reset timeout after each message
                    cts.CancelAfter(timeoutMs);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private static async Task EchoAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);

        while (result.MessageType != WebSocketMessageType.Close)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None).ConfigureAwait(false);
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
        }

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).ConfigureAwait(false);
    }
}
