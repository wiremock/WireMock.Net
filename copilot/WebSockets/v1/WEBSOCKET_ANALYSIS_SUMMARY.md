# WireMock.Net WebSocket Analysis - Executive Summary

## Overview

This analysis examines the WireMock.Net architecture and proposes a comprehensive WebSocket implementation strategy that maintains consistency with the existing fluent interface design patterns.

---

## Key Findings

### 1. Architecture Foundation

**WireMock.Net is built on three architectural layers:**

```
┌─────────────────────────────────────────┐
│ Abstractions Layer                      │
│ (Interfaces & Models)                   │
│ WireMock.Net.Abstractions               │
└──────────────────┬──────────────────────┘
                   │
                   ↓
┌─────────────────────────────────────────┐
│ Core Implementation Layer                │
│ (Full fluent interface)                 │
│ WireMock.Net.Minimal                    │
└──────────────────┬──────────────────────┘
                   │
                   ↓
┌─────────────────────────────────────────┐
│ Integration Layers                      │
│ (OWIN, StandAlone, Full)               │
│ WireMock.Net, WireMock.Net.StandAlone  │
└─────────────────────────────────────────┘
```

### 2. Fluent Interface Pattern

The fluent API is built on **four interconnected patterns:**

| Pattern | Purpose | Location | Key Files |
|---------|---------|----------|-----------|
| **Request Builder** | HTTP/WebSocket matching | RequestBuilders/ | `Request.cs` + `Request.With*.cs` |
| **Response Builder** | HTTP/WebSocket responses | ResponseBuilders/ | `Response.cs` + `Response.With*.cs` |
| **Mapping Builder** | Scenario orchestration | Server/ | `MappingBuilder.cs` + `RespondWithAProvider.cs` |
| **Specialized Builders** | Domain-specific logic | ResponseBuilders/ | `WebSocketResponseBuilder.cs` (new) |

### 3. Design Principles

1. **Composition over Inheritance**: Partial classes separate concerns while maintaining fluent chains
2. **Interface Segregation**: Consumers depend on small, focused interfaces
3. **Method Chaining**: All builder methods return the builder type for fluency
4. **Async-First**: Callbacks and transformers support both sync and async operations
5. **Extensibility**: New features don't require changes to core classes

---

## WebSocket Implementation Strategy

### Phase 1: Abstractions (WireMock.Net.Abstractions)

**Create new abstractions:**

```csharp
IWebSocketMessage          // Single message in stream
IWebSocketResponse         // Collection of messages + metadata
IWebSocketResponseBuilder  // Fluent builder for WebSocket config
```

**Extend existing abstractions:**

```csharp
// Update ResponseModel to include WebSocket config
public class WebSocketResponseModel { ... }

// Update IResponseBuilder to support WebSocket
public interface IResponseBuilder
{
    IResponseBuilder WithWebSocket(Action<IWebSocketResponseBuilder> configure);
    // ... other WebSocket methods
}

// Update IRequestBuilder for WebSocket matching
public interface IRequestBuilder
{
    IRequestBuilder WithWebSocketPath(string path);
    IRequestBuilder WithWebSocketSubprotocol(string subprotocol);
    // ... other WebSocket matching methods
}
```

### Phase 2: Models (WireMock.Net.Minimal)

**Create new domain models:**

```
Models/
├── WebSocketMessage.cs      // Individual message
├── WebSocketResponse.cs      // Response configuration
```

### Phase 3: Request Builder Extension

**Create partial class:**

```
RequestBuilders/
└── Request.WithWebSocket.cs
    ├── WithWebSocketUpgrade()        // Match upgrade headers
    ├── WithWebSocketPath()           // Match path + upgrade
    ├── WithWebSocketSubprotocol()    // Match subprotocol
    ├── WithWebSocketVersion()        // Match WS version
    └── WithWebSocketOrigin()         // Match origin (CORS)
```

### Phase 4: Response Builder Extension

**Create new components:**

```
ResponseBuilders/
├── Response.WithWebSocket.cs         // Add WebSocket methods to Response
├── WebSocketResponseBuilder.cs       // Fluent builder for messages
└── WebSocketResponseBuilder.cs       // IWebSocketResponseBuilder impl
```

**Key methods:**

```csharp
// Static messages
.WithWebSocket(ws => ws
    .WithMessage("text message", delayMs: 0)
    .WithJsonMessage(obj, delayMs: 500)
    .WithBinaryMessage(bytes, delayMs: 1000)
)

// Dynamic messages
.WithWebSocketCallback(async request => 
    new[] { ... messages based on request ... }
)

// Configuration
.WithWebSocketTransformer()
.WithWebSocketSubprotocol("protocol-name")
.WithWebSocketClose(1000, "reason")
.WithWebSocketAutoClose(delayMs)
```

### Phase 5: Server Integration

**Update server components:**

```
Server/
├── WireMockServer.cs                 // Handle WebSocket upgrade
├── WireMockMiddleware.cs             // WebSocket middleware
└── MappingMatcher.cs                 // Route WebSocket connections
```

---

## Usage Patterns

### Pattern 1: Simple Echo

```csharp
server.Given(Request.Create().WithWebSocketPath("/echo"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async req =>
            new[] { new WebSocketMessage { BodyAsString = req.Body } }
        )
    );
```

### Pattern 2: Sequence of Messages

```csharp
server.Given(Request.Create().WithWebSocketPath("/stream"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("Starting", delayMs: 0)
            .WithMessage("Processing", delayMs: 1000)
            .WithMessage("Done", delayMs: 2000)
            .WithClose(1000)
        )
    );
```

