// Copyright © WireMock.Net

using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using WireMock.Constants;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Owin.ActivityTracing;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using WireMock.WebSockets;

namespace WireMock.ResponseProviders;

internal class WebSocketResponseProvider(WebSocketBuilder builder) : IResponseProvider
{
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
                SubProtocol = builder.AcceptProtocol,
                KeepAliveInterval = builder.KeepAliveIntervalSeconds ?? TimeSpan.FromSeconds(WebSocketConstants.DefaultKeepAliveIntervalSeconds)

            };
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(acceptContext).ConfigureAwait(false);
#else
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(builder.AcceptProtocol).ConfigureAwait(false);
#endif

            // Get options from HttpContext.Items (set by WireMockMiddleware)
            if (!context.Items.TryGetValue(nameof(IWireMockMiddlewareOptions), out var optionsObj) ||
                optionsObj is not IWireMockMiddlewareOptions options)
            {
                throw new InvalidOperationException("IWireMockMiddlewareOptions not found in HttpContext.Items");
            }

            // Get logger from HttpContext.Items
            if (!context.Items.TryGetValue(nameof(IWireMockMiddlewareLogger), out var loggerObj) ||
                loggerObj is not IWireMockMiddlewareLogger logger)
            {
                throw new InvalidOperationException("IWireMockMiddlewareLogger not found in HttpContext.Items");
            }

            // Get or create registry from options
            var registry = builder.IsBroadcast
                ? options.WebSocketRegistries.GetOrAdd(mapping.Guid, _ => new WebSocketConnectionRegistry())
                : null;

            // Create WebSocket context
            var wsContext = new WireMockWebSocketContext(
                context,
                webSocket,
                requestMessage,
                mapping,
                registry,
                builder,
                options,
                logger
            );

            // Update scenario state following the same pattern as WireMockMiddleware
            if (mapping.Scenario != null)
            {
                wsContext.UpdateScenarioState();
            }

            // Add to registry if broadcast is enabled
            registry?.AddConnection(wsContext);

            try
            {
                // Handle the WebSocket based on configuration
                if (builder.ProxySettings != null)
                {
                    await HandleProxyAsync(wsContext, builder.ProxySettings).ConfigureAwait(false);
                }
                else if (builder.IsEcho)
                {
                    await HandleEchoAsync(wsContext).ConfigureAwait(false);
                }
                else if (builder.MessageHandler != null)
                {
                    await HandleCustomAsync(wsContext, builder.MessageHandler).ConfigureAwait(false);
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

        var shouldTrace = context.Options?.ActivityTracingOptions is not null;

        try
        {
            while (context.WebSocket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                Activity? receiveActivity = null;
                if (shouldTrace)
                {
                    receiveActivity = WireMockActivitySource.StartWebSocketMessageActivity(WebSocketMessageDirection.Receive, context.Mapping.Guid);
                }

                try
                {
                    var result = await context.WebSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cts.Token
                    ).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        if (shouldTrace)
                        {
                            WireMockActivitySource.EnrichWithWebSocketMessage(
                                receiveActivity,
                                result.MessageType,
                                result.Count,
                                result.EndOfMessage,
                                null,
                                context.Options?.ActivityTracingOptions
                            );
                        }

                        LogWebSocketMessage(context, WebSocketMessageDirection.Receive, result.MessageType, null, receiveActivity);

                        await context.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closed by client"
                        ).ConfigureAwait(false);
                        break;
                    }

                    // Enrich activity with message details
                    string? textContent = null;
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        textContent = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }

                    if (shouldTrace)
                    {
                        WireMockActivitySource.EnrichWithWebSocketMessage(
                            receiveActivity,
                            result.MessageType,
                            result.Count,
                            result.EndOfMessage,
                            textContent,
                            context.Options?.ActivityTracingOptions
                        );
                    }

                    // Log the receive operation
                    LogWebSocketMessage(context, WebSocketMessageDirection.Receive, result.MessageType, textContent, receiveActivity);

                    // Echo back (this will be logged by context.SendAsync)
                    await context.WebSocket.SendAsync(
                        new ArraySegment<byte>(buffer, 0, result.Count),
                        result.MessageType,
                        result.EndOfMessage,
                        cts.Token
                    ).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    WireMockActivitySource.RecordException(receiveActivity, ex);
                    throw;
                }
                finally
                {
                    receiveActivity?.Dispose();
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

    private static async Task HandleCustomAsync(
        WireMockWebSocketContext context,
        Func<WebSocketMessage, IWebSocketContext, Task> handler)
    {
        var bufferSize = context.Builder.MaxMessageSize ?? WebSocketConstants.DefaultReceiveBufferSize;
        var buffer = new byte[bufferSize];
        var timeout = context.Builder.CloseTimeout ?? TimeSpan.FromMinutes(WebSocketConstants.DefaultCloseTimeoutMinutes);
        using var cts = new CancellationTokenSource(timeout);

        var shouldTrace = context.Options?.ActivityTracingOptions is not null;

        try
        {
            while (context.WebSocket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                Activity? receiveActivity = null;
                if (shouldTrace)
                {
                    receiveActivity = WireMockActivitySource.StartWebSocketMessageActivity(WebSocketMessageDirection.Receive, context.Mapping.Guid);
                }

                try
                {
                    var result = await context.WebSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cts.Token
                    ).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        if (shouldTrace)
                        {
                            WireMockActivitySource.EnrichWithWebSocketMessage(
                                receiveActivity,
                                result.MessageType,
                                result.Count,
                                result.EndOfMessage,
                                null,
                                context.Options?.ActivityTracingOptions
                            );
                        }

                        LogWebSocketMessage(context, WebSocketMessageDirection.Receive, result.MessageType, null, receiveActivity);

                        await context.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closed by client"
                        ).ConfigureAwait(false);
                        break;
                    }

                    var message = CreateWebSocketMessage(result, buffer);

                    // Enrich activity with message details
                    if (shouldTrace)
                    {
                        WireMockActivitySource.EnrichWithWebSocketMessage(
                            receiveActivity,
                            result.MessageType,
                            result.Count,
                            result.EndOfMessage,
                            message.Text,
                            context.Options?.ActivityTracingOptions
                        );
                    }

                    // Log the receive operation
                    object? data = message.Text != null ? message.Text : message.Bytes;
                    LogWebSocketMessage(context, WebSocketMessageDirection.Receive, result.MessageType, data, receiveActivity);

                    // Call custom handler
                    await handler(message, context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    WireMockActivitySource.RecordException(receiveActivity, ex);
                    throw;
                }
                finally
                {
                    receiveActivity?.Dispose();
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

    private static async Task HandleProxyAsync(WireMockWebSocketContext context, ProxyAndRecordSettings settings)
    {
        using var clientWebSocket = new ClientWebSocket();

        var targetUri = new Uri(settings.Url);
        await clientWebSocket.ConnectAsync(targetUri, CancellationToken.None).ConfigureAwait(false);

        // Bidirectional proxy
        var clientToServer = ForwardMessagesAsync(context, clientWebSocket, WebSocketMessageDirection.Receive);
        var serverToClient = ForwardMessagesAsync(context, clientWebSocket, WebSocketMessageDirection.Send);

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

    private static async Task ForwardMessagesAsync(
        WireMockWebSocketContext context,
        ClientWebSocket clientWebSocket,
        WebSocketMessageDirection direction)
    {
        var buffer = new byte[WebSocketConstants.ProxyForwardBufferSize];

        // Get options for activity tracing
        var options = context.HttpContext.Items.TryGetValue(nameof(WireMockMiddlewareOptions), out var optionsObj) &&
                      optionsObj is IWireMockMiddlewareOptions wireMockOptions
            ? wireMockOptions
            : null;

        var shouldTrace = options?.ActivityTracingOptions is not null;

        var source = direction == WebSocketMessageDirection.Receive ? context.WebSocket : (WebSocket)clientWebSocket;
        var destination = direction == WebSocketMessageDirection.Receive ? (WebSocket)clientWebSocket : context.WebSocket;

        while (source.State == WebSocketState.Open && destination.State == WebSocketState.Open)
        {
            Activity? activity = null;
            if (shouldTrace)
            {
                activity = WireMockActivitySource.StartWebSocketMessageActivity(direction, context.Mapping.Guid);
            }

            try
            {
                var result = await source.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    if (shouldTrace)
                    {
                        WireMockActivitySource.EnrichWithWebSocketMessage(
                            activity,
                            result.MessageType,
                            result.Count,
                            result.EndOfMessage,
                            null,
                            options?.ActivityTracingOptions
                        );
                    }

                    LogWebSocketMessage(context, direction, result.MessageType, null, activity);

                    await destination.CloseAsync(
                        result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                        result.CloseStatusDescription,
                        CancellationToken.None
                    );
                    break;
                }

                // Enrich activity with message details
                object? data = null;
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    data = new byte[result.Count];
                    Array.Copy(buffer, (byte[])data, result.Count);
                }

                if (shouldTrace)
                {
                    WireMockActivitySource.EnrichWithWebSocketMessage(
                        activity,
                        result.MessageType,
                        result.Count,
                        result.EndOfMessage,
                        data as string,
                        options?.ActivityTracingOptions
                    );
                }

                // Log the proxy operation
                LogWebSocketMessage(context, direction, result.MessageType, data, activity);

                await destination.SendAsync(
                    new ArraySegment<byte>(buffer, 0, result.Count),
                    result.MessageType,
                    result.EndOfMessage,
                    CancellationToken.None
                );
            }
            catch (Exception ex)
            {
                WireMockActivitySource.RecordException(activity, ex);
                throw;
            }
            finally
            {
                activity?.Dispose();
            }
        }
    }

    private static async Task WaitForCloseAsync(WireMockWebSocketContext context)
    {
        var buffer = new byte[WebSocketConstants.MinimumBufferSize];
        var timeout = context.Builder.CloseTimeout ?? TimeSpan.FromMinutes(WebSocketConstants.DefaultCloseTimeoutMinutes);
        using var cts = new CancellationTokenSource(timeout);

        var shouldTrace = context.Options?.ActivityTracingOptions is not null;

        try
        {
            while (context.WebSocket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                Activity? receiveActivity = null;
                if (shouldTrace)
                {
                    receiveActivity = WireMockActivitySource.StartWebSocketMessageActivity(WebSocketMessageDirection.Receive, context.Mapping.Guid);
                }

                try
                {
                    var result = await context.WebSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cts.Token
                    );

                    if (shouldTrace)
                    {
                        WireMockActivitySource.EnrichWithWebSocketMessage(
                            receiveActivity,
                            result.MessageType,
                            result.Count,
                            result.EndOfMessage,
                            null,
                            context.Options?.ActivityTracingOptions
                        );
                    }

                    // Log the receive operation
                    object? data = null;
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        data = new byte[result.Count];
                        Array.Copy(buffer, (byte[])data, result.Count);
                    }

                    LogWebSocketMessage(context, WebSocketMessageDirection.Receive, result.MessageType, data, receiveActivity);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    WireMockActivitySource.RecordException(receiveActivity, ex);
                    throw;
                }
                finally
                {
                    receiveActivity?.Dispose();
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

    private static void LogWebSocketMessage(
        WireMockWebSocketContext context,
        WebSocketMessageDirection direction,
        WebSocketMessageType messageType,
        object? data,
        Activity? activity)
    {
        // Skip logging if log count limit is disabled
        if (context.Options.MaxRequestLogCount == 0)
        {
            return;
        }

        // Create body data
        IBodyData bodyData;
        if (messageType == WebSocketMessageType.Text && data is string textContent)
        {
            bodyData = new BodyData
            {
                BodyAsString = textContent,
                DetectedBodyType = BodyType.String
            };
        }
        else if (messageType == WebSocketMessageType.Binary && data is byte[] binary)
        {
            bodyData = new BodyData
            {
                BodyAsBytes = binary,
                DetectedBodyType = BodyType.Bytes
            };
        }
        else
        {
            bodyData = new BodyData
            {
                BodyAsString = messageType.ToString(),
                DetectedBodyType = BodyType.String
            };
        }

        // Create a pseudo-request or pseudo-response depending on direction
        RequestMessage? requestMessage = null;
        IResponseMessage? responseMessage = null;

        var method = $"WS_{direction.ToString().ToUpperInvariant()}";

        if (direction == WebSocketMessageDirection.Receive)
        {
            // Received message - log as request
            requestMessage = new RequestMessage(
                new UrlDetails(context.RequestMessage.Url),
                method,
                context.RequestMessage.ClientIP,
                bodyData,
                null,
                null
            )
            {
                DateTime = DateTime.UtcNow
            };
        }
        else
        {
            // Sent message - log as response
            responseMessage = new ResponseMessage
            {
                Method = method,
                StatusCode = HttpStatusCode.SwitchingProtocols, // WebSocket status
                BodyData = bodyData,
                DateTime = DateTime.UtcNow
            };
        }

        // Create log entry
        var logEntry = new LogEntry
        {
            Guid = Guid.NewGuid(),
            RequestMessage = requestMessage,
            ResponseMessage = responseMessage,
            MappingGuid = context.Mapping.Guid,
            MappingTitle = context.Mapping.Title
        };

        // Enrich activity if present
        if (activity != null && context.Options.ActivityTracingOptions != null)
        {
            WireMockActivitySource.EnrichWithLogEntry(activity, logEntry, context.Options.ActivityTracingOptions);
        }

        // Log using LogLogEntry
        context.Logger.LogLogEntry(logEntry, context.Options.MaxRequestLogCount is null or > 0);

        activity?.Dispose();
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