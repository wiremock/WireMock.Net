# WebSocket Fluent Interface - Quick Reference

## At a Glance

### Current Architecture (HTTP Only)

```csharp
// Request matching
Request.Create()
    .WithPath("/api")
    .WithHeader("...")
    .UsingGet()

// Response building
Response.Create()
    .WithStatusCode(200)
    .WithBodyAsJson(...)
    .WithTransformer()

// Mapping
server.Given(request)
    .AtPriority(1)
    .InScenario("...")
    .RespondWith(response)
```

### Proposed Addition (WebSocket Support)

```csharp
// WebSocket request matching
Request.Create()
    .WithWebSocketPath("/ws")
    .WithWebSocketSubprotocol("chat")

// WebSocket response building
Response.Create()
    .WithWebSocket(ws => ws
        .WithMessage("Hello")
        .WithJsonMessage(obj)
        .WithTransformer()
    )
    .WithWebSocketCallback(async req => ... messages ...)

// Mapping (same as HTTP)
server.Given(request)
    .RespondWith(response)
```

---

## Quick Comparison: HTTP vs WebSocket Fluent API

### Request Builder

| HTTP | WebSocket |
|------|-----------|
| `WithPath(string)` | `WithWebSocketPath(string)` |
| `WithHeader(string, string)` | `WithHeader(...)` (same) |
| `UsingGet()` | `WithWebSocketUpgrade()` (implicit) |
| `WithParam(string, string)` | (not applicable) |
| `WithBody(string)` | (connection is upgrade, no body) |

### Response Builder

| HTTP | WebSocket |
|------|-----------|
| `WithStatusCode(int)` | `WithWebSocketClose(int)` |
| `WithBody(string)` | `WithMessage(string)` |
| `WithBodyAsJson(object)` | `WithJsonMessage(object)` |
| (binary: rarely used) | `WithBinaryMessage(byte[])` |
| `WithCallback(...)` | `WithWebSocketCallback(...)` |
| `WithTransformer()` | `WithTransformer()` (same) |

### Mapping Configuration

| Feature | HTTP | WebSocket |
|---------|------|-----------|
| Priority | ✓ `AtPriority(int)` | ✓ `AtPriority(int)` |
| Scenario | ✓ `InScenario(...)` | ✓ `InScenario(...)` |
| Webhook | ✓ `WithWebhook(...)` | ✓ `WithWebhook(...)` |
| Title/Desc | ✓ `WithTitle(...)` | ✓ `WithTitle(...)` |

---

## Implementation Checklist

### Phase 1: Abstractions
- [ ] Create `IWebSocketMessage` interface
- [ ] Create `IWebSocketResponse` interface
- [ ] Create `IWebSocketResponseBuilder` interface
- [ ] Add `WebSocketModel` to admin mappings
- [ ] Extend `IRequestBuilder` with WebSocket methods
- [ ] Extend `IResponseBuilder` with WebSocket methods

### Phase 2: Domain Models
- [ ] Implement `WebSocketMessage` class
- [ ] Implement `WebSocketResponse` class

### Phase 3: Request Builder Extension
- [ ] Create `Request.WithWebSocket.cs` partial class
- [ ] Implement `WithWebSocketUpgrade()`
- [ ] Implement `WithWebSocketPath()`
- [ ] Implement `WithWebSocketSubprotocol()`
- [ ] Add unit tests

### Phase 4: Response Builder Extension
- [ ] Create `Response.WithWebSocket.cs` partial class
- [ ] Implement WebSocket response methods
- [ ] Create `WebSocketResponseBuilder.cs`
- [ ] Add transformer support
- [ ] Add callback support
- [ ] Add unit tests

### Phase 5: Server Integration
- [ ] Update `WireMockMiddleware.cs` to handle WebSocket upgrades
- [ ] Implement WebSocket connection handling
- [ ] Implement message delivery
- [ ] Add connection lifecycle management
- [ ] Add integration tests

### Phase 6: Admin Interface
- [ ] Extend `MappingModel` with WebSocket config
- [ ] Update mapping serialization
- [ ] Add REST API endpoints for WebSocket management

---

## File Changes Summary