### Pattern 3: Dynamic Content with Transformer

```csharp
server.Given(Request.Create().WithWebSocketPath("/api"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { user = "{{request.headers.X-User}}" })
            .WithTransformer()
        )
    );
```

### Pattern 4: State-Based Behavior

```csharp
server.Given(Request.Create().WithWebSocketPath("/chat"))
    .InScenario("ChatRoom")
    .WhenStateIs("Authenticated")
    .WillSetStateTo("ChatActive")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { status = "authenticated" })
        )
    );
```

---

## File Structure

```
src/WireMock.Net.Abstractions/
├── Models/
│   ├── IWebSocketMessage.cs          (NEW)
│   ├── IWebSocketResponse.cs          (NEW)
│   └── IWebhookRequest.cs            (existing)
├── Admin/Mappings/
│   └── WebSocketModel.cs              (NEW)
├── BuilderExtensions/
│   └── WebSocketResponseModelBuilder.cs (NEW)
└── (other existing files)

src/WireMock.Net.Minimal/
├── Models/
│   ├── WebSocketMessage.cs            (NEW)
│   └── WebSocketResponse.cs           (NEW)
├── RequestBuilders/
│   ├── Request.cs                    (existing)
│   └── Request.WithWebSocket.cs      (NEW)
├── ResponseBuilders/
│   ├── Response.cs                   (existing)
│   ├── Response.WithWebSocket.cs     (NEW)
│   └── WebSocketResponseBuilder.cs   (NEW)
├── Server/
│   ├── WireMockServer.cs             (modify)
│   ├── WireMockMiddleware.cs         (modify)
│   └── MappingMatcher.cs             (modify)
└── (other existing files)
```

---

## Implementation Benefits

### ✅ Consistency
- Uses same fluent patterns as existing HTTP mocking
- Developers already familiar with the API

### ✅ Flexibility
- Supports static messages, dynamic callbacks, templates
- Works with existing transformers (Handlebars, Scriban)

### ✅ Composability
- Messages, transformers, state management compose naturally
- Integrates with scenario management and webhooks

### ✅ Testability
- Deterministic message ordering
- Controllable delays simulate realistic scenarios
- State management enables complex test flows

### ✅ Maintainability
- Partial classes separate concerns
- No breaking changes to existing code
- Follows established patterns

---

## Comparison with Alternatives

### Approach A: Direct Implementation (Proposed)
```
Pros: Consistent with existing patterns, familiar API, composable
Cons: More code, careful design needed
✓ Recommended
```

### Approach B: Minimal Wrapper
```
Pros: Quick implementation
Cons: Inconsistent API, hard to extend, confusing for users
✗ Not recommended
```

### Approach C: Separate Library
```
Pros: Decoupled from main codebase
Cons: Fragmented ecosystem, duplicate code, harder to maintain
✗ Not recommended
```

---

## Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Fluent API for WebSocket** | Consistency with HTTP mocking |
| **Partial classes for extension** | Separation of concerns |
| **Builder pattern for messages** | Composable message sequences |
| **Async callback support** | WebSockets are inherently async |
| **Transformer support** | Reuse existing templating engine |
| **Message delays** | Realistic simulation of network latency |
| **Callback generators** | Dynamic responses based on request context |

---

## Risk Assessment

### Low Risk
- ✅ No changes to existing HTTP mocking functionality
- ✅ New code isolated in separate files
- ✅ Interfaces designed for backward compatibility

### Medium Risk
- ⚠️ WebSocket middleware integration with OWIN/AspNetCore
- ⚠️ Message ordering and delivery guarantees
- ⚠️ Connection state management

### Mitigation
- Comprehensive unit tests for builders
- Integration tests for middleware
- Connection lifecycle tests
- Load testing for concurrent connections

---

## Timeline Estimate

| Phase | Duration | Effort |
|-------|----------|--------|
| Phase 1: Abstractions | 1-2 days | Low |
| Phase 2: Models | 1-2 days | Low |
| Phase 3: Request Builder | 2-3 days | Medium |
| Phase 4: Response Builder | 3-4 days | Medium |
| Phase 5: Server Integration | 5-7 days | High |
| Phase 6: Admin Interface | 2-3 days | Medium |
| Testing & Documentation | 5-7 days | Medium |
| **Total** | **3-4 weeks** | **~100 hours** |

---

## Next Steps

1. **Review & Approval**: Share this design with team
2. **Create abstractions**: Start with IWebSocketMessage, IWebSocketResponse
3. **Implement builders**: RequestBuilder and ResponseBuilder extensions
4. **Integrate with server**: Update WireMockMiddleware for WebSocket support
5. **Add admin API**: Expose WebSocket configuration via REST API
6. **Document & release**: Add examples, tutorials, API docs

---

## Related Documentation

1. **WEBSOCKET_FLUENT_INTERFACE_DESIGN.md** - Detailed architecture and patterns
2. **WEBSOCKET_IMPLEMENTATION_TEMPLATES.md** - Ready-to-use code templates
3. **WEBSOCKET_PATTERNS_BEST_PRACTICES.md** - Visual guides and best practices

---

## Conclusion

The proposed WebSocket implementation maintains WireMock.Net's design philosophy of providing an intuitive, composable fluent API. By extending rather than replacing existing patterns, developers can leverage their knowledge of HTTP mocking to easily mock complex WebSocket scenarios.

The phased approach minimizes risk, the design supports both simple and complex use cases, and the fluent API ensures consistency across the entire platform.

**Recommendation**: Proceed with Phase 1 (Abstractions) to validate the design approach, then continue with subsequent phases based on community feedback.
