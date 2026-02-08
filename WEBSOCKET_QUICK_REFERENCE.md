# WebSocket Implementation - Quick Reference Card

## Installation

```bash
dotnet add package WireMock.Net
```

No additional packages needed - WebSocket support is built-in for .NET Core 3.1+

## Minimum Example

```csharp
var server = WireMockServer.Start();

server.Given(Request.Create().WithPath("/ws"))
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx => {
            // Your handler code
        }));

// Connect and use
using var client = new ClientWebSocket();
await client.ConnectAsync(new Uri($"ws://localhost:{server.Port}/ws"), default);
```

## Request Matching

```csharp
Request.Create()
    .WithPath("/path")                          // Match path
    .WithWebSocketSubprotocol("chat")           // Match subprotocol
    .WithHeader("Authorization", "Bearer ...")  // Match headers
    .WithCustomHandshakeHeaders(                // Custom handshake validation
        ("X-Custom-Header", "value"))
```

## Response Configuration

```csharp
Response.Create()
    // Handler Options
    .WithWebSocketHandler(async ctx => {})                    // Full context
    .WithWebSocketHandler(async ws => {})                     // Just WebSocket
    .WithWebSocketMessageHandler(async msg => {})             // Message routing
    .WithWebSocketMessage(new WebSocketMessage { ... })       // Send on connect
    
    // Configuration
    .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))         // Heartbeat
    .WithWebSocketTimeout(TimeSpan.FromMinutes(5))            // Timeout
```

## Handler Patterns

### Echo Handler
```csharp
.WithWebSocketHandler(async ctx => {
    var buffer = new byte[1024 * 4];
    while (ctx.WebSocket.State == WebSocketState.Open) {
        var result = await ctx.WebSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), default);
        await ctx.WebSocket.SendAsync(
            new ArraySegment<byte>(buffer, 0, result.Count),
            result.MessageType, result.EndOfMessage, default);
    }
})
```

### Message Routing
```csharp
.WithWebSocketMessageHandler(async msg => msg.Type switch {
    "ping" => new WebSocketMessage { Type = "pong" },
    "subscribe" => new WebSocketMessage { Type = "subscribed" },
    _ => null
})
```

### Server Push
```csharp
.WithWebSocketHandler(async ctx => {
    while (ctx.WebSocket.State == WebSocketState.Open) {
        var data = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
        await ctx.WebSocket.SendAsync(
            new ArraySegment<byte>(data),
            WebSocketMessageType.Text, true, default);
        await Task.Delay(5000);
    }
})
```

## Handler Context

```csharp
ctx.WebSocket           // The WebSocket instance
ctx.RequestMessage      // The HTTP upgrade request
ctx.Headers            // Request headers as Dictionary
ctx.SubProtocol        // Negotiated subprotocol (string?)
ctx.UserState          // Custom state Dictionary<string, object>
```

## WebSocketMessage

```csharp
new WebSocketMessage {
    Type = "message-type",                    // Message type identifier
    Data = new { ... },                       // Arbitrary data
    TextData = "...",                         // Text message content
    RawData = new byte[] { ... },             // Binary data
    IsBinary = false,                         // Message type indicator
    Timestamp = DateTime.UtcNow                // Auto-set creation time
}
```

## Testing Pattern

```csharp
[Fact]
public async Task WebSocket_ShouldWork() {
    var server = WireMockServer.Start();
    
    server.Given(Request.Create().WithPath("/ws"))
        .RespondWith(Response.Create()
            .WithWebSocketHandler(async ctx => {
                // Configure handler
            }));
    
    using var client = new ClientWebSocket();
    await client.ConnectAsync(
        new Uri($"ws://localhost:{server.Port}/ws"), default);
    
    // Test interactions
    
    server.Stop();
}
```

## Common Patterns

