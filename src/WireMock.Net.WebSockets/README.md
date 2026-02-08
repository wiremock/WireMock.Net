# WireMock.Net WebSocket Support

This package adds WebSocket mocking capabilities to WireMock.Net, enabling you to mock real-time WebSocket connections for testing purposes.

## Features

- **Simple Fluent API** - Consistent with WireMock.Net's builder pattern
- **Multiple Handler Types** - Raw WebSocket, context-based, and message-based handlers
- **Subprotocol Support** - Negotiate WebSocket subprotocols
- **Keep-Alive** - Configure heartbeat intervals
- **Binary & Text Messages** - Handle both message types
- **Message Routing** - Route based on message type or content
- **Connection Lifecycle** - Full control over connection handling

## Installation

```bash
dotnet add package WireMock.Net
```

The WebSocket support is included in the main WireMock.Net package for .NET Core 3.1+.

## Quick Start

### Basic Echo Server

```csharp
var server = WireMockServer.Start();

server
    .Given(Request.Create()
        .WithPath("/echo")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            var buffer = new byte[1024 * 4];
            var result = await ctx.WebSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);

            await ctx.WebSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, result.Count),
                result.MessageType,
                result.EndOfMessage,
                CancellationToken.None);
        })
    );

// Connect and test
using var client = new ClientWebSocket();
await client.ConnectAsync(
    new Uri($"ws://localhost:{server.Port}/echo"),
    CancellationToken.None);
```

## API Reference

### Request Matching

#### WithWebSocketPath(string path)

Match WebSocket connections to a specific path:

```csharp
.Given(Request.Create()
    .WithPath("/notifications")
)
```

#### WithWebSocketSubprotocol(params string[] subProtocols)

Match specific WebSocket subprotocols:

```csharp
.Given(Request.Create()
    .WithPath("/chat")
    .WithHeader("Sec-WebSocket-Protocol", "chat")
)
```

#### WithCustomHandshakeHeaders(params (string, string)[] headers)

Validate custom headers during WebSocket handshake:

```csharp
.Given(Request.Create()
    .WithPath("/secure-ws")
    .WithHeader("Authorization", "Bearer token123")
)
```

### Response Building

#### WithWebSocketHandler(Func<WebSocketHandlerContext, Task> handler)

Set a handler that receives the full connection context:

```csharp
.RespondWith(Response.Create()
    .WithWebSocketHandler(async ctx =>
    {
        // ctx.WebSocket - the WebSocket instance
        // ctx.RequestMessage - the upgrade request
        // ctx.Headers - request headers
        // ctx.SubProtocol - negotiated subprotocol
        // ctx.UserState - custom state dictionary
    })
)
```

#### WithWebSocketHandler(Func<WebSocket, Task> handler)

Set a simpler handler with just the WebSocket:

```csharp
.RespondWith(Response.Create()
    .WithWebSocketHandler(async ws =>
    {
        // Direct WebSocket access
    })
)
```

#### WithWebSocketMessageHandler(Func<WebSocketMessage, Task<WebSocketMessage?>> handler)

Use message-based routing for structured communication:

```csharp
.RespondWith(Response.Create()
    .WithWebSocketMessageHandler(async msg =>
    {
        return msg.Type switch
        {
            "subscribe" => new WebSocketMessage { Type = "subscribed", TextData = "..." },
            "ping" => new WebSocketMessage { Type = "pong", TextData = "..." },
            _ => null
        };
    })
)
```

#### WithWebSocketKeepAlive(TimeSpan interval)

Configure keep-alive heartbeat interval:

```csharp
.RespondWith(Response.Create()
    .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))
)
```

#### WithWebSocketTimeout(TimeSpan timeout)

Set connection timeout:

```csharp
.RespondWith(Response.Create()
    .WithWebSocketTimeout(TimeSpan.FromMinutes(5))
)
```

#### WithWebSocketMessage(WebSocketMessage message)

Send a specific message immediately upon connection:

```csharp
.RespondWith(Response.Create()
    .WithWebSocketMessage(new WebSocketMessage
    {
        Type = "connected",
        TextData = "{\"status\":\"connected\"}"
    })
)
```

## Examples

### Server-Initiated Notifications

```csharp
server
    .Given(Request.Create().WithPath("/notifications"))
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            while (ctx.WebSocket.State == WebSocketState.Open)
            {
                var notification = Encoding.UTF8.GetBytes("{\"event\":\"update\"}");
                await ctx.WebSocket.SendAsync(
                    new ArraySegment<byte>(notification),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                await Task.Delay(5000);
            }
        })
        .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))
    );
```

### Message Type Routing

```csharp
server
    .Given(Request.Create().WithPath("/api/v1"))
    .RespondWith(Response.Create()
        .WithWebSocketMessageHandler(async msg =>
        {
            if (string.IsNullOrEmpty(msg.Type))
                return null;

            return msg.Type switch
            {
                "subscribe" => HandleSubscribe(msg),
                "publish" => HandlePublish(msg),
                "unsubscribe" => HandleUnsubscribe(msg),
                "ping" => new WebSocketMessage { Type = "pong", TextData = "" },
                _ => new WebSocketMessage { Type = "error", TextData = "Unknown command" }
            };
        })
    );
```

### Authenticated WebSocket

```csharp
server
    .Given(Request.Create()
        .WithPath("/secure")
        .WithHeader("Authorization", "Bearer valid-token")
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            // Only authenticated connections reach here
            var token = ctx.Headers["Authorization"][0];
            await SendWelcomeAsync(ctx.WebSocket, token);
        })
    );
```

### Binary Data Streaming

```csharp
server
    .Given(Request.Create().WithPath("/stream"))
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx =>
        {
            for (int i = 0; i < 100; i++)
            {
                var data = BitConverter.GetBytes(i);
                await ctx.WebSocket.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Binary,
                    true,
                    CancellationToken.None);
            }
        })
    );
```

## WebSocketMessage Class

```csharp
public class WebSocketMessage
{
    public string Type { get; set; }                  // Message type identifier
    public DateTime Timestamp { get; set; }           // Creation timestamp
    public object? Data { get; set; }                 // Arbitrary data
    public bool IsBinary { get; set; }                // Binary or text message
    public byte[]? RawData { get; set; }              // Raw binary data
    public string? TextData { get; set; }             // Text content
}
```

## WebSocketHandlerContext Class

```csharp
public class WebSocketHandlerContext
{
    public WebSocket WebSocket { get; init; }                              // WebSocket instance
    public IRequestMessage RequestMessage { get; init; }                   // Upgrade request
    public IDictionary<string, string[]> Headers { get; init; }            // Request headers
    public string? SubProtocol { get; init; }                              // Negotiated subprotocol
    public IDictionary<string, object> UserState { get; init; }            // Custom state storage
}
```

## Limitations and Notes

- WebSocket support is available for .NET Core 3.1 and later
- When using `WithWebSocketHandler`, the middleware pipeline must remain active for the duration of the connection
- Always properly close WebSocket connections using `CloseAsync()`
- Keep-alive intervals should be appropriate for your use case (typically 15-60 seconds)
- Binary messages require `IsBinary = true` on the message

## Integration with WireMock.Net Features

WebSocket mappings work with:
- Request path matching
- Header validation
- Probability-based responses
- Mapping priority
- Admin interface (list, reset, etc.)

However, these features are **not** supported:
- Body matching (WebSockets don't have HTTP bodies)
- Response transformers (yet)
- Proxy mode (yet)

## Thread Safety

All handlers are executed on a single thread per connection. Multiple concurrent connections are handled independently and safely.

## Performance Considerations

- Each WebSocket connection maintains an active task
- For long-lived connections, implement proper keep-alive
- Use `WithWebSocketTimeout()` to prevent zombie connections
- Consider connection limits in server configuration

## Contributing

Contributions are welcome! Please see the main WireMock.Net repository for guidelines.

## License

MIT License - See LICENSE file in the repository