### New Files (Abstractions)
```
src/WireMock.Net.Abstractions/
├── Models/IWebSocketMessage.cs
├── Models/IWebSocketResponse.cs
├── BuilderExtensions/IWebSocketResponseBuilder.cs
└── Admin/Mappings/WebSocketModel.cs
```

### New Files (Implementation)
```
src/WireMock.Net.Minimal/
├── Models/WebSocketMessage.cs
├── Models/WebSocketResponse.cs
├── RequestBuilders/Request.WithWebSocket.cs
├── ResponseBuilders/Response.WithWebSocket.cs
└── ResponseBuilders/WebSocketResponseBuilder.cs
```

### Modified Files
```
src/WireMock.Net.Minimal/
├── ResponseBuilders/Response.cs (add interface definitions)
├── RequestBuilders/Request.cs (add interface definitions)
├── Server/WireMockServer.cs (WebSocket support)
├── Owin/WireMockMiddleware.cs (handle upgrades)
└── Owin/MappingMatcher.cs (WebSocket routing)
```

---

## Code Examples

### Simple WebSocket

```csharp
// Echo server
server.Given(Request.Create().WithWebSocketPath("/echo"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async req =>
            new[] { new WebSocketMessage { BodyAsString = req.Body } }
        )
    );
```

### Messages with Delays

```csharp
// Multi-message response
server.Given(Request.Create().WithWebSocketPath("/stream"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("First", 0)
            .WithMessage("Second", 500)
            .WithMessage("Third", 1000)
        )
    );
```

### Dynamic Messages

```csharp
// Request-based generation
server.Given(Request.Create().WithWebSocketPath("/api"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async request =>
        {
            var userId = request.Headers["X-User-Id"].First();
            return new[]
            {
                new WebSocketMessage 
                { 
                    BodyAsString = $"Hello {userId}" 
                }
            };
        })
    );
```

### Templated Messages

```csharp
// Handlebars in message content
server.Given(Request.Create().WithWebSocketPath("/api"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new 
            { 
                user = "{{request.headers.X-User}}"
            })
            .WithTransformer()
        )
    );
```

### Subprotocol Negotiation

```csharp
// Version-specific behavior
server.Given(Request.Create()
    .WithWebSocketPath("/api")
    .WithWebSocketSubprotocol("v2"))
    .RespondWith(Response.Create()
        .WithWebSocketSubprotocol("v2")
        .WithWebSocket(ws => ws
            .WithMessage("v2 protocol")
        )
    );
```

### With State Management

```csharp
// Scenario-aware behavior
server.Given(Request.Create().WithWebSocketPath("/chat"))
    .InScenario("ChatSession")
    .WhenStateIs("LoggedIn")
    .WillSetStateTo("ChatActive")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { status = "logged-in" })
        )
    );
```

---

## Design Principles

1. **Fluent First**: All builder methods return builder for chaining
2. **Composable**: Messages, transformers, callbacks combine naturally
3. **Consistent**: Follows HTTP mocking patterns and conventions
4. **Extensible**: Partial classes allow feature additions without refactoring
5. **Testable**: Deterministic, controllable message delivery

---

## Integration Points

### With Existing Features

```csharp
// Scenario management
server.Given(Request.Create().WithWebSocketPath("/ws"))
    .InScenario("Session")
    .WhenStateIs("Connected")
    .WillSetStateTo("Active")
    .RespondWith(...)

// Priority ordering
server.Given(Request.Create().WithWebSocketPath("/ws"))
    .AtPriority(1)
    .RespondWith(...)

// Webhooks
server.Given(Request.Create().WithWebSocketPath("/ws"))
    .WithWebhook(new Webhook { ... })
    .RespondWith(...)

// Admin interface
var mappings = server.MappingModels
    .Where(m => m.Response.WebSocket != null)
    .ToList()
```

---

## Testing Patterns

### Unit Test Template

```csharp
[Fact]
public void WebSocket_WithMultipleMessages_MaintainsOrder()
{
    // Arrange
    var builder = new WebSocketResponseBuilder();

    // Act
    var response = builder
        .WithMessage("First", 0)
        .WithMessage("Second", 100)
        .Build();

    // Assert
    Assert.Equal("First", response.Messages[0].BodyAsString);
    Assert.Equal("Second", response.Messages[1].BodyAsString);
    Assert.Equal(100, response.Messages[1].DelayMs);
}
```

