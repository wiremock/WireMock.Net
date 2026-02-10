// Copyright © WireMock.Net

using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Owin;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket context implementation
/// </summary>
public class WireMockWebSocketContext : IWebSocketContext
{
    private readonly IWireMockMiddlewareOptions _options;

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

    /// <summary>
    /// Creates a new WebSocketContext
    /// </summary>
    internal WireMockWebSocketContext(
        HttpContext httpContext,
        WebSocket webSocket,
        IRequestMessage requestMessage,
        IMapping mapping,
        WebSocketConnectionRegistry? registry,
        WebSocketBuilder builder)
    {
        HttpContext = Guard.NotNull(httpContext);
        WebSocket = Guard.NotNull(webSocket);
        RequestMessage = Guard.NotNull(requestMessage);
        Mapping = Guard.NotNull(mapping);
        Registry = registry;
        Builder = Guard.NotNull(builder);

        // Get options from HttpContext
        if (httpContext.Items.TryGetValue(nameof(WireMockMiddlewareOptions), out var options))
        {
            _options = (IWireMockMiddlewareOptions)options!;
        }
        else
        {
            throw new InvalidOperationException("WireMockMiddlewareOptions not found in HttpContext.Items");
        }
    }

    /// <inheritdoc />
    public Task SendAsync(string text, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        return WebSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
    {
        return WebSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Binary,
            true,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task SendAsJsonAsync(object data, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(data);
        return SendAsync(json, cancellationToken);
    }

    /// <inheritdoc />
    public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription)
    {
        return WebSocket.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
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
        if (_options.Scenarios.TryGetValue(Mapping.Scenario, out var scenarioState))
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
            _options.Scenarios.TryAdd(Mapping.Scenario, new ScenarioState
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
        if (!_options.Scenarios.TryGetValue(Mapping.Scenario, out var scenario))
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

    /// <inheritdoc />
    public async Task BroadcastJsonAsync(object data, CancellationToken cancellationToken = default)
    {
        if (Registry != null)
        {
            await Registry.BroadcastJsonAsync(data, cancellationToken);
        }
    }
}