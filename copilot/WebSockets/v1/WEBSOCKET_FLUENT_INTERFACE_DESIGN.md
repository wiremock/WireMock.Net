# WebSocket Support in WireMock.Net - Fluent Interface Design Proposal

## Executive Summary

This document analyzes the WireMock.Net architecture and proposes a fluent interface design for WebSocket support in the `WireMock.Net.Minimal` project, following the established patterns in the codebase.

---

## Part 1: Current Architecture Analysis

### 1.1 Project Structure

**Core Projects:**
- **WireMock.Net.Abstractions**: Defines interfaces and abstract models (no implementation)
- **WireMock.Net.Minimal**: Core implementation with full fluent interface support
- **WireMock.Net.StandAlone**: OWIN self-hosting wrapper
- **WireMock.Net**: Full-featured version (extends Minimal)

### 1.2 Fluent Interface Pattern Overview

The fluent interface is built on three primary components working together:

#### **A. Request Builder Pattern** (`RequestBuilders/Request*.cs` files)

```csharp
// Entry point
var requestBuilder = Request.Create()
    .WithPath("/api/users")
    .UsingGet()
    .WithHeader("Authorization", "Bearer token");
```

**Key Characteristics:**
- Partial class `Request` with multiple `Request.WithXxx.cs` files
- Each file focuses on a specific concern (Path, Headers, Params, etc.)
- Implements `IRequestBuilder` interface
- Returns `this` (IRequestBuilder) for chaining
- Uses composition: `Request : RequestMessageCompositeMatcher, IRequestBuilder`

#### **B. Response Builder Pattern** (`ResponseBuilders/Response*.cs` files)

```csharp
// Fluent response building
Response.Create()
    .WithStatusCode(200)
    .WithHeader("Content-Type", "application/json")
    .WithBodyAsJson(new { id = 1, name = "John" })
    .WithDelay(TimeSpan.FromSeconds(1))
    .WithTransformer()
```

**Key Characteristics:**
- Partial class `Response` with separate files for features
- Methods return `IResponseBuilder` (returns `this`)
- Supports both sync and async callbacks via `WithCallback()`
- Pluggable transformers (Handlebars, Scriban)
- Examples:
  - `Response.WithCallback.cs`: Sync/async request handlers
  - `Response.WithTransformer.cs`: Template transformation
  - `Response.WithProxy.cs`: HTTP proxying
  - `Response.WithFault.cs`: Simulated faults

#### **C. Mapping Builder Pattern** (`MappingBuilder.cs` + `RespondWithAProvider.cs`)

```csharp
server.Given(Request.Create().WithPath("/endpoint"))
    .AtPriority(1)
    .WithTitle("My Endpoint")
    .InScenario("User Workflow")
    .WhenStateIs("LoggedIn")
    .WillSetStateTo("DataFetched")
    .WithWebhook(new Webhook { ... })
    .RespondWith(Response.Create().WithBody("response"))
```

**Key Characteristics:**
- `MappingBuilder.Given()` returns `IRespondWithAProvider`
- `RespondWithAProvider` chains metadata (priority, scenario, webhooks)
- Terminal method: `RespondWith(IResponseProvider)` or `ThenRespondWith()`
- Fluent methods return `IRespondWithAProvider` for chaining
- Example webhook support shows the pattern for extensions

### 1.3 Key Design Patterns Used

| Pattern | Location | Purpose |
|---------|----------|---------|
| **Partial Classes** | `Response.cs`, `Request.cs` | Separation of concerns while maintaining fluent interface |
| **Builder Pattern** | `RequestBuilders/`, `ResponseBuilders/` | Incremental construction |
| **Composite Pattern** | `RequestMessageCompositeMatcher` | Composable matchers |
| **Interface Segregation** | `IResponseBuilder`, `IRequestBuilder` | Contract definition |
| **Fluent API** | All builders | Method chaining |
| **Extension Methods** | Various `*.cs` partial files | Feature addition without breaking changes |

