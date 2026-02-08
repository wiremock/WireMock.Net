#if !NET452
// Copyright © WireMock.Net

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests.WebSockets;

public class WebSocketTests
{
    [Fact]
    public async Task WebSocket_EchoHandler_Should_EchoMessages()
    {
        // Arrange
        var server = WireMockServer.Start();
        
        server
            .Given(Request.Create()
                //.WithPath("/echo")
                .WithWebSocketPath("/echo")
            )
            .RespondWith(Response.Create()
                .WithWebSocketHandler(async ctx =>
                {
                    var buffer = new byte[1024 * 4];
                    
                    while (ctx.WebSocket.State == WebSocketState.Open)
                    {
                        var result = await ctx.WebSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ctx.WebSocket.CloseAsync(
                                WebSocketCloseStatus.NormalClosure,
                                "Closing",
                                CancellationToken.None);
                        }
                        else
                        {
                            await ctx.WebSocket.SendAsync(
                                new ArraySegment<byte>(buffer, 0, result.Count),
                                result.MessageType,
                                result.EndOfMessage,
                                CancellationToken.None);
                        }
                    }
                })
            );

        // Act
        using var client = new ClientWebSocket();
        await client.ConnectAsync(
            new Uri($"ws://{server.Url}/echo"),
            CancellationToken.None);

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

        // Assert
        var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Assert.Equal("Hello WebSocket!", response);

        server.Stop();
    }
}
#endif