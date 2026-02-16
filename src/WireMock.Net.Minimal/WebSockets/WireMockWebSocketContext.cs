// Copyright © WireMock.Net

using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
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
    public Guid ConnectionId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public HttpContext HttpContext { get; }

    /// <inheritdoc />
    public WebSocket WebSocket { get; }

    /// <inheritdoc />
    public IRequestMessage RequestMessage { get; }

    /// <inheritdoc />
    public IMapping Mapping { get; }

    internal WebSocketConnectionRegistry? Registry { get; }

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
        WebSocketConnectionRegistry? registry,
        WebSocketBuilder builder,
        IWireMockMiddlewareOptions options,
        IWireMockMiddlewareLogger logger)
    {
        HttpContext = Guard.NotNull(httpContext);
        WebSocket = Guard.NotNull(webSocket);
        RequestMessage = Guard.NotNull(requestMessage);
        Mapping = Guard.NotNull(mapping);
        Registry = registry;
        Builder = Guard.NotNull(builder);
        Options = Guard.NotNull(options);
        Logger = Guard.NotNull(logger);
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

    private void LogWebSocketMessage(
        WebSocketMessageDirection direction,
        WebSocketMessageType messageType,
        object? data,
        Activity? activity)
    {
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
                DetectedBodyType = BodyType.Bytes
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
                StatusCode = HttpStatusCode.SwitchingProtocols, // WebSocket status
                BodyData = bodyData,
                DateTime = DateTime.UtcNow
            };
        }

        // Create a perfect match result
        var requestMatchResult = new RequestMatchResult();
        requestMatchResult.AddScore(typeof(WebSocketMessageDirection), MatchScores.Perfect, null);

        // Create log entry
        var logEntry = new LogEntry
        {
            Guid = Guid.NewGuid(),
            RequestMessage = requestMessage,
            ResponseMessage = responseMessage,
            MappingGuid = Mapping.Guid,
            MappingTitle = Mapping.Title,
            RequestMatchResult = requestMatchResult
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

    /// <inheritdoc />
    public async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription)
    {
        await WebSocket.CloseAsync(closeStatus, statusDescription, CancellationToken.None);

        LogWebSocketMessage(WebSocketMessageDirection.Send, WebSocketMessageType.Close, $"CloseStatus: {closeStatus}, Description: {statusDescription}", null);
    }

    /// <inheritdoc />
    public void SetScenarioState(string nextState)
    {
        SetScenarioState(nextState, null);
    }

    /// <inheritdoc />
    public void SetScenarioState(string nextState, string? description)
    {
        if (Mapping.Scenario == null)
        {
            return;
        }

        // Use the same logic as WireMockMiddleware
        if (Options.Scenarios.TryGetValue(Mapping.Scenario, out var scenarioState))
        {
            // Directly set the next state (bypass counter logic for manual WebSocket state changes)
            scenarioState.NextState = nextState;
            scenarioState.Started = true;
            scenarioState.Finished = nextState == null;

            // Reset counter when manually setting state
            scenarioState.Counter = 0;
        }
        else
        {
            // Create new scenario state if it doesn't exist
            Options.Scenarios.TryAdd(Mapping.Scenario, new ScenarioState
            {
                Name = Mapping.Scenario,
                NextState = nextState,
                Started = true,
                Finished = nextState == null,
                Counter = 0
            });
        }
    }

    /// <summary>
    /// Update scenario state following the same pattern as WireMockMiddleware.UpdateScenarioState
    /// This is called automatically when the WebSocket connection is established.
    /// </summary>
    internal void UpdateScenarioState()
    {
        if (Mapping.Scenario == null)
        {
            return;
        }

        // Ensure scenario exists
        if (!Options.Scenarios.TryGetValue(Mapping.Scenario, out var scenario))
        {
            return;
        }

        // Follow exact same logic as WireMockMiddleware.UpdateScenarioState
        // Increase the number of times this state has been executed
        scenario.Counter++;

        // Only if the number of times this state is executed equals the required StateTimes, 
        // proceed to next state and reset the counter to 0
        if (scenario.Counter == (Mapping.TimesInSameState ?? 1))
        {
            scenario.NextState = Mapping.NextState;
            scenario.Counter = 0;
        }

        // Else just update Started and Finished
        scenario.Started = true;
        scenario.Finished = Mapping.NextState == null;
    }

    /// <inheritdoc />
    public async Task BroadcastTextAsync(string text, CancellationToken cancellationToken = default)
    {
        if (Registry != null)
        {
            await Registry.BroadcastTextAsync(text, cancellationToken);
        }
    }
}