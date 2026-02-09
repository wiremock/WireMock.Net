// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
}