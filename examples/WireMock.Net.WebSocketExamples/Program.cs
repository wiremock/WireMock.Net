// Copyright © WireMock.Net

using System.Net.WebSockets;
using System.Text;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.WebSocketExamples;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("WireMock.Net WebSocket Examples");
        Console.WriteLine("================================\n");

        Console.WriteLine("Choose an example to run:");
        Console.WriteLine("1. Echo Server");
        Console.WriteLine("2. Custom Message Handler");
        Console.WriteLine("3. ...");
        Console.WriteLine("4. Scenario/State Machine");
        Console.WriteLine("5. WebSocket Proxy");
        Console.WriteLine("6. Multiple WebSocket Endpoints");
        Console.WriteLine("7. All Examples (runs all endpoints)");
        Console.WriteLine("0. Exit\n");

        Console.Write("Enter choice: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await RunEchoServerExample();
                break;
            case "2":
                await RunCustomMessageHandlerExample();
                break;
            case "3":
                await RunBroadcastExample();
                break;
            case "4":
                await RunScenarioExample();
                break;
            case "5":
                await RunProxyExample();
                break;
            case "6":
                await RunMultipleEndpointsExample();
                break;
            case "7":
                await RunAllExamples();
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Invalid choice");
                break;
        }
    }

    /// <summary>
    /// Example 1: Simple Echo Server
    /// Echoes back all messages received from the client
    /// </summary>
    private static async Task RunEchoServerExample()
    {
        Console.WriteLine("\n=== Echo Server Example ===");
        Console.WriteLine("Starting WebSocket echo server...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger()
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithEcho()
                )
            );

        Console.WriteLine($"Echo server listening at: {server.Urls[0]}/ws/echo");
        Console.WriteLine("\nTest with a WebSocket client:");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/echo");
        Console.WriteLine("\nPress any key to test or CTRL+C to exit...");
        Console.ReadKey();

        // Test the echo server
        await TestWebSocketEcho(server.Urls[0]);

        Console.WriteLine("\nPress any key to stop server...");
        Console.ReadKey();
        server.Stop();
    }

    /// <summary>
    /// Example 2: Custom Message Handler
    /// Processes messages and sends custom responses
    /// </summary>
    private static async Task RunCustomMessageHandlerExample()
    {
        Console.WriteLine("\n=== Custom Message Handler Example ===");
        Console.WriteLine("Starting WebSocket server with custom message handler...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger()
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/chat")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            
                            // Handle different commands
                            if (text.StartsWith("/help"))
                            {
                                await context.SendAsync("Available commands: /help, /time, /echo <text>, /upper <text>, /reverse <text>");
                            }
                            else if (text.StartsWith("/time"))
                            {
                                await context.SendAsync($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                            }
                            else if (text.StartsWith("/echo "))
                            {
                                await context.SendAsync(text.Substring(6));
                            }
                            else if (text.StartsWith("/upper "))
                            {
                                await context.SendAsync(text.Substring(7).ToUpper());
                            }
                            else if (text.StartsWith("/reverse "))
                            {
                                var toReverse = text.Substring(9);
                                var reversed = new string(toReverse.Reverse().ToArray());
                                await context.SendAsync(reversed);
                            }
                            else if (text == "/quit")
                            {
                                await context.SendAsync("Goodbye!");
                                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested disconnect");
                            }
                            else
                            {
                                await context.SendAsync($"Unknown command: {text}. Type /help for available commands.");
                            }
                        }
                    })
                )
            );

        Console.WriteLine($"Chat server listening at: {server.Urls[0]}/ws/chat");
        Console.WriteLine("\nTest with:");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/chat");
        Console.WriteLine("\nThen try commands: /help, /time, /echo hello, /upper hello, /reverse hello");
        Console.WriteLine("\nPress any key to test or CTRL+C to exit...");
        Console.ReadKey();

        await TestWebSocketChat(server.Urls[0]);

        Console.WriteLine("\nPress any key to stop server...");
        Console.ReadKey();
        server.Stop();
    }

    /// <summary>
    /// Example 3: Broadcast Server
    /// Broadcasts messages to all connected clients
    /// </summary>
    private static async Task RunBroadcastExample()
    {
        Console.WriteLine("\n=== Broadcast Server Example ===");
        Console.WriteLine("Starting WebSocket broadcast server...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger()
        });

        var broadcastMappingGuid = Guid.NewGuid();

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast")
                .WithWebSocketUpgrade()
            )
            .WithGuid(broadcastMappingGuid)
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithBroadcast()
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");
                            var broadcastMessage = $"[{timestamp}] Broadcast: {text}";
                            
                            // Broadcast to all connected clients
                            await context.BroadcastTextAsync(broadcastMessage);
                            
                            Console.WriteLine($"Broadcasted to {server.GetWebSocketConnections(broadcastMappingGuid).Count} clients: {text}");
                        }
                    })
                )
            );

        Console.WriteLine($"Broadcast server listening at: {server.Urls[0]}/ws/broadcast");
        Console.WriteLine("\nConnect multiple clients:");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/broadcast");
        Console.WriteLine("\nMessages sent from any client will be broadcast to all clients");
        Console.WriteLine("\nPress any key to stop server...");
        Console.ReadKey();
        server.Stop();
    }

    /// <summary>
    /// Example 4: Scenario/State Machine
    /// Demonstrates state transitions during WebSocket session
    /// </summary>
    private static async Task RunScenarioExample()
    {
        Console.WriteLine("\n=== Scenario/State Machine Example ===");
        Console.WriteLine("Starting WebSocket server with scenario support...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger()
        });

        // Initial state: Waiting for players
        server
            .Given(Request.Create()
                .WithPath("/ws/game")
                .WithWebSocketUpgrade()
            )
            .InScenario("GameSession")
            .WillSetStateTo("Lobby")
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        await ctx.SendAsync("Welcome to the game lobby! Type 'ready' to start or 'quit' to leave.");
                    })
                )
            );

        // Lobby state: Waiting for ready
        server
            .Given(Request.Create()
                .WithPath("/ws/game")
                .WithWebSocketUpgrade()
            )
            .InScenario("GameSession")
            .WhenStateIs("Lobby")
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        var text = msg.Text?.ToLower() ?? string.Empty;
                        
                        if (text == "ready")
                        {
                            ctx.SetScenarioState("Playing");
                            await ctx.SendAsync("Game started! Type 'attack' to attack, 'defend' to defend, or 'quit' to exit.");
                        }
                        else if (text == "quit")
                        {
                            await ctx.SendAsync("You left the lobby. Goodbye!");
                            await ctx.CloseAsync(WebSocketCloseStatus.NormalClosure, "Player quit");
                        }
                        else
                        {
                            await ctx.SendAsync("In lobby. Type 'ready' to start or 'quit' to leave.");
                        }
                    })
                )
            );

        // Playing state: Game is active
        server
            .Given(Request.Create()
                .WithPath("/ws/game")
                .WithWebSocketUpgrade()
            )
            .InScenario("GameSession")
            .WhenStateIs("Playing")
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        var text = msg.Text?.ToLower() ?? string.Empty;
                        
                        if (text == "attack")
                        {
                            await ctx.SendAsync("You attacked! Critical hit! 💥");
                        }
                        else if (text == "defend")
                        {
                            await ctx.SendAsync("You defended! Shield up! 🛡️");
                        }
                        else if (text == "quit")
                        {
                            ctx.SetScenarioState("GameOver");
                            await ctx.SendAsync("Game over! Thanks for playing.");
                            await ctx.CloseAsync(WebSocketCloseStatus.NormalClosure, "Game ended");
                        }
                        else
                        {
                            await ctx.SendAsync("Unknown action. Type 'attack', 'defend', or 'quit'.");
                        }
                    })
                )
            );

        Console.WriteLine($"Game server listening at: {server.Urls[0]}/ws/game");
        Console.WriteLine("\nConnect and follow the game flow:");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/game");
        Console.WriteLine("\nGame flow: Lobby -> Type 'ready' -> Playing -> Type 'attack'/'defend' -> Type 'quit'");
        Console.WriteLine("\nPress any key to stop server...");
        Console.ReadKey();
        server.Stop();
    }

    /// <summary>
    /// Example 5: WebSocket Proxy
    /// Proxies WebSocket connections to another server
    /// </summary>
    private static async Task RunProxyExample()
    {
        Console.WriteLine("\n=== WebSocket Proxy Example ===");
        Console.WriteLine("Starting WebSocket proxy server...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger()
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/proxy")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocketProxy("ws://echo.websocket.org")
            );

        Console.WriteLine($"Proxy server listening at: {server.Urls[0]}/ws/proxy");
        Console.WriteLine("Proxying to: ws://echo.websocket.org");
        Console.WriteLine("\nTest with:");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/proxy");
        Console.WriteLine("\nPress any key to stop server...");
        Console.ReadKey();
        server.Stop();
    }

    /// <summary>
    /// Example 6: Multiple WebSocket Endpoints
    /// Demonstrates running multiple WebSocket endpoints simultaneously
    /// </summary>
    private static async Task RunMultipleEndpointsExample()
    {
        Console.WriteLine("\n=== Multiple WebSocket Endpoints Example ===");
        Console.WriteLine("Starting server with multiple WebSocket endpoints...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger(),
            WebSocketSettings = new WebSocketSettings
            {
                MaxConnections = 100,
                KeepAliveIntervalSeconds = 30
            }
        });

        // Endpoint 1: Echo
        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        // Endpoint 2: Time service
        server
            .Given(Request.Create()
                .WithPath("/ws/time")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        await ctx.SendAsync($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                    })
                )
            );

        // Endpoint 4: Protocol-specific
        server
            .Given(Request.Create()
                .WithPath("/ws/protocol")
                .WithWebSocketUpgrade("chat", "superchat")
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithAcceptProtocol("chat")
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        await ctx.SendAsync($"Using protocol: chat. Message: {msg.Text}");
                    })
                )
            );

        Console.WriteLine("Available WebSocket endpoints:");
        Console.WriteLine($"  1. Echo:     {server.Urls[0]}/ws/echo");
        Console.WriteLine($"  2. Time:     {server.Urls[0]}/ws/time");
        Console.WriteLine($"  3. JSON:     {server.Urls[0]}/ws/json");
        Console.WriteLine($"  4. Protocol: {server.Urls[0]}/ws/protocol");
        Console.WriteLine("\nTest with wscat:");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/echo");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/time");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/json");
        Console.WriteLine("  wscat -c ws://localhost:9091/ws/protocol -s chat");
        Console.WriteLine("\nPress any key to stop server...");
        Console.ReadKey();
        server.Stop();
    }

    /// <summary>
    /// Example 7: Run All Examples
    /// Starts a server with all example endpoints
    /// </summary>
    private static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== All Examples Running ===");
        Console.WriteLine("Starting server with all WebSocket endpoints...\n");

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            Logger = new WireMockConsoleLogger(),
            WebSocketSettings = new WebSocketSettings
            {
                MaxConnections = 200
            }
        });

        SetupAllEndpoints(server);

        Console.WriteLine("All WebSocket endpoints are running:");
        Console.WriteLine($"  Echo:      {server.Urls[0]}/ws/echo");
        Console.WriteLine($"  Chat:      {server.Urls[0]}/ws/chat");
        Console.WriteLine($"  Broadcast: {server.Urls[0]}/ws/broadcast");
        Console.WriteLine($"  Game:      {server.Urls[0]}/ws/game");
        Console.WriteLine($"  Time:      {server.Urls[0]}/ws/time");
        Console.WriteLine($"  JSON:      {server.Urls[0]}/ws/json");
        Console.WriteLine("\nServer statistics:");
        Console.WriteLine($"  Total mappings: {server.Mappings.Count}");
        
        Console.WriteLine("\nPress any key to view connection stats or CTRL+C to exit...");
        
        while (true)
        {
            Console.ReadKey(true);
            var connections = server.GetWebSocketConnections();
            Console.WriteLine($"\nActive WebSocket connections: {connections.Count}");
            foreach (var conn in connections)
            {
                Console.WriteLine($"  - {conn.ConnectionId}: {conn.RequestMessage.Path} (State: {conn.WebSocket.State})");
            }
            Console.WriteLine("\nPress any key to refresh or CTRL+C to exit...");
        }
    }

    private static void SetupAllEndpoints(WireMockServer server)
    {
        // Echo endpoint
        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        // Chat endpoint
        server
            .Given(Request.Create()
                .WithPath("/ws/chat")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            await context.SendAsync($"Echo: {message.Text}");
                        }
                    })
                )
            );

        // Broadcast endpoint
        var broadcastGuid = Guid.NewGuid();
        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast")
                .WithWebSocketUpgrade()
            )
            .WithGuid(broadcastGuid)
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithBroadcast()
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            await context.BroadcastTextAsync($"[Broadcast] {message.Text}");
                        }
                    })
                )
            );

        // Game scenario endpoint
        SetupGameScenario(server);

        // Time endpoint
        server
            .Given(Request.Create()
                .WithPath("/ws/time")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        await ctx.SendAsync($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                    })
                )
            );
    }

    private static void SetupGameScenario(WireMockServer server)
    {
        server
            .Given(Request.Create()
                .WithPath("/ws/game")
                .WithWebSocketUpgrade()
            )
            .InScenario("GameSession")
            .WillSetStateTo("Lobby")
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        await ctx.SendAsync("Welcome! Type 'ready' to start.");
                    })
                )
            );

        server
            .Given(Request.Create()
                .WithPath("/ws/game")
                .WithWebSocketUpgrade()
            )
            .InScenario("GameSession")
            .WhenStateIs("Lobby")
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        if (msg.Text?.ToLower() == "ready")
                        {
                            ctx.SetScenarioState("Playing");
                            await ctx.SendAsync("Game started!");
                        }
                    })
                )
            );
    }

    // Helper methods for testing
    private static async Task TestWebSocketEcho(string baseUrl)
    {
        try
        {
            using var client = new ClientWebSocket();
            var uri = new Uri($"{baseUrl.Replace("http://", "ws://")}/ws/echo");
            
            Console.WriteLine($"\nConnecting to {uri}...");
            await client.ConnectAsync(uri, CancellationToken.None);
            Console.WriteLine("Connected!");

            var testMessages = new[] { "Hello", "World", "WebSocket", "Test" };
            
            foreach (var testMessage in testMessages)
            {
                Console.WriteLine($"\nSending: {testMessage}");
                var bytes = Encoding.UTF8.GetBytes(testMessage);
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

                var buffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var received = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {received}");
            }

            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
            Console.WriteLine("\nTest completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nTest failed: {ex.Message}");
        }
    }

    private static async Task TestWebSocketChat(string baseUrl)
    {
        try
        {
            using var client = new ClientWebSocket();
            var uri = new Uri($"{baseUrl.Replace("http://", "ws://")}/ws/chat");
            
            Console.WriteLine($"\nConnecting to {uri}...");
            await client.ConnectAsync(uri, CancellationToken.None);
            Console.WriteLine("Connected!");

            var commands = new[] { "/help", "/time", "/echo Hello", "/upper test", "/reverse hello" };
            
            foreach (var command in commands)
            {
                Console.WriteLine($"\nSending: {command}");
                var bytes = Encoding.UTF8.GetBytes(command);
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

                var buffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var received = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {received}");
                
                await Task.Delay(500);
            }

            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
            Console.WriteLine("\nTest completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nTest failed: {ex.Message}");
        }
    }
}
