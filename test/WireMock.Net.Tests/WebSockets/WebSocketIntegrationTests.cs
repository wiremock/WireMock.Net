// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;
using Xunit.Abstractions;

namespace WireMock.Net.Tests.WebSockets;

public class WebSocketIntegrationTests
{
    private readonly ITestOutputHelper _output;

    public WebSocketIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task EchoServer_Should_Echo_Text_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithHeader("x", "y")
                .WithWebSocket(ws => ws
                    .WithEcho()
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/echo");

        // Act
        await client.ConnectAsync(uri, CancellationToken.None);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Hello, WebSocket!";
        var sendBytes = Encoding.UTF8.GetBytes(testMessage);
        await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

        // Assert
        result.MessageType.Should().Be(WebSocketMessageType.Text);
        result.EndOfMessage.Should().BeTrue();
        var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
        received.Should().Be(testMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task EchoServer_Should_Echo_Multiple_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/echo");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testMessages = new[] { "Hello", "World", "WebSocket", "Test" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            var sendBytes = Encoding.UTF8.GetBytes(testMessage);
            await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

            received.Should().Be(testMessage, $"message '{testMessage}' should be echoed back");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task EchoServer_Should_Echo_Binary_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/echo");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        await client.SendAsync(new ArraySegment<byte>(testData), WebSocketMessageType.Binary, true, CancellationToken.None);

        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

        // Assert
        result.MessageType.Should().Be(WebSocketMessageType.Binary);
        var receivedData = new byte[result.Count];
        Array.Copy(receiveBuffer, receivedData, result.Count);
        receivedData.Should().BeEquivalentTo(testData);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task EchoServer_Should_Handle_Empty_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/echo");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        var sendBytes = Encoding.UTF8.GetBytes(string.Empty);
        await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

        // Assert
        result.Count.Should().Be(0);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task CustomHandler_Should_Handle_Help_Command()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
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

                            if (text.StartsWith("/help"))
                            {
                                await context.SendTextAsync("Available commands: /help, /time, /echo <text>, /upper <text>, /reverse <text>");
                            }
                        }
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/chat");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        var sendBytes = Encoding.UTF8.GetBytes("/help");
        await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

        // Assert
        received.Should().Contain("Available commands");
        received.Should().Contain("/help");
        received.Should().Contain("/time");
        received.Should().Contain("/echo");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task CustomHandler_Should_Handle_Multiple_Commands_In_Sequence()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
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

                            if (text.StartsWith("/help"))
                            {
                                await context.SendTextAsync("Available commands: /help, /time, /echo <text>, /upper <text>, /reverse <text>");
                            }
                            else if (text.StartsWith("/time"))
                            {
                                await context.SendTextAsync($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                            }
                            else if (text.StartsWith("/echo "))
                            {
                                await context.SendTextAsync(text.Substring(6));
                            }
                            else if (text.StartsWith("/upper "))
                            {
                                await context.SendTextAsync(text.Substring(7).ToUpper());
                            }
                            else if (text.StartsWith("/reverse "))
                            {
                                var toReverse = text.Substring(9);
                                var reversed = new string(toReverse.Reverse().ToArray());
                                await context.SendTextAsync(reversed);
                            }
                        }
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/chat");
        await client.ConnectAsync(uri, CancellationToken.None);

        var commands = new (string, Action<string>)[]
        {
            ("/help", (string response) => response.Should().Contain("Available commands")),
            ("/time", (string response) => response.Should().Contain("Server time")),
            ("/echo Test", (string response) => response.Should().Be("Test")),
            ("/upper test", (string response) => response.Should().Be("TEST")),
            ("/reverse hello", (string response) => response.Should().Be("olleh"))
        };

        // Act & Assert
        foreach (var (command, assertion) in commands)
        {
            var sendBytes = Encoding.UTF8.GetBytes(command);
            await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

            assertion(received);
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task SendJsonAsync_Should_Send_Json_Response()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/json")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithHeader("x", "y")
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        var response = new
                        {
                            timestamp = DateTime.UtcNow,
                            message = msg.Text,
                            length = msg.Text?.Length ?? 0,
                            type = msg.MessageType.ToString()
                        };
                        await ctx.SendJsonAsync(response);
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/json");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        var testMessage = "Test JSON message";
        var sendBytes = Encoding.UTF8.GetBytes(testMessage);
        await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        var receiveBuffer = new byte[2048];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

        // Assert
        result.MessageType.Should().Be(WebSocketMessageType.Text);
        
        var json = JObject.Parse(received);
        json["message"]!.ToString().Should().Be(testMessage);
        json["length"]!.Value<int>().Should().Be(testMessage.Length);
        json["type"]!.ToString().Should().Be("Text");
        json["timestamp"].Should().NotBeNull();

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task SendJsonAsync_Should_Handle_Multiple_Json_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/json")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        var response = new
                        {
                            timestamp = DateTime.UtcNow,
                            message = msg.Text,
                            length = msg.Text?.Length ?? 0,
                            type = msg.MessageType.ToString(),
                            connectionId = ctx.ConnectionId.ToString()
                        };
                        await ctx.SendJsonAsync(response);
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/json");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            var sendBytes = Encoding.UTF8.GetBytes(testMessage);
            await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer = new byte[2048];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

            var json = JObject.Parse(received);
            json["message"]!.ToString().Should().Be(testMessage);
            json["length"]!.Value<int>().Should().Be(testMessage.Length);
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task SendJsonAsync_Should_Serialize_Complex_Objects()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/json")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (msg, ctx) =>
                    {
                        var response = new
                        {
                            status = "success",
                            data = new
                            {
                                originalMessage = msg.Text,
                                processedAt = DateTime.UtcNow,
                                metadata = new
                                {
                                    length = msg.Text?.Length ?? 0,
                                    type = msg.MessageType.ToString()
                                }
                            },
                            nested = new[]
                            {
                                new { id = 1, name = "Item1" },
                                new { id = 2, name = "Item2" }
                            }
                        };
                        await ctx.SendJsonAsync(response);
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/json");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        var testMessage = "Complex test";
        var sendBytes = Encoding.UTF8.GetBytes(testMessage);
        await client.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        var receiveBuffer = new byte[2048];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        var received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

        // Assert
        var json = JObject.Parse(received);
        json["status"]!.ToString().Should().Be("success");
        json["data"]!["originalMessage"]!.ToString().Should().Be(testMessage);
        json["data"]!["metadata"]!["length"]!.Value<int>().Should().Be(testMessage.Length);
        json["nested"]!.Should().HaveCount(2);
        json["nested"]![0]!["id"]!.Value<int>().Should().Be(1);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task Broadcast_Should_Send_Message_To_All_Connected_Clients()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
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
                        }
                    })
                )
            );

        // Connect multiple clients
        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();
        using var client3 = new ClientWebSocket();

        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/broadcast");

        await client1.ConnectAsync(uri, CancellationToken.None);
        await client2.ConnectAsync(uri, CancellationToken.None);
        await client3.ConnectAsync(uri, CancellationToken.None);

        // Wait a moment for all connections to be registered
        await Task.Delay(100);

        // Act - Send message from client1
        var testMessage = "Hello everyone!";
        var sendBytes = Encoding.UTF8.GetBytes(testMessage);
        await client1.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        // Assert - All clients should receive the broadcast
        var receiveBuffer1 = new byte[1024];
        var result1 = await client1.ReceiveAsync(new ArraySegment<byte>(receiveBuffer1), CancellationToken.None);
        var received1 = Encoding.UTF8.GetString(receiveBuffer1, 0, result1.Count);

        var receiveBuffer2 = new byte[1024];
        var result2 = await client2.ReceiveAsync(new ArraySegment<byte>(receiveBuffer2), CancellationToken.None);
        var received2 = Encoding.UTF8.GetString(receiveBuffer2, 0, result2.Count);

        var receiveBuffer3 = new byte[1024];
        var result3 = await client3.ReceiveAsync(new ArraySegment<byte>(receiveBuffer3), CancellationToken.None);
        var received3 = Encoding.UTF8.GetString(receiveBuffer3, 0, result3.Count);

        received1.Should().Contain("Broadcast:").And.Contain(testMessage);
        received2.Should().Contain("Broadcast:").And.Contain(testMessage);
        received3.Should().Contain("Broadcast:").And.Contain(testMessage);

        // All should receive the same message
        received1.Should().Be(received2);
        received2.Should().Be(received3);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
        await client3.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task Broadcast_Should_Only_Send_To_Open_Connections()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
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
                            await context.BroadcastTextAsync($"Broadcast: {message.Text}");
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/broadcast");

        await client1.ConnectAsync(uri, CancellationToken.None);
        await client2.ConnectAsync(uri, CancellationToken.None);

        await Task.Delay(100);

        // Close client2
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Leaving", CancellationToken.None);
        await Task.Delay(100);

        // Act - Send message from client1 (client2 is now closed)
        var testMessage = "Still here";
        var sendBytes = Encoding.UTF8.GetBytes(testMessage);
        await client1.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        // Assert - Only client1 should receive
        var receiveBuffer1 = new byte[1024];
        var result1 = await client1.ReceiveAsync(new ArraySegment<byte>(receiveBuffer1), CancellationToken.None);
        var received1 = Encoding.UTF8.GetString(receiveBuffer1, 0, result1.Count);

        received1.Should().Contain("Broadcast:").And.Contain(testMessage);
        client2.State.Should().Be(WebSocketState.Closed);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task BroadcastJson_Should_Send_Json_To_All_Clients()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        var broadcastMappingGuid = Guid.NewGuid();

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-json")
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
                            var data = new
                            {
                                sender = context.ConnectionId,
                                message = message.Text,
                                timestamp = DateTime.UtcNow,
                                type = "broadcast"
                            };
                            await context.BroadcastJsonAsync(data);
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/broadcast-json");

        await client1.ConnectAsync(uri, CancellationToken.None);
        await client2.ConnectAsync(uri, CancellationToken.None);

        await Task.Delay(100);

        // Act - Send message from client1
        var testMessage = "JSON broadcast test";
        var sendBytes = Encoding.UTF8.GetBytes(testMessage);
        await client1.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        // Assert - Both clients should receive JSON
        var receiveBuffer1 = new byte[2048];
        var result1 = await client1.ReceiveAsync(new ArraySegment<byte>(receiveBuffer1), CancellationToken.None);
        var received1 = Encoding.UTF8.GetString(receiveBuffer1, 0, result1.Count);

        var receiveBuffer2 = new byte[2048];
        var result2 = await client2.ReceiveAsync(new ArraySegment<byte>(receiveBuffer2), CancellationToken.None);
        var received2 = Encoding.UTF8.GetString(receiveBuffer2, 0, result2.Count);

        var json1 = JObject.Parse(received1);
        var json2 = JObject.Parse(received2);

        json1["message"]!.ToString().Should().Be(testMessage);
        json1["type"]!.ToString().Should().Be("broadcast");
        json1["sender"].Should().NotBeNull();

        json2["message"]!.ToString().Should().Be(testMessage);
        json2["type"]!.ToString().Should().Be("broadcast");

        // Both should have the same content
        json1["message"]!.ToString().Should().Be(json2["message"]!.ToString());

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task Broadcast_Should_Handle_Multiple_Sequential_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
        });

        var broadcastMappingGuid = Guid.NewGuid();
        var messageCount = 0;

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
                            Interlocked.Increment(ref messageCount);
                            await context.BroadcastTextAsync($"Message {messageCount}: {message.Text}");
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/broadcast");

        await client1.ConnectAsync(uri, CancellationToken.None);
        await client2.ConnectAsync(uri, CancellationToken.None);

        await Task.Delay(100);

        var messages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var msg in messages)
        {
            var sendBytes = Encoding.UTF8.GetBytes(msg);
            await client1.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer1 = new byte[1024];
            var result1 = await client1.ReceiveAsync(new ArraySegment<byte>(receiveBuffer1), CancellationToken.None);
            var received1 = Encoding.UTF8.GetString(receiveBuffer1, 0, result1.Count);

            var receiveBuffer2 = new byte[1024];
            var result2 = await client2.ReceiveAsync(new ArraySegment<byte>(receiveBuffer2), CancellationToken.None);
            var received2 = Encoding.UTF8.GetString(receiveBuffer2, 0, result2.Count);

            received1.Should().Contain(msg);
            received2.Should().Contain(msg);
            received1.Should().Be(received2);
        }

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task Broadcast_Should_Work_With_Many_Clients()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_output)
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
                            await context.BroadcastTextAsync($"Broadcast: {message.Text}");
                        }
                    })
                )
            );

        var uri = new Uri($"{server.Urls[0].Replace("http://", "ws://")}/ws/broadcast");
        const int clientCount = 5;
        var clients = new List<ClientWebSocket>();

        try
        {
            // Connect multiple clients
            for (int i = 0; i < clientCount; i++)
            {
                var client = new ClientWebSocket();
                await client.ConnectAsync(uri, CancellationToken.None);
                clients.Add(client);
            }

            await Task.Delay(200); // Give time for all connections to register

            // Act - Send message from first client
            var testMessage = "Mass broadcast";
            var sendBytes = Encoding.UTF8.GetBytes(testMessage);
            await clients[0].SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            // Assert - All clients should receive
            var receiveTasks = clients.Select(async client =>
            {
                var receiveBuffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                return Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            }).ToList();

            var received = await Task.WhenAll(receiveTasks);

            received.Should().HaveCount(clientCount);
            received.Should().OnlyContain(msg => msg.Contains("Broadcast:") && msg.Contains(testMessage));
        }
        finally
        {
            // Cleanup
            foreach (var client in clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
                }
                client.Dispose();
            }
        }
    }
}