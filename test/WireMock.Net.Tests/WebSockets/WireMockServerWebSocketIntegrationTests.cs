// Copyright © WireMock.Net

using System.Net.WebSockets;
using AwesomeAssertions;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests.WebSockets;

public class WireMockServerWebSocketIntegrationTests(ITestOutputHelper output, ITestContextAccessor testContext)
{
    private readonly CancellationToken _ct = testContext.Current.CancellationToken;

    [Fact]
    public async Task GetWebSocketConnections_Should_Return_All_Active_Connections()
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
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithEcho()
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();
        using var client3 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/test");

        // Act
        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);
        await client3.ConnectAsync(uri, _ct);

        // Assert
        var connections = server.GetWebSocketConnections();
        connections.Should().HaveCount(3);
        connections.Should().AllSatisfy(c => c.Should().NotBeNull());

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client3.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(300, _ct);
    }

    [Fact]
    public async Task GetWebSocketConnections_Should_Return_Empty_When_No_Connections()
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
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithEcho()
                )
            );

        // Act
        var connections = server.GetWebSocketConnections();

        // Assert
        connections.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWebSocketConnections_Should_Return_Connections_For_Specific_Mapping()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var mapping1Guid = Guid.NewGuid();
        var mapping2Guid = Guid.NewGuid();

        server
            .Given(Request.Create()
                .WithPath("/ws/echo1")
                .WithWebSocketUpgrade()
            )
            .WithGuid(mapping1Guid)
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithEcho()
                )
            );

        server
            .Given(Request.Create()
                .WithPath("/ws/echo2")
                .WithWebSocketUpgrade()
            )
            .WithGuid(mapping2Guid)
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithEcho()
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();
        using var client3 = new ClientWebSocket();

        var uri1 = new Uri($"{server.Url}/ws/echo1");
        var uri2 = new Uri($"{server.Url}/ws/echo2");

        // Act
        await client1.ConnectAsync(uri1, _ct);
        await client2.ConnectAsync(uri1, _ct);
        await client3.ConnectAsync(uri2, _ct);

        // Assert
        var allConnections = server.GetWebSocketConnections();
        allConnections.Should().HaveCount(3);

        var mapping1Connections = server.GetWebSocketConnections(mapping1Guid);
        mapping1Connections.Should().HaveCount(2);

        var mapping2Connections = server.GetWebSocketConnections(mapping2Guid);
        mapping2Connections.Should().HaveCount(1);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client3.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(300, _ct);
    }

    [Fact]
    public async Task AbortWebSocketConnectionAsync_Should_Close_Specific_Connection()
    {
        // Arrange
        var server = WireMockServer.Start(new WireMockServerSettings
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
                    .WithCloseTimeout(TimeSpan.FromSeconds(30))
                    .WithEcho()
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/test");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        var connections = server.GetWebSocketConnections();
        connections.Should().HaveCount(2);

        var connectionIdToAbort = connections.First().ConnectionId;

        // Act
        await server.AbortWebSocketConnectionAsync(connectionIdToAbort, "Abort by test", _ct);

        // Assert
        var remainingConnections = server.GetWebSocketConnections();
        remainingConnections.Should().HaveCount(1);
        var remainingConnection = remainingConnections.First();
        remainingConnection.ConnectionId.Should().NotBe(connectionIdToAbort);

        await Task.Delay(200, _ct);
    }

    [Fact]
    public async Task BroadcastToWebSocketsAsync_Should_Broadcast_Text_To_Specific_Mapping()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastMessage = "Server broadcast message";
        var mappingGuid = Guid.NewGuid();

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast")
                .WithWebSocketUpgrade()
            )
            .WithGuid(mappingGuid)
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            if (text.StartsWith("ready"))
                            {
                                await context.SendAsync("ready!");
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        // Signal ready
        await client1.SendAsync("ready", cancellationToken: _ct);
        await client2.SendAsync("ready", cancellationToken: _ct);

        var text1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var text2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        text1.Should().Be("ready!");
        text2.Should().Be("ready!");

        // Act
        await server.BroadcastToWebSocketsAsync(mappingGuid, broadcastMessage, cancellationToken: _ct);

        // Assert
        var received1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        received1.Should().Be(broadcastMessage);
        received2.Should().Be(broadcastMessage);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(200, _ct);
    }

    [Fact]
    public async Task BroadcastToWebSocketsAsync_Should_Broadcast_Binary_To_Specific_Mapping()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastData = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var mappingGuid = Guid.NewGuid();

        server
            .Given(Request.Create()
                .WithPath("/ws/broadcast-binary")
                .WithWebSocketUpgrade()
            )
            .WithGuid(mappingGuid)
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            if (text.StartsWith("ready"))
                            {
                                await context.SendAsync("ready!");
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/broadcast-binary");

        await client1.ConnectAsync(uri, _ct);
        await client2.ConnectAsync(uri, _ct);

        // Signal ready
        await client1.SendAsync("ready", cancellationToken: _ct);
        await client2.SendAsync("ready", cancellationToken: _ct);

        var text1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var text2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        text1.Should().Be("ready!");
        text2.Should().Be("ready!");

        // Act
        await server.BroadcastToWebSocketsAsync(mappingGuid, broadcastData, cancellationToken: _ct);

        // Assert
        var received1 = await client1.ReceiveAsBytesAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsBytesAsync(cancellationToken: _ct);

        received1.Should().BeEquivalentTo(broadcastData);
        received2.Should().BeEquivalentTo(broadcastData);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(200, _ct);
    }

    [Fact]
    public async Task BroadcastToAllWebSocketsAsync_Should_Broadcast_Text_To_All_Mappings()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastMessage = "Broadcast to all mappings";

        server
            .Given(Request.Create()
                .WithPath("/ws/mapping1")
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
                            if (text.StartsWith("ready"))
                            {
                                await context.SendAsync("ready!");
                            }
                        }
                    })
                )
            );

        server
            .Given(Request.Create()
                .WithPath("/ws/mapping2")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            if (text.StartsWith("ready"))
                            {
                                await context.SendAsync("ready!");
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri1 = new Uri($"{server.Url}/ws/mapping1");
        var uri2 = new Uri($"{server.Url}/ws/mapping2");

        await client1.ConnectAsync(uri1, _ct);
        await client2.ConnectAsync(uri2, _ct);

        // Signal ready
        await client1.SendAsync("ready", cancellationToken: _ct);
        await client2.SendAsync("ready", cancellationToken: _ct);

        var text1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var text2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        text1.Should().Be("ready!");
        text2.Should().Be("ready!");

        // Act
        await server.BroadcastToAllWebSocketsAsync(broadcastMessage, cancellationToken: _ct);

        // Assert - both clients from different mappings should receive the broadcast
        var received1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        received1.Should().Be(broadcastMessage);
        received2.Should().Be(broadcastMessage);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(200, _ct);
    }

    [Fact]
    public async Task BroadcastToAllWebSocketsAsync_Should_Broadcast_Binary_To_All_Mappings()
    {
        // Arrange
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(output),
            Urls = ["ws://localhost:0"]
        });

        var broadcastData = new byte[] { 0xAA, 0xBB, 0xCC };

        server
            .Given(Request.Create()
                .WithPath("/ws/mapping1")
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
                            if (text.StartsWith("ready"))
                            {
                                await context.SendAsync("ready!");
                            }
                        }
                    })
                )
            );

        server
            .Given(Request.Create()
                .WithPath("/ws/mapping2")
                .WithWebSocketUpgrade()
            )
            .RespondWith(Response.Create()
                .WithWebSocket(ws => ws
                    .WithMessageHandler(async (message, context) =>
                    {
                        if (message.MessageType == WebSocketMessageType.Text)
                        {
                            var text = message.Text ?? string.Empty;
                            if (text.StartsWith("ready"))
                            {
                                await context.SendAsync("ready!");
                            }
                        }
                    })
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri1 = new Uri($"{server.Url}/ws/mapping1");
        var uri2 = new Uri($"{server.Url}/ws/mapping2");

        await client1.ConnectAsync(uri1, _ct);
        await client2.ConnectAsync(uri2, _ct);

        // Signal ready
        await client1.SendAsync("ready", cancellationToken: _ct);
        await client2.SendAsync("ready", cancellationToken: _ct);

        var text1 = await client1.ReceiveAsTextAsync(cancellationToken: _ct);
        var text2 = await client2.ReceiveAsTextAsync(cancellationToken: _ct);

        text1.Should().Be("ready!");
        text2.Should().Be("ready!");

        // Act
        await server.BroadcastToAllWebSocketsAsync(broadcastData, cancellationToken: _ct);

        // Assert
        var received1 = await client1.ReceiveAsBytesAsync(cancellationToken: _ct);
        var received2 = await client2.ReceiveAsBytesAsync(cancellationToken: _ct);

        received1.Should().BeEquivalentTo(broadcastData);
        received2.Should().BeEquivalentTo(broadcastData);

        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);
        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(200, _ct);
    }

    [Fact]
    public async Task GetWebSocketConnections_Should_Update_After_Client_Disconnect()
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
                    .WithCloseTimeout(TimeSpan.FromSeconds(5))
                    .WithEcho()
                )
            );

        using var client1 = new ClientWebSocket();
        using var client2 = new ClientWebSocket();

        var uri = new Uri($"{server.Url}/ws/test");

        await client1.ConnectAsync(uri, _ct);
        await Task.Delay(100, _ct);
        await client2.ConnectAsync(uri, _ct);
        await Task.Delay(100, _ct);

        var initialConnections = server.GetWebSocketConnections();
        initialConnections.Should().HaveCount(2);

        // Act
        await client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", _ct);
        await Task.Delay(100, _ct);

        // Assert
        var remainingConnections = server.GetWebSocketConnections();
        remainingConnections.Should().HaveCount(1);

        await client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", _ct);

        await Task.Delay(200, _ct);
    }
}