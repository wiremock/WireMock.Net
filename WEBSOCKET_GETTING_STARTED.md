# WireMock.Net WebSocket - Getting Started Guide

## Quick Start

### Installation

The WebSocket support is included in WireMock.Net for .NET Core 3.1+:

```bash
dotnet add package WireMock.Net
```

### Basic Echo WebSocket

```csharp
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

// Start the server
var server = WireMockServer.Start();

// Configure WebSocket endpoint
server
    .Given(Request.Create()
        .WithPath("/echo")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            using (ctx.WebSocket)
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
                        // Echo back
                        await ctx.WebSocket.SendAsync(
                            new ArraySegment<byte>(buffer, 0, result.Count),
                            result.MessageType,
                            result.EndOfMessage,
                            CancellationToken.None);
                    }
                }
            }
        })
    );

// Connect to it
using var client = new ClientWebSocket();
await client.ConnectAsync(
    new Uri($"ws://localhost:{server.Port}/echo"),
    CancellationToken.None);

// Send a message
var message = Encoding.UTF8.GetBytes("Hello!");
await client.SendAsync(
    new ArraySegment<byte>(message),
    WebSocketMessageType.Text,
    true,
    CancellationToken.None);

// Receive echo
var buffer = new byte[1024];
var received = await client.ReceiveAsync(
    new ArraySegment<byte>(buffer),
    CancellationToken.None);

var response = Encoding.UTF8.GetString(buffer, 0, received.Count);
Console.WriteLine($"Received: {response}"); // Output: Hello!

server.Stop();
```

## Common Patterns

### 1. Authenticated WebSocket

```csharp
server
    .Given(Request.Create()
        .WithPath("/secure")
        .WithHeader("Authorization", "Bearer my-token")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            // Authenticated - proceed
            var msg = Encoding.UTF8.GetBytes("{\"status\":\"authenticated\"}");
            await ctx.WebSocket.SendAsync(
                new ArraySegment<byte>(msg),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        })
    );
```

### 2. Subprotocol Matching

```csharp
server
    .Given(Request.Create()
        .WithPath("/chat")
        .WithHeader("Sec-WebSocket-Protocol", "chat")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            // Handle chat protocol
        })
    );
```

### 3. Server-Initiated Messages

```csharp
server
    .Given(Request.Create()
        .WithPath("/notifications")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            while (ctx.WebSocket.State == WebSocketState.Open)
            {
                // Send heartbeat every 5 seconds
                var heartbeat = Encoding.UTF8.GetBytes("{\"type\":\"ping\"}");
                await ctx.WebSocket.SendAsync(
                    new ArraySegment<byte>(heartbeat),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                await Task.Delay(5000);
            }
        })
        .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))
    );
```

### 4. Message-Based Routing

```csharp
server
    .Given(Request.Create()
        .WithPath("/api/v1")
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
                    TextData = "{\"id\":123}"
                },
                "unsubscribe" => new WebSocketMessage
                {
                    Type = "unsubscribed",
                    TextData = "{\"id\":123}"
                },
                "ping" => new WebSocketMessage
                {
                    Type = "pong",
                    TextData = ""
                },
                _ => null  // No response
            };
        })
    );
```

### 5. Binary Messages

```csharp
server
    .Given(Request.Create()
        .WithPath("/binary")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            var buffer = new byte[1024];
            var result = await ctx.WebSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Binary)
            {
                // Process binary data
                var binaryData = buffer.AsSpan(0, result.Count);
                // ... process ...
            }
        })
    );
```

### 6. Data Streaming

```csharp
server
    .Given(Request.Create()
        .WithPath("/stream")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            for (int i = 0; i < 100; i++)
            {
                var data = Encoding.UTF8.GetBytes(
                    $"{{\"index\":{i},\"data\":\"Item {i}\"}}");
                
                await ctx.WebSocket.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                await Task.Delay(100);
            }

            await ctx.WebSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Stream complete",
                CancellationToken.None);
        })
        .WithWebSocketTimeout(TimeSpan.FromMinutes(5))
    );
```