| Pattern | Code |
|---------|------|
| **Path Only** | `.WithPath("/ws")` |
| **Path + Subprotocol** | `.WithPath("/ws")` + `.WithWebSocketSubprotocol("chat")` |
| **With Authentication** | `.WithHeader("Authorization", "Bearer token")` |
| **Echo Back** | See Echo Handler above |
| **Route by Type** | See Message Routing above |
| **Send on Connect** | `.WithWebSocketMessage(msg)` |
| **Keep Alive** | `.WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))` |
| **Long Timeout** | `.WithWebSocketTimeout(TimeSpan.FromHours(1))` |

## Async Utilities

```csharp
// Send Text
await ws.SendAsync(
    new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)),
    WebSocketMessageType.Text, true, default);

// Send Binary
await ws.SendAsync(
    new ArraySegment<byte>(bytes),
    WebSocketMessageType.Binary, true, default);

// Receive
var buffer = new byte[1024];
var result = await ws.ReceiveAsync(
    new ArraySegment<byte>(buffer), default);

// Close
await ws.CloseAsync(
    WebSocketCloseStatus.NormalClosure, "Done", default);
```

## Properties Available

```csharp
var response = Response.Create();
response.WebSocketHandler              // Func<WebSocketHandlerContext, Task>
response.WebSocketMessageHandler        // Func<WebSocketMessage, Task<WebSocketMessage?>>
response.WebSocketKeepAliveInterval     // TimeSpan?
response.WebSocketTimeout               // TimeSpan?
response.IsWebSocketConfigured          // bool
```

## Error Handling

```csharp
try {
    // WebSocket operations
} catch (WebSocketException ex) {
    // Handle WebSocket errors
} catch (OperationCanceledException) {
    // Handle timeout
} finally {
    if (ws.State != WebSocketState.Closed) {
        await ws.CloseAsync(
            WebSocketCloseStatus.InternalServerError,
            "Error", default);
    }
}
```

## Frequently Used Namespaces

```csharp
using System.Net.WebSockets;              // WebSocket, WebSocketState, etc.
using System.Text;                        // Encoding
using System.Threading;                   // CancellationToken
using System.Threading.Tasks;             // Task
using WireMock.RequestBuilders;          // Request
using WireMock.ResponseBuilders;         // Response
using WireMock.Server;                   // WireMockServer
using WireMock.WebSockets;               // WebSocketMessage, etc.
```

## Version Support

| Platform | Support |
|----------|---------|
| .NET Core 3.1 | ✅ Full |
| .NET 5.0 | ✅ Full |
| .NET 6.0 | ✅ Full |
| .NET 7.0 | ✅ Full |
| .NET 8.0 | ✅ Full |
| .NET Framework | ❌ Not supported |
| .NET Standard | ⏳ Framework refs only |

## Troubleshooting Checklist

- [ ] Server started before connecting?
- [ ] Correct URL path? (ws:// not ws)
- [ ] Handler set with WithWebSocketHandler()?
- [ ] Closing connections properly?
- [ ] CancellationToken passed to async methods?
- [ ] Keep-alive interval < client timeout?
- [ ] Error handling in handler?
- [ ] Tests using IAsyncLifetime?

## Performance Tips

✅ Close WebSockets when done  
✅ Set appropriate timeouts  
✅ Use keep-alive for idle connections  
✅ Handle exceptions gracefully  
✅ Don't block in handlers (await, don't Task.Result)  

## Limits & Constraints

- ⚠️ .NET Core 3.1+  only
- ⚠️ HTTPS (WSS) needs certificate setup
- ⚠️ Sequential message processing per connection
- ⚠️ Default buffer size: 1024 * 4 bytes

## Links

- [WebSocket RFC 6455](https://tools.ietf.org/html/rfc6455)
- [System.Net.WebSockets Docs](https://docs.microsoft.com/en-us/dotnet/api/system.net.websockets)
- [WireMock.Net GitHub](https://github.com/WireMock-Net/WireMock.Net)
- [WireMock.Net Issues](https://github.com/WireMock-Net/WireMock.Net/issues)

---

**For detailed documentation, see**: `WEBSOCKET_GETTING_STARTED.md` or `src/WireMock.Net.WebSockets/README.md`
