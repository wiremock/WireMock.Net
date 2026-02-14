// Copyright © WireMock.Net

using System.Net.WebSockets;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests.WebSockets;

public class WebSocketIntegrationTests(ITestOutputHelper output)
{
    [Fact]
    public async Task EchoServer_Should_Echo_Text_Messages()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/echo");

        // Act
        await client.ConnectAsync(uri, CancellationToken.None);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Hello, WebSocket!";
        await client.SendAsync(testMessage, cancellationToken: cancelationToken);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);
        received.Should().Be(testMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task WithText_Should_Send_Configured_Text()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/message");

        // Act
        await client.ConnectAsync(uri, cancelationToken);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage, cancellationToken: cancelationToken);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);
        received.Should().Be(responseMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task WithText_Should_Send_Same_Text_For_Multiple_Messages()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/message");
        await client.ConnectAsync(uri, cancelationToken);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: cancelationToken);

            var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);
            received.Should().Be(responseMessage, $"should always return the fixed response regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task WithBinary_Should_Send_Configured_Bytes()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
                    .SendMessage(m => m.WithBinary(responseBytes))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/binary");

        // Act
        await client.ConnectAsync(uri, cancelationToken);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage, cancellationToken: cancelationToken);

        // Assert
        var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: cancelationToken);
        receivedData.Should().BeEquivalentTo(responseBytes);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task WithBinary_Should_Send_Same_Bytes_For_Multiple_Messages()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
                    .SendMessage(m => m.WithBinary(responseBytes))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/binary");
        await client.ConnectAsync(uri, cancelationToken);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: cancelationToken);

            var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: cancelationToken);
            receivedData.Should().BeEquivalentTo(responseBytes, $"should always return the fixed bytes regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }
  

    [Fact]
    public async Task EchoServer_Should_Echo_Multiple_Messages()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/echo");
        await client.ConnectAsync(uri, cancelationToken);

        var testMessages = new[] { "Hello", "World", "WebSocket", "Test" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: cancelationToken);

            var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);

            received.Should().Be(testMessage, $"message '{testMessage}' should be echoed back");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task EchoServer_Should_Echo_Binary_Messages()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/echo");
        await client.ConnectAsync(uri, cancelationToken);

        var testData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        await client.SendAsync(new ArraySegment<byte>(testData), WebSocketMessageType.Binary, true, cancelationToken);

        var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: cancelationToken);

        // Assert
        receivedData.Should().BeEquivalentTo(testData);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task EchoServer_Should_Handle_Empty_Messages()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/echo");
        await client.ConnectAsync(uri, cancelationToken);

        // Act
        await client.SendAsync(string.Empty, cancellationToken: cancelationToken);

        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancelationToken);

        // Assert
        result.Count.Should().Be(0);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task CustomHandler_Should_Handle_Help_Command()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/chat");
        await client.ConnectAsync(uri, cancelationToken);

        // Act
        await client.SendAsync("/help", cancellationToken: cancelationToken);

        var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);

        // Assert
        received.Should().Contain("Available commands");
        received.Should().Contain("/help");
        received.Should().Contain("/time");
        received.Should().Contain("/echo");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task CustomHandler_Should_Handle_Multiple_Commands_In_Sequence()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
        var uri = new Uri($"{server.Url}/ws/chat");
        await client.ConnectAsync(uri, cancelationToken);

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
            await client.SendAsync(command, cancellationToken: cancelationToken);

            var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);

            assertion(received);
        }

        await client.SendAsync("/close", cancellationToken: cancelationToken);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task WhenMessage_Should_Handle_Multiple_Conditions_Fluently()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
                    .WhenMessage("/help").ThenSendMessage(m => m.WithText("Available commands"))
                    .WhenMessage("/time").ThenSendMessage(m => m.WithText($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"))
                    .WhenMessage("/echo *").ThenSendMessage(m => m.WithText("echo response"))
                    .WhenMessage(new ExactMatcher("/exact")).ThenSendMessage(m => m.WithText("is exact"))
                    .WhenMessage(new FuncMatcher(s => s == "/func")).ThenSendMessage(m => m.WithText("is func"))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/conditional");
        await client.ConnectAsync(uri, cancelationToken);

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
            await client.SendAsync(message, cancellationToken: cancelationToken);

            var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);

            received.Should().Contain(expectedContains, $"message '{message}' should return response containing '{expectedContains}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }

    [Fact]
    public async Task WhenMessage_NoMatch_Should_Return404()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/test")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WhenMessage("/close")
                    .ThenSendMessage(m => m.WithText("Closing connection")
                    .AndClose()
                ))
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/test");
        await client.ConnectAsync(uri, cancelationToken);

        // Act
        await client.SendAsync("/close", cancellationToken: cancelationToken);

        var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);

        // Assert
        received.Should().Contain("Closing connection");

        // Try to receive again - this will complete the close handshake
        // and update the client state to Closed
        try
        {
            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancelationToken);

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
    public async Task WhenMessage_Should_Close_Connection_When_AndClose_Is_Used()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
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
                    .WhenMessage("/close")
                    .ThenSendMessage(m => m.WithText("Closing connection")
                    .AndClose()
                ))
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/close");
        await client.ConnectAsync(uri, cancelationToken);

        // Act
        await client.SendAsync("/close", cancellationToken: cancelationToken);

        var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);

        // Assert
        received.Should().Contain("Closing connection");

        // Try to receive again - this will complete the close handshake
        // and update the client state to Closed
        try
        {
            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancelationToken);
            
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
    public async Task WithTransformer_Should_Transform_Message_Using_Handlebars()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/transform")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .SendMessage(m => m.WithText("{{request.Path}} {{[String.Lowercase] message.Text}}"))
                )
                .WithTransformer()
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/transform");

        // Act
        await client.ConnectAsync(uri, cancelationToken);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "HellO";
        await client.SendAsync(testMessage, cancellationToken: cancelationToken);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: cancelationToken);
        received.Should().Be("/ws/transform hello");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", cancelationToken);
    }
}