---

## Part 2: WebSocket Support Design

### 2.1 Architecture for WebSocket Support

WebSocket support should follow a similar pattern to existing features. The key difference is that WebSockets are **bidirectional** and **stateful**, requiring:

1. **Request matching** (connection phase)
2. **Message routing** (message handling)
3. **State management** (connection state)
4. **Simulated server messages** (push messages)

### 2.2 Proposed Model Classes (WireMock.Net.Abstractions)

Create new interfaces in `WireMock.Net.Abstractions`:

```csharp
// File: Admin/Mappings/WebSocketModel.cs
namespace WireMock.Admin.Mappings;

public class WebSocketMessageModel
{
    public int? DelayMs { get; set; }
    public string? BodyAsString { get; set; }
    public byte[]? BodyAsBytes { get; set; }
    public bool IsText { get; set; } = true;
}

public class WebSocketResponseModel
{
    public List<WebSocketMessageModel> Messages { get; set; } = new();
    public bool UseTransformer { get; set; }
    public TransformerType TransformerType { get; set; } = TransformerType.Handlebars;
    public string? CloseMessage { get; set; }
    public int? CloseCode { get; set; }
}
```

### 2.3 Domain Models (WireMock.Net.Minimal)

Create new model in `Models/`:

```csharp
// File: src/WireMock.Net.Minimal/Models/WebSocketMessage.cs
namespace WireMock.Models;

public class WebSocketMessage : IWebSocketMessage
{
    public int DelayMs { get; set; }
    public string? BodyAsString { get; set; }
    public byte[]? BodyAsBytes { get; set; }
    public bool IsText { get; set; } = true;
}

// File: src/WireMock.Net.Minimal/Models/WebSocketResponse.cs
namespace WireMock.Models;

public class WebSocketResponse : IWebSocketResponse
{
    public List<IWebSocketMessage> Messages { get; set; } = new();
    public bool UseTransformer { get; set; }
    public TransformerType TransformerType { get; set; } = TransformerType.Handlebars;
    public string? CloseMessage { get; set; }
    public int? CloseCode { get; set; }
}
```

### 2.4 Request Builder Extension (WireMock.Net.Minimal)

Create partial class to extend request matching for WebSockets:

```csharp
// File: src/WireMock.Net.Minimal/RequestBuilders/Request.WithWebSocket.cs
namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <summary>
    /// Match WebSocket connection upgrade requests.
    /// </summary>
    public IRequestBuilder WithWebSocketUpgrade()
    {
        Add(new RequestMessageHeaderMatcher("Upgrade", new ExactMatcher("websocket")));
        Add(new RequestMessageHeaderMatcher("Connection", new WildcardMatcher("*Upgrade*")));
        return this;
    }

    /// <summary>
    /// Match specific WebSocket subprotocol.
    /// </summary>
    public IRequestBuilder WithWebSocketSubprotocol(string subprotocol)
    {
        Guard.NotNullOrWhiteSpace(subprotocol);
        Add(new RequestMessageHeaderMatcher("Sec-WebSocket-Protocol", new ExactMatcher(subprotocol)));
        return this;
    }

    /// <summary>
    /// Match WebSocket connection by path (typical pattern).
    /// </summary>
    public IRequestBuilder WithWebSocketPath(string path)
    {
        Guard.NotNullOrWhiteSpace(path);
        return WithPath(path).WithWebSocketUpgrade();
    }
}
```

### 2.5 Response Builder Extension (WireMock.Net.Minimal)

Create partial class for WebSocket response handling:

```csharp
// File: src/WireMock.Net.Minimal/ResponseBuilders/Response.WithWebSocket.cs
namespace WireMock.ResponseBuilders;

public partial class Response
{
    public IWebSocketResponse? WebSocketResponse { get; private set; }

    public bool WithWebSocketUsed { get; private set; }

    /// <summary>
    /// Configure WebSocket response with messages to send after connection.
    /// </summary>
    public IResponseBuilder WithWebSocket(Action<IWebSocketResponseBuilder> configure)
    {
        Guard.NotNull(configure);

        var builder = new WebSocketResponseBuilder();
        configure(builder);

        WithWebSocketUsed = true;
        WebSocketResponse = builder.Build();

        return this;
    }

    /// <summary>
    /// Configure WebSocket response with a single message.
    /// </summary>
    public IResponseBuilder WithWebSocketMessage(string message, int? delayMs = null)
    {
        Guard.NotNullOrWhiteSpace(message);

        return WithWebSocket(b => b
            .WithMessage(message, delayMs)
        );
    }

    /// <summary>
    /// Configure WebSocket with async callback for dynamic message generation.
    /// </summary>
    public IResponseBuilder WithWebSocketCallback(
        Func<IRequestMessage, Task<IEnumerable<IWebSocketMessage>>> handler)
    {
        Guard.NotNull(handler);

        WithWebSocketUsed = true;
        WebSocketCallbackAsync = handler;

        return this;
    }

    /// <summary>
    /// Sets transformer for WebSocket messages.
    /// </summary>
    public IResponseBuilder WithWebSocketTransformer(
        bool use = true,
        TransformerType transformerType = TransformerType.Handlebars)
    {
        if (WebSocketResponse != null)
        {
            WebSocketResponse.UseTransformer = use;
            WebSocketResponse.TransformerType = transformerType;
        }

        return this;
    }

    /// <summary>
    /// Configure WebSocket close frame (graceful disconnect).
    /// </summary>
    public IResponseBuilder WithWebSocketClose(int? closeCode = 1000, string? reason = null)
    {
        if (WebSocketResponse != null)
        {
            WebSocketResponse.CloseCode = closeCode;
            WebSocketResponse.CloseMessage = reason;
        }

        return this;
    }

    public Func<IRequestMessage, Task<IEnumerable<IWebSocketMessage>>>? WebSocketCallbackAsync { get; private set; }

    public bool WithWebSocketCallbackUsed => WebSocketCallbackAsync != null;
}
```

### 2.6 WebSocket Response Builder (WireMock.Net.Minimal)

Create fluent builder for WebSocket messages:

```csharp
// File: src/WireMock.Net.Minimal/ResponseBuilders/WebSocketResponseBuilder.cs
namespace WireMock.ResponseBuilders;

public class WebSocketResponseBuilder : IWebSocketResponseBuilder
{
    private readonly List<IWebSocketMessage> _messages = new();
    private bool _useTransformer;
    private TransformerType _transformerType = TransformerType.Handlebars;
    private int? _closeCode;
    private string? _closeMessage;

    public IWebSocketResponseBuilder WithMessage(string message, int? delayMs = null)
    {
        Guard.NotNullOrWhiteSpace(message);

        _messages.Add(new WebSocketMessage
        {
            BodyAsString = message,
            DelayMs = delayMs ?? 0,
            IsText = true
        });

        return this;
    }

    public IWebSocketResponseBuilder WithBinaryMessage(byte[] data, int? delayMs = null)
    {
        Guard.NotNull(data);

        _messages.Add(new WebSocketMessage
        {
            BodyAsBytes = data,
            DelayMs = delayMs ?? 0,
            IsText = false
        });

        return this;
    }

    public IWebSocketResponseBuilder WithJsonMessage(object data, int? delayMs = null)
    {
        Guard.NotNull(data);

        var json = JsonConvert.SerializeObject(data);

        _messages.Add(new WebSocketMessage
        {
            BodyAsString = json,
            DelayMs = delayMs ?? 0,
            IsText = true
        });

        return this;
    }

    public IWebSocketResponseBuilder WithTransformer(
        bool use = true,
        TransformerType transformerType = TransformerType.Handlebars)
    {
        _useTransformer = use;
        _transformerType = transformerType;

        return this;
    }

    public IWebSocketResponseBuilder WithClose(int closeCode = 1000, string? reason = null)
    {
        _closeCode = closeCode;
        _closeMessage = reason;

        return this;
    }

    public IWebSocketResponse Build()
    {
        return new WebSocketResponse
        {
            Messages = _messages,
            UseTransformer = _useTransformer,
            TransformerType = _transformerType,
            CloseCode = _closeCode,
            CloseMessage = _closeMessage
        };
    }
}
```

