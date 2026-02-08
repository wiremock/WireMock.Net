// Copyright © WireMock.Net

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Examples.WebSockets;

/// <summary>
/// Examples of using WebSocket support in WireMock.Net
/// </summary>
public static class WebSocketExamples
{
    /// <summary>
    /// Example 1: Simple echo WebSocket server
    /// </summary>
    public static async Task EchoWebSocketExampleAsync()
    {
        var server = WireMockServer.Start();

        // Set up a WebSocket that echoes messages back
        server
            .Given(Request.Create()
                .WithPath("/echo")
            )
            .RespondWith(Response.Create()
                .WithWebSocketHandler(async ctx =>
                {
                    using var webSocket = ctx.WebSocket;
                    var buffer = new byte[1024 * 4];

                    while (webSocket.State == WebSocketState.Open)
                    {
                        var result = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await webSocket.CloseAsync(
                                WebSocketCloseStatus.NormalClosure,
                                "Closing",
                                CancellationToken.None);
                        }
                        else
                        {
                            await webSocket.SendAsync(
                                new ArraySegment<byte>(buffer, 0, result.Count),
                                result.MessageType,
                                result.EndOfMessage,
                                CancellationToken.None);
                        }
                    }
                })
            );

        // Connect and test
        using var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri($"ws://localhost:{server.Port}/echo"), CancellationToken.None);

        var message = Encoding.UTF8.GetBytes("Hello WebSocket!");
        await client.SendAsync(
            new ArraySegment<byte>(message),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);

        var buffer = new byte[1024 * 4];
        var result = await client.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None);

        var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Received: {response}");

        server.Stop();
    }

    /// <summary>
    /// Example 2: Server-initiated messages (heartbeat/keep-alive)
    /// </summary>
    public static void HeartbeatWebSocketExample()
    {
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/notifications")
            )
            .RespondWith(Response.Create()
                .WithWebSocketHandler(async ctx =>
                {
                    var webSocket = ctx.WebSocket;
                    var buffer = new byte[1024 * 4];

                    // Send periodic heartbeat
                    _ = Task.Run(async () =>
                    {
                        while (webSocket.State == WebSocketState.Open)
                        {
                            try
                            {
                                var heartbeat = Encoding.UTF8.GetBytes("{\"type\":\"heartbeat\"}");
                                await webSocket.SendAsync(
                                    new ArraySegment<byte>(heartbeat),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);

                                await Task.Delay(5000);
                            }
                            catch
                            {
                                break;
                            }
                        }
                    });

                    // Echo incoming messages
                    while (webSocket.State == WebSocketState.Open)
                    {
                        try
                        {
                            var result = await webSocket.ReceiveAsync(
                                new ArraySegment<byte>(buffer),
                                CancellationToken.None);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await webSocket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Closing",
                                    CancellationToken.None);
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                })
                .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))
            );

        Console.WriteLine($"WebSocket server running at ws://localhost:{server.Port}/notifications");
    }

    /// <summary>
    /// Example 3: Message-based routing
    /// </summary>
    public static void MessageRoutingExample()
    {
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/api/ws")
            )
            .RespondWith(Response.Create()
                .WithWebSocketMessageHandler(async msg =>
                {
                    // Route based on message type
                    return msg.Type switch
                    {
                        "subscribe" => new WebSocketMessage
                        {
                            Type = "subscribed",
                            TextData = "{\"status\":\"subscribed\"}"
                        },
                        "ping" => new WebSocketMessage
                        {
                            Type = "pong",
                            TextData = "{\"type\":\"pong\"}"
                        },
                        _ => new WebSocketMessage
                        {
                            Type = "error",
                            TextData = $"{{\"error\":\"Unknown message type: {msg.Type}\"}}"
                        }
                    };
                })
                .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))
            );

        Console.WriteLine($"WebSocket server running at ws://localhost:{server.Port}/api/ws");
    }

    /// <summary>
    /// Example 4: WebSocket with custom headers validation
    /// </summary>
    public static void AuthenticatedWebSocketExample()
    {
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/secure-ws")
                .WithHeader("Authorization", "Bearer valid-token")
            )
            .RespondWith(Response.Create()
                .WithWebSocketHandler(async ctx =>
                {
                    // This handler only executes if Authorization header matches
                    var token = ctx.Headers.TryGetValue("Authorization", out var values)
                        ? values[0]
                        : "none";

                    var message = Encoding.UTF8.GetBytes($"{{\"authenticated\":true,\"token\":\"{token}\"}}");
                    await ctx.WebSocket.SendAsync(
                        new ArraySegment<byte>(message),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                })
            );

        Console.WriteLine($"Secure WebSocket server running at ws://localhost:{server.Port}/secure-ws");
    }

    /// <summary>
    /// Example 5: WebSocket with message streaming
    /// </summary>
    public static void StreamingWebSocketExample()
    {
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/stream")
            )
            .RespondWith(Response.Create()
                .WithWebSocketHandler(async ctx =>
                {
                    var webSocket = ctx.WebSocket;

                    // Stream 10 messages
                    for (int i = 0; i < 10; i++)
                    {
                        var message = Encoding.UTF8.GetBytes(
                            $"{{\"sequence\":{i},\"data\":\"Item {i}\",\"timestamp\":\"{DateTime.UtcNow:O}\"}}");

                        await webSocket.SendAsync(
                            new ArraySegment<byte>(message),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);

                        await Task.Delay(1000);
                    }

                    // Send completion message
                    var completion = Encoding.UTF8.GetBytes("{\"type\":\"complete\"}");
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(completion),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);

                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Stream complete",
                        CancellationToken.None);
                })
                .WithWebSocketTimeout(TimeSpan.FromMinutes(5))
            );

        Console.WriteLine($"Streaming WebSocket server running at ws://localhost:{server.Port}/stream");
    }
}