## API Reference

### Response Builder Methods

#### `WithWebSocketHandler(Func<WebSocketHandlerContext, Task> handler)`

Sets a handler with full context access:
- `ctx.WebSocket` - The WebSocket instance
- `ctx.RequestMessage` - The HTTP upgrade request
- `ctx.Headers` - Request headers
- `ctx.SubProtocol` - Negotiated subprotocol
- `ctx.UserState` - Custom state dictionary

#### `WithWebSocketHandler(Func<WebSocket, Task> handler)`

Sets a simplified handler with just the WebSocket.

#### `WithWebSocketMessageHandler(Func<WebSocketMessage, Task<WebSocketMessage?>> handler)`

Sets a message-based handler for structured communication. Return `null` to send no response.

#### `WithWebSocketKeepAlive(TimeSpan interval)`

Sets keep-alive heartbeat interval (default: 30 seconds).

#### `WithWebSocketTimeout(TimeSpan timeout)`

Sets connection timeout (default: 5 minutes).

#### `WithWebSocketMessage(WebSocketMessage message)`

Sends a specific message upon connection.

### Request Builder Methods

#### `WithWebSocketPath(string path)`

Matches WebSocket connections to a specific path.

#### `WithWebSocketSubprotocol(params string[] subProtocols)`

Matches specific WebSocket subprotocols.

#### `WithCustomHandshakeHeaders(params (string Key, string Value)[] headers)`

Validates custom headers during WebSocket handshake.

## Testing WebSocket Mocks

### Using ClientWebSocket

```csharp
[Fact]
public async Task MyWebSocketTest()
{
    var server = WireMockServer.Start();
    
    // Configure mock...
    
    using var client = new ClientWebSocket();
    await client.ConnectAsync(
        new Uri($"ws://localhost:{server.Port}/path"),
        CancellationToken.None);
    
    // Send/receive messages...
    
    server.Stop();
}
```

### With xUnit

```csharp
public class WebSocketTests : IAsyncLifetime
{
    private WireMockServer? _server;

    public async Task InitializeAsync()
    {
        _server = WireMockServer.Start();
        // Configure mappings...
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _server?.Stop();
        _server?.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task WebSocket_ShouldEchoMessages()
    {
        // Test implementation...
    }
}
```

## Troubleshooting

### Connection Refused

Ensure the server is started before attempting to connect:

```csharp
var server = WireMockServer.Start();
Assert.True(server.IsStarted);  // Verify before use
```

### Timeout Issues

Increase the timeout if handling slow operations:

```csharp
.WithWebSocketTimeout(TimeSpan.FromMinutes(10))
```

### Message Not Received

Ensure `EndOfMessage` is set to `true` when sending:

```csharp
await webSocket.SendAsync(
    new ArraySegment<byte>(data),
    WebSocketMessageType.Text,
    true,  // Must be true
    cancellationToken);
```

### Keep-Alive Not Working

Ensure keep-alive interval is shorter than client timeout:

```csharp
// Client timeout: 5 minutes (default)
// Keep-alive: 30 seconds (default)
.WithWebSocketKeepAlive(TimeSpan.FromSeconds(20))  // Less than client timeout
```

## Performance Tips

1. **Close connections properly** - Always close WebSockets when done
2. **Set appropriate timeouts** - Prevent zombie connections
3. **Handle exceptions gracefully** - Use try-catch in handlers
4. **Limit message size** - Process large messages in chunks
5. **Use keep-alive** - For long-idle connections

## Limitations

⚠️ WebSocket support requires .NET Core 3.1 or later  
⚠️ HTTPS/WSS requires certificate configuration  
⚠️ Message processing is sequential per connection  
⚠️ Binary messages larger than buffer need streaming  

## Additional Resources

- [WebSocket RFC 6455](https://tools.ietf.org/html/rfc6455)
- [System.Net.WebSockets Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.net.websockets)
- [WireMock.Net Documentation](https://github.com/WireMock-Net/WireMock.Net)