### 2.7 Usage Examples

#### **Basic WebSocket Echo**

```csharp
server.Given(Request.Create().WithWebSocketPath("/echo"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("Connected to echo server")
        )
        .WithWebSocketCallback(async request =>
        {
            // Echo messages back from request body
            var messageText = request.Body;
            return new[]
            {
                new WebSocketMessage 
                { 
                    BodyAsString = $"Echo: {messageText}",
                    DelayMs = 100
                }
            };
        })
    );
```

#### **Simulated Server - Multiple Messages**

```csharp
server.Given(Request.Create().WithWebSocketPath("/chat"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("Welcome to chat room", delayMs: 0)
            .WithMessage("Other users: 2", delayMs: 500)
            .WithMessage("Ready for messages", delayMs: 1000)
            .WithTransformer()
            .WithClose(1000, "Room closing")
        )
    );
```

#### **JSON WebSocket API**

```csharp
server.Given(Request.Create()
    .WithWebSocketPath("/api/notifications")
    .WithWebSocketSubprotocol("chat-v1"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { type = "connected", userId = "{{request.headers.Authorization}}" })
            .WithJsonMessage(new { type = "notification", message = "You have a new message" }, delayMs: 2000)
            .WithTransformer()
        )
    );
```

#### **Dynamic Messages Based on Request**

```csharp
server.Given(Request.Create().WithWebSocketPath("/data-stream"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async request =>
        {
            var userId = request.Headers["X-User-Id"]?.FirstOrDefault();
            
            var messages = new List<WebSocketMessage>();
            for (int i = 0; i < 3; i++)
            {
                messages.Add(new WebSocketMessage
                {
                    BodyAsString = JsonConvert.SerializeObject(new 
                    { 
                        userId, 
                        sequence = i,
                        timestamp = DateTime.UtcNow
                    }),
                    DelayMs = i * 1000,
                    IsText = true
                });
            }
            
            return messages;
        })
    );
```

#### **Binary WebSocket (e.g., Protobuf)**

```csharp
server.Given(Request.Create().WithWebSocketPath("/binary"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithBinaryMessage(protoBytes, delayMs: 100)
            .WithBinaryMessage(anotherProtoBytes, delayMs: 200)
            .WithClose(1000)
        )
    );
```

---

## Part 3: Implementation Roadmap

### Phase 1: Abstractions & Core Models
1. Create `IWebSocketMessage` interface in `WireMock.Net.Abstractions`
2. Create `IWebSocketResponse` interface in `WireMock.Net.Abstractions`
3. Create `IWebSocketResponseBuilder` interface in `WireMock.Net.Abstractions`
4. Implement model classes in `WireMock.Net.Minimal`

### Phase 2: Request Builder Extensions
1. Create `Request.WithWebSocket.cs` partial class
2. Add WebSocket-specific matchers
3. Add integration tests for request matching

### Phase 3: Response Builder Extensions
1. Create `Response.WithWebSocket.cs` partial class
2. Create `WebSocketResponseBuilder.cs`
3. Implement transformer support
4. Add callback support for dynamic messages

### Phase 4: Server Integration
1. Extend `WireMockMiddleware.cs` to handle WebSocket upgrades
2. Implement WebSocket message routing
3. Connection lifecycle management (open/close)
4. Message queueing and delivery

### Phase 5: Admin Interface
1. Update `MappingModel` to include WebSocket configuration
2. Add WebSocket support to mapping serialization
3. REST API endpoints for WebSocket management

