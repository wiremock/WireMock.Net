# WebSocket Design - Naming Update (v2)

## Change Summary

**Updated Naming**: `WithWebSocketUpgrade()` → `WithWebSocket()`

**Status**: ✅ All code templates and examples updated

---

## Why This Change?

### The Problem with `WithWebSocketUpgrade()`
- ❌ 21 characters - verbose
- ❌ Emphasizes HTTP protocol detail (upgrade mechanism)
- ❌ Harder to discover in IntelliSense
- ❌ Different from Response builder naming
- ❌ Doesn't match developer expectations

### The Solution: `WithWebSocket()`
- ✅ 14 characters - 33% shorter
- ✅ Clear intent - "I'm using WebSocket"
- ✅ Easy to discover - intuitive searching
- ✅ Consistent - matches Response builder
- ✅ Intuitive - what developers search for

---

## The Change in Code

### Old Design (v1)
```csharp
Request.Create()
    .WithPath("/ws")
    .WithWebSocketUpgrade()        // ❌ What's an "upgrade"?
    .WithWebSocketSubprotocol("v1")
```

### New Design (v2)
```csharp
Request.Create()
    .WithPath("/ws")
    .WithWebSocket()               // ✅ Clear: I'm using WebSocket
    .WithWebSocketSubprotocol("v1")

// Or with convenience method:
Request.Create()
    .WithWebSocketPath("/ws")      // ✅ Combines path + WebSocket
    .WithWebSocketSubprotocol("v1")
```

---

## Two Valid Patterns

### Pattern 1: Explicit Composition
```csharp
Request.Create()
    .WithPath("/ws")
    .WithWebSocket()
    .WithWebSocketSubprotocol("v1")
```
**Use when**: Complex matchers, need flexibility, explicit is clearer

### Pattern 2: Convenience Method
```csharp
Request.Create()
    .WithWebSocketPath("/ws")
    .WithWebSocketSubprotocol("v1")
```
**Use when**: Simple setup, quick prototyping, code clarity

---

## Complete Request Builder API (v2)

```csharp
// Core WebSocket matching
public IRequestBuilder WithWebSocket()
{
    // Matches: Upgrade: websocket, Connection: *Upgrade*
}

// Convenience: combines WithPath() + WithWebSocket()
public IRequestBuilder WithWebSocketPath(string path)
{
    return WithPath(path).WithWebSocket();
}

// Additional matchers
public IRequestBuilder WithWebSocketSubprotocol(string subprotocol)
{
    // Matches: Sec-WebSocket-Protocol: subprotocol
}

public IRequestBuilder WithWebSocketVersion(string version = "13")
{
    // Matches: Sec-WebSocket-Version: version
}

public IRequestBuilder WithWebSocketOrigin(string origin)
{
    // Matches: Origin: origin
}
```

---

## Real-World Examples (v2)

### Echo Server
```csharp
server.Given(Request.Create()
    .WithWebSocketPath("/echo"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("Echo ready")
        )
        .WithWebSocketCallback(async request =>
            new[] { new WebSocketMessage { BodyAsString = $"Echo: {request.Body}" } }
        )
    );
```

### Chat with Subprotocol
```csharp
server.Given(Request.Create()
    .WithWebSocketPath("/chat")
    .WithWebSocketSubprotocol("chat-v1"))
    .RespondWith(Response.Create()
        .WithWebSocketSubprotocol("chat-v1")
        .WithWebSocket(ws => ws
            .WithMessage("Welcome to chat")
        )
    );
```

### With CORS/Origin
```csharp
server.Given(Request.Create()
    .WithPath("/secure-ws")
    .WithWebSocket()
    .WithWebSocketOrigin("https://app.com"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("CORS validated")
        )
    );
```

### With Scenario State
```csharp
server.Given(Request.Create()
    .WithWebSocketPath("/api")
    .WithWebSocket())
    .InScenario("ActiveSessions")
    .WhenStateIs("Authenticated")
    .WillSetStateTo("SessionActive")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("Session established")
        )
    );
```

---

## Comparison: Old vs New

| Aspect | Old (v1) | New (v2) | Improvement |
|--------|----------|----------|-------------|
| **Method Name** | `WithWebSocketUpgrade()` | `WithWebSocket()` | Simpler (21→14 chars) |
| **Intent** | "Upgrade the protocol" | "Use WebSocket" | Clearer |
| **Consistency** | Different from Response | Matches Response | Unified API |
| **Discoverability** | Hard to find | Easy in IntelliSense | Better UX |
| **Pattern Support** | Implicit | Explicit + Convenience | More flexible |
| **Code Clarity** | Emphasizes HTTP detail | Emphasizes WebSocket | Abstraction right |

---

## Design Rationale

### Why Not Other Names?
- ❌ `WithWebSocketConnect()` - implies connection initiation
- ❌ `WithWebSocketEnabled()` - redundant (boolean implied)
- ❌ `WithWebSocketUpgrade()` - emphasizes HTTP mechanism
- ✅ `WithWebSocket()` - direct, clear, intuitive

### Why Two Patterns?
- **Explicit** (`WithPath().WithWebSocket()`): Clear composition, DRY principle
- **Convenience** (`WithWebSocketPath()`): Faster typing, self-documenting

Both are equally valid - choose based on your preference.

---

## Migration Guide (If Updating Code)

### Find & Replace
```
WithWebSocketUpgrade()  →  WithWebSocket()
```

### In Code Examples
**Before:**
```csharp
Request.Create().WithPath("/ws").WithWebSocketUpgrade()
```

**After:**
```csharp
Request.Create().WithPath("/ws").WithWebSocket()
```

Or use convenience:
```csharp
Request.Create().WithWebSocketPath("/ws")
```

---

## Consistency with Response Builder

### Request Side
```csharp
Request.Create()
    .WithWebSocket()  // Core method
```

### Response Side
```csharp
Response.Create()
    .WithWebSocket(ws => ws  // Same root name
        .WithMessage(...)
    )
```

This naming consistency makes the fluent API intuitive and easy to learn.

---

## Benefits Summary

✅ **Simpler**: Fewer characters, easier to type  
✅ **Clearer**: Focuses on intent, not protocol details  
✅ **Consistent**: Matches Response builder naming  
✅ **Better UX**: IntelliSense friendly  
✅ **Flexible**: Both explicit and convenience available  
✅ **Aligned**: Matches WireMock.Net conventions  

---

## Implementation Checklist

- [x] Design rationale documented
- [x] Code examples updated
- [x] Templates updated
- [x] Two patterns explained
- [x] Migration guide provided
- [x] Benefits documented
- [ ] Team implementation (your turn)
- [ ] Code review
- [ ] Testing
- [ ] Documentation update

---

**Version**: v2  
**Status**: ✅ Complete - Ready for Implementation  
**Impact**: Naming improvement, no breaking changes
