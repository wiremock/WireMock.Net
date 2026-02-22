// Copyright © WireMock.Net

using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using WireMock.Logging;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Owin.ActivityTracing;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket context implementation
/// </summary>
public class WireMockWebSocketContext : IWebSocketContext
{
    /// <inheritdoc />
    public Guid ConnectionId { get; }

    /// <inheritdoc />
    public HttpContext HttpContext { get; }

    /// <inheritdoc />
    public WebSocket WebSocket { get; }

    /// <inheritdoc />
    public IRequestMessage RequestMessage { get; }

    /// <inheritdoc />
    public IMapping Mapping { get; }

    internal WebSocketConnectionRegistry Registry { get; }

    internal WebSocketBuilder Builder { get; }

    internal IWireMockMiddlewareOptions Options { get; }

    internal IWireMockMiddlewareLogger Logger { get; }

    /// <summary>
    /// Creates a new WebSocketContext
    /// </summary>
    internal WireMockWebSocketContext(
        HttpContext httpContext,
        WebSocket webSocket,
        IRequestMessage requestMessage,
        IMapping mapping,
        WebSocketConnectionRegistry registry,
        WebSocketBuilder builder,
        IWireMockMiddlewareOptions options,
        IWireMockMiddlewareLogger logger,
        IGuidUtils guidUtils
    )
    {
        HttpContext = httpContext;
        WebSocket = webSocket;
        RequestMessage = requestMessage;
        Mapping = mapping;
        Registry = registry;
        Builder = builder;
        Options = options;
        Logger = logger;

        ConnectionId = guidUtils.NewGuid();
    }

    /// <inheritdoc />
    public Task SendAsync(string text, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        return SendAsyncInternal(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            text,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
    {
        return SendAsyncInternal(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Binary,
            true,
            bytes,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken = default)
    {
        await WebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken).ConfigureAwait(false);

        LogWebSocketMessage(WebSocketMessageDirection.Send, WebSocketMessageType.Close, $"CloseStatus: {closeStatus}, Description: {statusDescription}", null);
    }

    /// <inheritdoc />
    public void Abort(string? statusDescription = null)
    {
        WebSocket.Abort();

        LogWebSocketMessage(WebSocketMessageDirection.Send, WebSocketMessageType.Close, $"CloseStatus: Abort, Description: {statusDescription}", null);
    }

    /// <inheritdoc />
    public async Task BroadcastAsync(string text, bool excludeSender = false, CancellationToken cancellationToken = default)
    {
        Guid? excludeConnectionId = excludeSender ? ConnectionId : null;
        await Registry.BroadcastAsync(text, excludeConnectionId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task BroadcastAsync(byte[] bytes, bool excludeSender = false, CancellationToken cancellationToken = default)
    {
        Guid? excludeConnectionId = excludeSender ? ConnectionId : null;
        await Registry.BroadcastAsync(bytes, excludeConnectionId, cancellationToken);
    }

    internal void LogWebSocketMessage(
        WebSocketMessageDirection direction,
        WebSocketMessageType messageType,
        object? data,
        Activity? activity)
    {
        IBodyData bodyData;
        if (messageType == WebSocketMessageType.Text && data is string textContent)
        {
            bodyData = new BodyData
            {
                BodyAsString = textContent,
                DetectedBodyType = BodyType.WebSocketText
            };
        }
        else if (messageType == WebSocketMessageType.Binary && data is byte[] binary)
        {
            bodyData = new BodyData
            {
                BodyAsBytes = binary,
                DetectedBodyType = BodyType.WebSocketBinary
            };
        }
        else
        {
            bodyData = new BodyData
            {
                BodyAsString = messageType.ToString(),
                DetectedBodyType = BodyType.WebSocketClose
            };
        }

        var method = $"WS_{direction.ToString().ToUpperInvariant()}";

        RequestMessage? requestMessage = null;
        IResponseMessage? responseMessage = null;

        if (direction == WebSocketMessageDirection.Receive)
        {
            // Received message - log as request
            requestMessage = new RequestMessage(
                new UrlDetails(RequestMessage.Url),
                method,
                RequestMessage.ClientIP,
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
            MappingGuid = Mapping.Guid,
            MappingTitle = Mapping.Title
        };

        // Enrich activity if present
        if (activity != null && Options.ActivityTracingOptions != null)
        {
            WireMockActivitySource.EnrichWithLogEntry(activity, logEntry, Options.ActivityTracingOptions);
        }

        // Log using LogLogEntry
        Logger.LogLogEntry(logEntry, Options.MaxRequestLogCount is null or > 0);

        activity?.Dispose();
    }

    private async Task SendAsyncInternal(
        ArraySegment<byte> buffer,
        WebSocketMessageType messageType,
        bool endOfMessage,
        object? data,
        CancellationToken cancellationToken)
    {
        Activity? activity = null;
        var shouldTrace = Options.ActivityTracingOptions is not null;

        if (shouldTrace)
        {
            activity = WireMockActivitySource.StartWebSocketMessageActivity(WebSocketMessageDirection.Send, Mapping.Guid);
            WireMockActivitySource.EnrichWithWebSocketMessage(
                activity,
                messageType,
                buffer.Count,
                endOfMessage,
                data as string,
                Options.ActivityTracingOptions
            );
        }

        try
        {
            await WebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken).ConfigureAwait(false);

            // Log the send operation
            if (Options.MaxRequestLogCount is null or > 0)
            {
                LogWebSocketMessage(WebSocketMessageDirection.Send, messageType, data, activity);
            }
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