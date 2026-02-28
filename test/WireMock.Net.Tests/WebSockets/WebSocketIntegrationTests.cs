// Copyright © WireMock.Net

using System.Net.WebSockets;
using WireMock.Matchers;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests.WebSockets;

[Collection(nameof(WebSocketIntegrationTests))]
public class WebSocketIntegrationTests(ITestOutputHelper output, ITestContextAccessor testContext)
{
    private readonly CancellationToken _ct = testContext.Current.CancellationToken;

    [Fact]
    public async Task WithNoSetupShouldJustWaitForClose_AndCLose_When_ClientCloses()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/x")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(_ => { })
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/x");

        // Act
        await client.ConnectAsync(uri, _ct);
        client.State.Should().Be(WebSocketState.Open);

        // Assert
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

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
        var uri = new Uri($"{server.Url}/ws/echo");

        // Act
        await client.ConnectAsync(uri, _ct);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Hello, WebSocket!";
        await client.SendAsync(testMessage, cancellationToken: _ct);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);
        received.Should().Be(testMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
        var uri = new Uri($"{server.Url}/ws/message");

        // Act
        await client.ConnectAsync(uri, _ct);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage, cancellationToken: _ct);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);
        received.Should().Be(responseMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
        var uri = new Uri($"{server.Url}/ws/message");
        await client.ConnectAsync(uri, _ct);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: _ct);

            var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);
            received.Should().Be(responseMessage, $"should always return the fixed response regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task WithBinary_Should_Send_Configured_Bytes()
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
                    .SendMessage(m => m.WithBinary(responseBytes))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/binary");

        // Act
        await client.ConnectAsync(uri, _ct);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "Any message from client";
        await client.SendAsync(testMessage, cancellationToken: _ct);

        // Assert
        var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: _ct);
        receivedData.Should().BeEquivalentTo(responseBytes);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task WithBinary_Should_Send_Same_Bytes_For_Multiple_Messages()
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
                    .SendMessage(m => m.WithBinary(responseBytes))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/binary");
        await client.ConnectAsync(uri, _ct);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: _ct);

            var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: _ct);
            receivedData.Should().BeEquivalentTo(responseBytes, $"should always return the fixed bytes regardless of input message '{testMessage}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
        var uri = new Uri($"{server.Url}/ws/echo");
        await client.ConnectAsync(uri, _ct);

        var testMessages = new[] { "Hello", "World", "WebSocket", "Test" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: _ct);

            var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);

            received.Should().Be(testMessage, $"message '{testMessage}' should be echoed back");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
        var uri = new Uri($"{server.Url}/ws/echo");
        await client.ConnectAsync(uri, _ct);

        var testData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        await client.SendAsync(new ArraySegment<byte>(testData), WebSocketMessageType.Binary, true, _ct);

        var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: _ct);

        // Assert
        receivedData.Should().BeEquivalentTo(testData);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
        var uri = new Uri($"{server.Url}/ws/echo");
        await client.ConnectAsync(uri, _ct);

        // Act
        await client.SendAsync(string.Empty, cancellationToken: _ct);

        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _ct);

        // Assert
        result.Count.Should().Be(0);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
        var uri = new Uri($"{server.Url}/ws/chat");
        await client.ConnectAsync(uri, _ct);

        // Act
        await client.SendAsync("/help", cancellationToken: _ct);

        var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);

        // Assert
        received.Should().Contain("Available commands");
        received.Should().Contain("/help");
        received.Should().Contain("/time");
        received.Should().Contain("/echo");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
                                await context.SendAsync("Available commands: /help, /time, /echo <text>, /upper <text>, /reverse <text>", _ct);
                            }
                            else if (text.StartsWith("/time"))
                            {
                                await context.SendAsync($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC", _ct);
                            }
                            else if (text.StartsWith("/echo "))
                            {
                                await context.SendAsync(text.Substring(6), _ct);
                            }
                            else if (text.StartsWith("/upper "))
                            {
                                await context.SendAsync(text.Substring(7).ToUpper(), _ct);
                            }
                            else if (text.StartsWith("/reverse "))
                            {
                                var toReverse = text.Substring(9);
                                var reversed = new string(toReverse.Reverse().ToArray());
                                await context.SendAsync(reversed, _ct);
                            }
                            else if (text.StartsWith("/close"))
                            {
                                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", _ct);
                            }
                        }
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/chat");
        await client.ConnectAsync(uri, _ct);

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
            await client.SendAsync(command, cancellationToken: _ct);

            var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);

            assertion(received);
        }

        await client.SendAsync("/close", cancellationToken: _ct);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
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
                    .WhenMessage("/help").ThenSendMessage(m => m.WithText("Available commands"))
                    .WhenMessage("/time").ThenSendMessage(m => m.WithText($"Server time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"))
                    .WhenMessage("/echo *").ThenSendMessage(m => m.WithText("echo response"))
                    .WhenMessage(new ExactMatcher("/exact")).ThenSendMessage(m => m.WithText("is exact"))
                    .WhenMessage(new FuncMatcher(s => s == "/func")).ThenSendMessage(m => m.WithText("is func"))
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/conditional");
        await client.ConnectAsync(uri, _ct);

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
            await client.SendAsync(message, cancellationToken: _ct);

            var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);

            received.Should().Contain(expectedContains, $"message '{message}' should return response containing '{expectedContains}'");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Request_NoMatch_OnPath_Should_Return404()
    {
        // Arrange
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
                    .WhenMessage("/test")
                    .ThenSendMessage(m => m.WithText("Test")
                ))
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/abc");

        // Act
        Func<Task> connectAction = () => client.ConnectAsync(uri, _ct);

        // Assert
        (await connectAction.Should().ThrowAsync<WebSocketException>())
            .WithMessage("The server returned status code '404' when status code '101' was expected.");
    }

    [Fact]
    public async Task Request_NoMatch_OnMessageText_Should_ThrowException()
    {
        // Arrange
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
                    .WithCloseTimeout(TimeSpan.FromSeconds(3))
                    .WhenMessage("/test")
                    .ThenSendMessage(m => m.WithText("Test")
                ))
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/test");

        await client.ConnectAsync(uri, _ct);
        await client.SendAsync("/abc", cancellationToken: _ct);

        // Act
        Func<Task> receiveAction = () => client.ReceiveAsTextAsync(cancellationToken: _ct);

        // Assert
        (await receiveAction.Should().ThrowAsync<WebSocketException>())
            .WithMessage("The remote party closed the WebSocket connection without completing the close handshake.");
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
                    .WhenMessage("/close")
                    .ThenSendMessage(m => m.WithText("Closing connection")
                    .AndClose()
                ))
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/close");
        await client.ConnectAsync(uri, _ct);

        // Act
        await client.SendAsync("/close", cancellationToken: _ct);

        var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);

        // Assert
        received.Should().Contain("Closing connection");

        // Try to receive again - this will complete the close handshake
        // and update the client state to Closed
        try
        {
            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _ct);

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
        await client.ConnectAsync(uri, _ct);
        client.State.Should().Be(WebSocketState.Open);

        var testMessage = "HellO";
        await client.SendAsync(testMessage, cancellationToken: _ct);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);
        received.Should().Be("/ws/transform hello");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task WithWebSocketProxy_Should_Proxy_Multiple_TextMessages()
    {
        // Arrange - Start target echo server
        using var exampleEchoServer = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        exampleEchoServer
            .Given(Request.Create()
                .WithPath("/ws/target")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        // Arrange - Start proxy server
        using var sut = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        sut
            .Given(Request.Create()
                .WithPath("/ws/proxy")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocketProxy($"{exampleEchoServer.Url}/ws/target")
            );

        using var client = new ClientWebSocket();
        var proxyUri = new Uri($"{sut.Url}/ws/proxy");
        await client.ConnectAsync(proxyUri, _ct);

        await Task.Delay(500, _ct);

        var testMessages = new[] { "First", "Second", "Third" };

        // Act & Assert
        foreach (var testMessage in testMessages)
        {
            await client.SendAsync(testMessage, cancellationToken: _ct);

            var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);
            received.Should().Be(testMessage, $"message '{testMessage}' should be proxied and echoed back");
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task WithWebSocketProxy_Should_Proxy_Binary_Messages()
    {
        // Arrange - Start target echo server
        using var exampleEchoServer = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        exampleEchoServer
            .Given(Request.Create()
                .WithPath("/ws/target")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws.WithEcho())
            );

        // Arrange - Start proxy server
        using var sut = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        sut
            .Given(Request.Create()
                .WithPath("/ws/proxy")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocketProxy($"{exampleEchoServer.Url}/ws/target")
            );

        using var client = new ClientWebSocket();
        var proxyUri = new Uri($"{sut.Url}/ws/proxy");
        await client.ConnectAsync(proxyUri, _ct);

        await Task.Delay(250, _ct);

        var testData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        await client.SendAsync(new ArraySegment<byte>(testData), WebSocketMessageType.Binary, true, _ct);

        var receivedData = await client.ReceiveAsBytesAsync(cancellationToken: _ct);

        await Task.Delay(250, _ct);

        // Assert
        receivedData.Should().BeEquivalentTo(testData, "binary data should be proxied and echoed back");

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Send_TextMessage_To_Multiple_Connected_Clients()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastMessage = "Broadcast to all clients";

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;

                            if (text == "register")
                            {
                                await context.SendAsync($"Registered: {context.ConnectionId}");
                            }
                            else if (text.StartsWith("broadcast:"))
                            {
                                var broadcastText = text.Substring(10);
                                await context.BroadcastAsync(broadcastText);
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();
        using var client3 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast");

        // Act
        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);
        await client3.ConnectAsync(uri, _ct);

        await Task.Delay(500, _ct);

        await client1.SendAsync("register", cancellationToken: _ct);
        await client2.SendAsync("register", cancellationToken: _ct);
        await client3.SendAsync("register", cancellationToken: _ct);

        // Receive registration confirmations
        var reg1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var reg2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);
        var reg3 = await client3.ReceiveAsTextAsync(cancellationToken: _ct);

        reg1.Should().StartWith("Registered: ");
        reg2.Should().StartWith("Registered: ");
        reg3.Should().StartWith("Registered: ");

        // Send broadcast from client1
        await client1.SendAsync($"broadcast:{broadcastMessage}", cancellationToken: _ct);

        // Assert - all clients should receive the broadcast
        var received1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);
        var received3 = await client3.ReceiveAsTextAsync(cancellationToken: _ct);

        received1.Should().Be(broadcastMessage);
        received2.Should().Be(broadcastMessage);
        received3.Should().Be(broadcastMessage);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client3.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Send_BinaryMessage_To_Multiple_Connected_Clients()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var message = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var broadcastMessageFromWireMock = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text && message.Text == "register")
                        {
                            await context.SendAsync($"Registered: {context.ConnectionId}");
                        }

                        if (message.MessageType == WebSocketMessageType.Binary)
                        {
                            await context.BroadcastAsync(broadcastMessageFromWireMock);
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();
        using var client3 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);
        await client3.ConnectAsync(uri, _ct);

        await Task.Delay(500, _ct);

        await client1.SendAsync("register", cancellationToken: _ct);
        await client2.SendAsync("register", cancellationToken: _ct);
        await client3.SendAsync("register", cancellationToken: _ct);

        // Receive registration confirmations
        var reg1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var reg2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);
        var reg3 = await client3.ReceiveAsTextAsync(cancellationToken: _ct);

        reg1.Should().StartWith("Registered: ");
        reg2.Should().StartWith("Registered: ");
        reg3.Should().StartWith("Registered: ");

        // Send broadcast from client1
        await client1.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Binary, true, cancellationToken: _ct);

        // Assert - all clients should receive the broadcast
        var received1 = await client1.ReceiveAsBytesAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsBytesAsync(cancellationToken: _ct);
        var received3 = await client3.ReceiveAsBytesAsync(cancellationToken: _ct);

        received1.Should().BeEquivalentTo(broadcastMessageFromWireMock);
        received2.Should().BeEquivalentTo(broadcastMessageFromWireMock);
        received3.Should().BeEquivalentTo(broadcastMessageFromWireMock);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client3.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Handle_Multiple_Broadcast_Messages()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-multi")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            await context.BroadcastAsync(text);
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast-multi");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        var messages = new[] { "Message 1", "Message 2", "Message 3" };

        // Act & Assert
        foreach (var message in messages)
        {
            await client1.SendAsync(message, cancellationToken: _ct);

            var received1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
            var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

            received1.Should().Be(message);
            received2.Should().Be(message);
        }

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Exclude_Sender_When_ExcludeSender_Is_True()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-exclude")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;

                            if (text.StartsWith("send:"))
                            {
                                var broadcastText = text.Substring(5);
                                await context.BroadcastAsync(broadcastText, excludeSender: true);
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast-exclude");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        var broadcastMessage = "Exclusive broadcast";

        // Act
        await client1.SendAsync($"send:{broadcastMessage}", cancellationToken: _ct);

        // Assert - only client2 should receive the message
        var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);
        received2.Should().Be(broadcastMessage);

        // client1 should not receive anything (or should timeout)
        var receiveTask1 = client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var delayTask = Task.Delay(500, _ct);

        var completedTask = await Task.WhenAny(receiveTask1, delayTask);
        completedTask.Should().Be(delayTask, "client1 should not receive the exclusive broadcast");

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Work_With_Single_Client()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastMessage = "Single client broadcast";

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-single")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            await context.BroadcastAsync(text);
                        }
                    })
                )
            );

        using var client = new ClientWebSocket();
        var uri = new Uri($"{server.Url}/ws/broadcast-single");

        // Act
        await client.ConnectAsync(uri, _ct);
        await client.SendAsync(broadcastMessage, cancellationToken: _ct);

        // Assert
        var received = await client.ReceiveAsTextAsync(cancellationToken: _ct);
        received.Should().Be(broadcastMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Handle_Client_Disconnect_During_Broadcast()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastMessage = "Message after disconnect";

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-disconnect")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            await context.BroadcastAsync(text);
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast-disconnect");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        // Act - disconnect client1
        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", _ct);

        await Task.Delay(500, _ct);

        // Send broadcast from client2 - should handle disconnected client gracefully
        await client2.SendAsync(broadcastMessage, cancellationToken: _ct);

        // Assert - client2 should still receive the broadcast
        var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);
        received2.Should().Be(broadcastMessage);

        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }

    [Fact]
    public async Task Broadcast_Should_Support_Targeted_Broadcasting_Based_On_Condition()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-conditional")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;

                            if (text.StartsWith("to-admins:"))
                            {
                                var adminMessage = text.Substring(10);
                                await context.SendAsync($"Admin broadcast: {adminMessage}");
                            }
                            else if (text.StartsWith("to-all:"))
                            {
                                var allMessage = text.Substring(7);
                                await context.BroadcastAsync(allMessage);
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast-conditional");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        // Act
        await client1.SendAsync("to-all:General message", cancellationToken: _ct);

        // Assert - both clients receive the broadcast
        var received1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        received1.Should().Be("General message");
        received2.Should().Be("General message");

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
    }
}