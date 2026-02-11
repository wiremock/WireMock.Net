// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using WireMock.Matchers;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit.Abstractions;

namespace WireMock.Net.Tests.WebSockets;

public class WebSocketIntegrationTests(ITestOutputHelper output)
{
    [Fact]
    public async Task EchoServer_Should_Echo_Text_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
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
        var uri = new Uri($"{server.Url!}/ws/echo");

        // Act
        await client.ConnectAsync(uri, CancellationToken.None);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Hello, WebSocket!";
        await client.SendAsync(testMessage);

        // Assert
        var received = await client.ReceiveAsTextAsync();
        received.Should().Be(testMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WithText_Should_Send_Configured_Text()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var responseMessage = "This is a predefined response";

        server
            .Given(Request.Create()
                .WithPath("/ws/message")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithText(responseMessage))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/message");

        // Act
        await client.ConnectAsync(uri, CancellationToken.None);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage);

        // Assert
        var received = await client.ReceiveAsTextAsync();
        received.Should().Be(responseMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WithText_Should_Send_Same_Text_For_Multiple_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var responseMessage = "Fixed response";

        server
            .Given(Request.Create()
                .WithPath("/ws/message")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithText(responseMessage))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/message");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage);

            var received = await client.ReceiveAsTextAsync();
            received.Should().Be(responseMessage, $"should always return the fixed response regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WithBytes_Should_Send_Configured_Bytes()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var responseBytes = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

        server
            .Given(Request.Create()
                .WithPath("/ws/binary")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithBytes(responseBytes))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/binary");

        // Act
        await client.ConnectAsync(uri, CancellationToken.None);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage);

        // Assert
        var receivedData = await client.ReceiveAsBytesAsync();
        receivedData.Should().BeEquivalentTo(responseBytes);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WithBytes_Should_Send_Same_Bytes_For_Multiple_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var responseBytes = new byte[] { 0x01, 0x02, 0x03 };

        server
            .Given(Request.Create()
                .WithPath("/ws/binary")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithBytes(responseBytes))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/binary");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage);

            var receivedData = await client.ReceiveAsBytesAsync();
            receivedData.Should().BeEquivalentTo(responseBytes, $"should always return the fixed bytes regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WithJson_Should_Send_Configured_Json()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var responseData = new
        {
            status = "ok",
            message = "This is a predefined JSON response",
            timestamp = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc)
        };

        server
            .Given(Request.Create()
                .WithPath("/ws/json")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithJson(responseData))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/json");

        // Act
        await client.ConnectAsync(uri, CancellationToken.None);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage);

        // Assert
        var received = await client.ReceiveAsTextAsync();
        
        var json = JObject.Parse(received);
        json["status"]!.ToString().Should().Be("ok");
        json["message"]!.ToString().Should().Be("This is a predefined JSON response");
        json["timestamp"].Should().NotBeNull();

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WithJson_Should_Send_Same_Json_For_Multiple_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var responseData = new
        {
            id = 42,
            name = "Fixed JSON Response"
        };

        server
            .Given(Request.Create()
                .WithPath("/ws/json")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithJson(responseData))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/json");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage);

            var received = await client.ReceiveAsTextAsync();

