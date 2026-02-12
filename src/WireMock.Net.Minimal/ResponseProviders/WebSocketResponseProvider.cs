// Copyright © WireMock.Net

using System.Buffers;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Owin;
using WireMock.Settings;
using WireMock.WebSockets;

namespace WireMock.ResponseProviders;

internal class WebSocketResponseProvider(WebSocketBuilder builder) : IResponseProvider
{
    private readonly WebSocketBuilder _builder = Guard.NotNull(builder);

    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(
        IMapping mapping,
        HttpContext context,
        IRequestMessage requestMessage,
        WireMockServerSettings settings)
    {
        // Check if this is a WebSocket upgrade request
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return (ResponseMessageBuilder.Create(HttpStatusCode.BadRequest, "Bad Request: Not a WebSocket upgrade request"), null);
        }

        try
        {
            // Accept the WebSocket connection
#if NET8_0_OR_GREATER
            var acceptContext = new WebSocketAcceptContext
            {
                SubProtocol = _builder.AcceptProtocol,
                KeepAliveInterval = _builder.KeepAliveIntervalSeconds ?? TimeSpan.FromSeconds(WebSocketConstants.DefaultKeepAliveIntervalSeconds)

            };
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(acceptContext).ConfigureAwait(false);
#else
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(_builder.AcceptProtocol).ConfigureAwait(false);
#endif

            // Get options from HttpContext.Items (set by WireMockMiddleware)
            if (!context.Items.TryGetValue(nameof(WireMockMiddlewareOptions), out var optionsObj) ||
                optionsObj is not IWireMockMiddlewareOptions options)
            {
                throw new InvalidOperationException("WireMockMiddlewareOptions not found in HttpContext.Items");
            }

            // Get or create registry from options
            var registry = _builder.IsBroadcast
                ? options.WebSocketRegistries.GetOrAdd(mapping.Guid, _ => new WebSocketConnectionRegistry())
                : null;

            // Create WebSocket context
            var wsContext = new WireMockWebSocketContext(
                context,
                webSocket,
                requestMessage,
                mapping,
                registry,
                _builder
            );

            // Update scenario state following the same pattern as WireMockMiddleware
            if (mapping.Scenario != null)
            {
                wsContext.UpdateScenarioState();
            }

            // Add to registry if broadcast is enabled
            if (registry != null)
            {
                registry.AddConnection(wsContext);
            }

            try
            {
                // Handle the WebSocket based on configuration
                if (_builder.ProxySettings != null)
                {
                    await HandleProxyAsync(wsContext, _builder.ProxySettings).ConfigureAwait(false);
                }
                else if (_builder.IsEcho)
                {
                    await HandleEchoAsync(wsContext).ConfigureAwait(false);
                }
                else if (_builder.MessageHandler != null)
                {
                    await HandleCustomAsync(wsContext, _builder.MessageHandler).ConfigureAwait(false);
                }
                else
                {
                    // Default: keep connection open until client closes
                    await WaitForCloseAsync(wsContext).ConfigureAwait(false);
                }
            }
            finally
            {
                // Remove from registry
                registry?.RemoveConnection(wsContext.ConnectionId);
            }

            // Return special marker to indicate WebSocket was handled
            return (new WebSocketHandledResponse(), null);
        }
        catch (Exception ex)
        {
            settings.Logger?.Error($"WebSocket error for mapping '{mapping.Guid}': {ex.Message}", ex);

            // If we haven't upgraded yet, we can return HTTP error
            if (!context.Response.HasStarted)
            {
                return (ResponseMessageBuilder.Create(HttpStatusCode.InternalServerError, $"WebSocket error: {ex.Message}"), null);
            }

            // Already upgraded - return marker
            return (new WebSocketHandledResponse(), null);
        }
    }

    private static async Task HandleEchoAsync(WireMockWebSocketContext context)
    {
        var bufferSize = context.Builder.MaxMessageSize ?? WebSocketConstants.DefaultReceiveBufferSize;
        using var buffer = ArrayPool<byte>.Shared.Lease(bufferSize);
        var timeout = context.Builder.CloseTimeout ?? TimeSpan.FromMinutes(WebSocketConstants.DefaultCloseTimeoutMinutes);
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            while (context.WebSocket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                var result = await context.WebSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cts.Token
                ).ConfigureAwait(false);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await context.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed by client"
                    ).ConfigureAwait(false);
                    break;
                }

                // Echo back
                await context.WebSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, result.Count),
                    result.MessageType,
                    result.EndOfMessage,
                    cts.Token
                ).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            if (context.WebSocket.State == WebSocketState.Open)
            {
                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Timeout");
            }
        }
    }

    private static async Task HandleCustomAsync(
        WireMockWebSocketContext context,
        Func<WebSocketMessage, IWebSocketContext, Task> handler)
    {
        var bufferSize = context.Builder.MaxMessageSize ?? WebSocketConstants.DefaultReceiveBufferSize;
        var buffer = new byte[bufferSize];
        var timeout = context.Builder.CloseTimeout ?? TimeSpan.FromMinutes(WebSocketConstants.DefaultCloseTimeoutMinutes);
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            while (context.WebSocket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                var result = await context.WebSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cts.Token
                ).ConfigureAwait(false);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await context.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed by client"
                    ).ConfigureAwait(false);
                    break;
                }

                var message = CreateWebSocketMessage(result, buffer);

                // Call custom handler
                await handler(message, context).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            if (context.WebSocket.State == WebSocketState.Open)
            {
                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Timeout");
            }
        }
    }

    private static async Task HandleProxyAsync(WireMockWebSocketContext context, ProxyAndRecordSettings settings)
    {
        using var clientWebSocket = new ClientWebSocket();

        var targetUri = new Uri(settings.Url);
        await clientWebSocket.ConnectAsync(targetUri, CancellationToken.None).ConfigureAwait(false);

        // Bidirectional proxy
        var clientToServer = ForwardMessagesAsync(context.WebSocket, clientWebSocket);
        var serverToClient = ForwardMessagesAsync(clientWebSocket, context.WebSocket);

        await Task.WhenAny(clientToServer, serverToClient).ConfigureAwait(false);

        // Close both
        if (context.WebSocket.State == WebSocketState.Open)
        {
            await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Proxy closed");
        }

        if (clientWebSocket.State == WebSocketState.Open)
        {
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Proxy closed", CancellationToken.None);
        }
    }

    private static async Task ForwardMessagesAsync(WebSocket source, WebSocket destination)
    {
        var buffer = new byte[WebSocketConstants.ProxyForwardBufferSize];

        while (source.State == WebSocketState.Open && destination.State == WebSocketState.Open)
        {
            var result = await source.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await destination.CloseAsync(
                    result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    result.CloseStatusDescription,
                    CancellationToken.None
                );
                break;
            }

            await destination.SendAsync(
                new ArraySegment<byte>(buffer, 0, result.Count),
                result.MessageType,
                result.EndOfMessage,
                CancellationToken.None
            );
        }
    }

    private static async Task WaitForCloseAsync(WireMockWebSocketContext context)
    {
        var buffer = new byte[WebSocketConstants.MinimumBufferSize];
        var timeout = context.Builder.CloseTimeout ?? TimeSpan.FromMinutes(WebSocketConstants.DefaultCloseTimeoutMinutes);
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            while (context.WebSocket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                var result = await context.WebSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cts.Token
                );

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client");
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            if (context.WebSocket.State == WebSocketState.Open)
            {
                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Timeout");
            }
        }
    }

    private static WebSocketMessage CreateWebSocketMessage(WebSocketReceiveResult result, byte[] buffer)
    {
        var message = new WebSocketMessage
        {
            MessageType = result.MessageType,
            EndOfMessage = result.EndOfMessage,
            Timestamp = DateTime.UtcNow
        };

        if (result.MessageType == WebSocketMessageType.Text)
        {
            message.Text = Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
        else
        {
            message.Bytes = new byte[result.Count];
            Array.Copy(buffer, message.Bytes, result.Count);
        }

        return message;
    }
}