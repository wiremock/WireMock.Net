// Copyright © WireMock.Net

using System.Net.WebSockets;
using System.Text;

namespace WireMock.Net.Tests.WebSockets;

internal static class ClientWebSocketExtensions
{
    internal static Task SendAsync(this ClientWebSocket client, string text, bool endOfMessage = true, CancellationToken cancellationToken = default)
    {
        return client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)), WebSocketMessageType.Text, endOfMessage, cancellationToken);
    }

    internal static async Task<string> ReceiveAsTextAsync(this ClientWebSocket client, CancellationToken cancellationToken = default)
    {
        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken);

        if (result.MessageType != WebSocketMessageType.Text)
        {
            throw new InvalidOperationException($"Expected a text message but received a {result.MessageType} message.");
        }

        if (!result.EndOfMessage)
        {
            throw new InvalidOperationException("Received message is too large for the buffer. Consider increasing the buffer size.");
        }

        return Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
    }
}