            var json = JObject.Parse(received);
            json["id"]!.Value<int>().Should().Be(42);
            json["name"]!.ToString().Should().Be("Fixed JSON Response", $"should always return the fixed JSON regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task EchoServer_Should_Echo_Multiple_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
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
        var uri = new Uri($"{server.Url!}/ws/echo");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testMessages = new[] { "Hello", "World", "WebSocket", "Test" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage);

            var received = await client.ReceiveAsTextAsync();

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
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
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
        var uri = new Uri($"{server.Url!}/ws/echo");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        await client.SendAsync(new ArraySegment<byte>(testData), WebSocketMessageType.Binary, true, CancellationToken.None);

        var receivedData = await client.ReceiveAsBytesAsync();

        // Assert
        receivedData.Should().BeEquivalentTo(testData);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task EchoServer_Should_Handle_Empty_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
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
        var uri = new Uri($"{server.Url!}/ws/echo");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        await client.SendAsync(string.Empty);

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
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
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
                                await context.SendAsync("Available commands: /help, /time, /echo <text>, /upper <text>, /reverse <text>");
                            }
                        }
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/chat");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        await client.SendAsync("/help");

        var received = await client.ReceiveAsTextAsync();

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
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
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
                            else if (text.StartsWith("/close"))
                            {
                                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection");
                            }
                        }
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/chat");
        await client.ConnectAsync(uri, CancellationToken.None);

        var commands = new (string, Action<string>)[]
        {
            ("/help", response => response.Should().Contain("Available commands")),
            ("/time", response => response.Should().Contain("Server time")),
            ("/echo Test", response => response.Should().Be("Test")),
            ("/upper test", response => response.Should().Be("TEST")),
            ("/reverse hello", response => response.Should().Be("olleh"))
        };

        // Act & Assert
        foreach (var (command, assertion) in commands)
        {
            await client.SendAsync(command);

            var received = await client.ReceiveAsTextAsync();

            assertion(received);
        }

        await client.SendAsync("/close");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WhenMessage_Should_Handle_Multiple_Conditions_Fluently()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/conditional")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(3))
                    .WhenMessage("/help").SendMessage(m => m.WithText("Available commands"))
                    .WhenMessage("/time").SendMessage(m => m.WithText($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"))
                    .WhenMessage("/echo *").SendMessage(m => m.WithText("echo response"))
                    .WhenMessage(new ExactMatcher("/exact")).SendMessage(m => m.WithText("is exact"))
                    .WhenMessage(new FuncMatcher(s => s == "/func")).SendMessage(m => m.WithText("is func"))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/conditional");
        await client.ConnectAsync(uri, CancellationToken.None);

        var testCases = new (string message, string expectedContains)[]
        {
            ("/help", "Available commands"),
            ("/time", "Server time"),
            ("/echo test", "echo response"),
            ("/exact", "is exact"),
            ("/func", "is func")
        };

        // Act & Assert
        foreach (var (message, expectedContains) in testCases)
        {
            await client.SendAsync(message);

            var received = await client.ReceiveAsTextAsync();

            received.Should().Contain(expectedContains, $"message '{message}' should return response containing '{expectedContains}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task WhenMessage_Should_Close_Connection_When_AndClose_Is_Used()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/close")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WhenMessage("/close").SendMessage(m => m.WithText("Closing connection").AndClose())
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url!}/ws/close");
        await client.ConnectAsync(uri, CancellationToken.None);

        // Act
        await client.SendAsync("/close");

        var received = await client.ReceiveAsTextAsync();

        // Assert
        received.Should().Contain("Closing connection");

        // Try to receive again - this will complete the close handshake
        // and update the client state to Closed
        try
        {
            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            
            // If we get here, the message type should be Close
            result.MessageType.Should().Be(WebSocketMessageType.Close);
        }
        catch (WebSocketException)
        {
            // Connection was closed, which is expected
        }

        // Verify the connection is CloseReceived
        client.State.Should().Be(WebSocketState.CloseReceived);
    }

    [Fact]
    public async Task Server_With_Multiple_Urls_Should_Handle_Http_And_WebSocket_In_Parallel()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["http://localhost:0", "ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/api/test")
                .UsingGet()
            )
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("OK")
            );

        server
            .Given(Request.Create()
                .WithPath("/ws/echo")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        // Act & Assert
        var httpTask = Task.Run(async () =>
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{server.Urls[0]}/api/test");

            await Task.Delay(100);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("OK");
        });

        var webSocketTask = Task.Run(async () =>
        {
            using var client = new ClientWebSocket();
            var uri = new Uri($"{server.Urls[1]}/ws/echo");

            await client.ConnectAsync(uri, default);
            client.State.Should().Be(WebSocketState.Open);

            var testMessage = "Hello from WebSocket";
            await client.SendAsync(testMessage);

            await Task.Delay(100);

            var received = await client.ReceiveAsTextAsync();
            received.Should().Be(testMessage);

            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", default);
        });

        await Task.WhenAll(httpTask, webSocketTask);
    }
}