### Integration Test Template

```csharp
[Fact]
public async Task WebSocket_Client_ReceivesMessages()
{
    // Arrange
    var server = WireMockServer.Start();
    server.Given(Request.Create().WithWebSocketPath("/test"))
        .RespondWith(Response.Create()
            .WithWebSocket(ws => ws
                .WithMessage("Hello")
                .WithMessage("World")
            )
        );

    // Act
    using var client = new ClientWebSocket();
    await client.ConnectAsync(new Uri($"ws://localhost:{server.Port}/test"), CancellationToken.None);
    
    // Receive first message
    var buffer = new byte[1024];
    var result = await client.ReceiveAsync(buffer, CancellationToken.None);
    var message1 = Encoding.UTF8.GetString(buffer, 0, result.Count);

    // Assert
    Assert.Equal("Hello", message1);
}
```

---

## Performance Considerations

| Aspect | Impact | Mitigation |
|--------|--------|-----------|
| Message queuing | Linear with message count | Use callbacks for large streams |
| Memory | One message per connection | Implement cleanup on close |
| Concurrency | Handle multiple connections | Use async callbacks |
| Delays | Thread pool usage | Use reasonable delays (< 60s) |

---

## Common Issues & Solutions

### Issue 1: Message ordering
**Problem**: Messages delivered out of order  
**Solution**: Use explicit delayMs, avoid concurrent message generation

### Issue 2: Connection timeout
**Problem**: Client disconnects before messages sent  
**Solution**: Reduce message delays, increase test timeout

### Issue 3: Memory leak
**Problem**: Connections not closing properly  
**Solution**: Always call `WithClose()` or `WithAutoClose()`

### Issue 4: Transformer not working
**Problem**: Template variables not substituted  
**Solution**: Ensure `WithTransformer()` is called, check variable syntax

---

## Related Classes & Methods

### Request Builder
- `Request.Create()` - Start building
- `WithPath(string)` - HTTP path or WebSocket path
- `WithHeader(string, string)` - Custom headers
- `UsingGet()`, `UsingPost()`, etc. - HTTP methods
- `WithWebSocketUpgrade()` - Mark as WebSocket
- `WithWebSocketPath(string)` - Convenience method
- `WithWebSocketSubprotocol(string)` - Protocol version

### Response Builder
- `Response.Create()` - Start building
- `WithStatusCode(int)` - HTTP status
- `WithBody(string)` - HTTP body
- `WithBodyAsJson(object)` - JSON response
- `WithCallback(...)` - Dynamic HTTP response
- `WithWebSocket(...)` - WebSocket configuration
- `WithWebSocketMessage(string)` - Single message
- `WithWebSocketCallback(...)` - Dynamic WebSocket messages
- `WithWebSocketTransformer()` - Template support
- `WithWebSocketClose(int, string)` - Graceful close

### Mapping Builder
- `Given(IRequestMatcher)` - Start mapping
- `AtPriority(int)` - Execution priority
- `InScenario(string)` - Scenario grouping
- `WhenStateIs(string)` - State condition
- `WillSetStateTo(string)` - State change
- `WithTitle(string)` - Display name
- `WithDescription(string)` - Documentation
- `WithWebhook(...)` - Side effects
- `RespondWith(IResponseProvider)` - Terminal method

---

## Versioning Strategy

### Version 1.0
- Basic WebSocket support
- Static messages
- Message delays
- Callback support
- Transformer integration

### Version 1.1
- Subprotocol negotiation
- Binary message support
- Auto-close functionality
- WebSocket metrics

### Version 2.0
- Streaming responses
- Backpressure handling
- Message compression
- Custom close codes

---

## References

- **RFC 6455**: The WebSocket Protocol
- **RFC 7231**: HTTP Semantics and Content
- **ASP.NET Core WebSocket Support**: https://docs.microsoft.com/en-us/dotnet/api/system.net.websockets
- **WireMock.Net Documentation**: https://wiremock.org/docs/dotnet

---

## Contact & Support

For questions or contributions regarding WebSocket support:
1. Review the comprehensive design documents
2. Check the implementation templates for code examples
3. Refer to the best practices guide for patterns
4. File issues with detailed reproduction steps