---

## Part 4: Key Design Decisions

### 4.1 Design Rationale

| Decision | Rationale |
|----------|-----------|
| **Fluent API for WebSocket** | Consistent with existing Request/Response builders |
| **Callback Support** | Enables dynamic message generation based on request context |
| **Async Messages** | WebSocket communication is inherently async |
| **Partial Classes** | Maintains separation of concerns (Path matching, Headers, WebSocket) |
| **Builder Pattern** | Allows composing complex WebSocket scenarios incrementally |
| **Message Queue** | Simulates realistic server behavior (message ordering, delays) |
| **Transformer Support** | Reuses Handlebars/Scriban for dynamic message content |

### 4.2 Comparison with Existing Features

```csharp
// Webhook (similar pattern - external notification)
.WithWebhook(new Webhook { Request = new WebhookRequest { ... } })

// WebSocket (new - connection-based messaging)
.WithWebSocket(ws => ws
    .WithMessage("...")
    .WithTransformer()
)

// Callback (existing - dynamic response)
.WithCallback(request => new ResponseMessage { ... })

// WebSocket Callback (new - dynamic WebSocket messages)
.WithWebSocketCallback(async request => 
    new[] { new WebSocketMessage { ... } }
)
```

---

## Part 5: Implementation Considerations

### 5.1 Dependencies
- **ASP.NET Core WebSocket support** (already available in Minimal)
- **IRequestMessage/IResponseMessage** (reuse existing)
- **Transformer infrastructure** (Handlebars/Scriban)
- **Message serialization** (Newtonsoft.Json)

### 5.2 Edge Cases to Handle
1. **Connection timeouts** - Server should be able to simulate client disconnect
2. **Message ordering** - Ensure messages are sent in the order defined
3. **Backpressure** - Handle slow clients
4. **Concurrent connections** - Multiple WebSocket clients to same endpoint
5. **Subprotocol negotiation** - Support WebSocket subprotocols

### 5.3 Testing Strategy
1. Unit tests for `WebSocketResponseBuilder`
2. Integration tests for connection matching
3. Message ordering and delivery tests
4. Transformer execution tests
5. Callback execution tests

### 5.4 Breaking Changes
- **None** - This is purely additive functionality following existing patterns

---

## Part 6: Integration Points

### 6.1 With Existing Features

```csharp
// WebSocket + Scenario State
server.Given(Request.Create().WithWebSocketPath("/status"))
    .InScenario("ServiceMonitoring")
    .WhenStateIs("Running")
    .WillSetStateTo("Stopped")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws.WithMessage("Service is running"))
    );

// WebSocket + Priority
server.Given(Request.Create().WithWebSocketPath("/ws"))
    .AtPriority(1)
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws.WithMessage("Priority response"))
    );

// WebSocket + Title & Description
server.Given(Request.Create().WithWebSocketPath("/api"))
    .WithTitle("WebSocket API")
    .WithDescription("Real-time data stream")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws.WithJsonMessage(data))
    );
```

### 6.2 With Admin Interface
```csharp
// Retrieve WebSocket mappings
var mappings = server.MappingModels
    .Where(m => m.Response.WebSocket != null)
    .ToList();

// Export WebSocket configuration
var json = server.MappingModels[0].ToString();
```

---

## Conclusion

The proposed WebSocket support follows WireMock.Net's established fluent API patterns while adding the unique requirements of bidirectional, stateful WebSocket communication. The design:

✅ **Maintains consistency** with existing Request/Response builders  
✅ **Enables reuse** of transformers and matchers  
✅ **Provides flexibility** through callbacks and builders  
✅ **Supports testing** scenarios (timing, multiple messages, state)  
✅ **Integrates naturally** with existing features (scenarios, priority, webhooks)  

This approach allows developers to mock complex WebSocket scenarios with the same familiar fluent syntax they use for HTTP mocking.
