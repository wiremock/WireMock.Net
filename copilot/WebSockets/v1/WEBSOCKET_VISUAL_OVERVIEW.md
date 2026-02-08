# WebSocket Implementation - Visual Architecture Overview

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    WireMock.Net Solution                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Abstraction Layer (WireMock.Net.Abstractions)           │   │
│  ├──────────────────────────────────────────────────────────┤   │
│  │ • IRequestBuilder                                        │   │
│  │   ├─ WithPath(), WithHeader(), UsingGet(), ...          │   │
│  │   └─ WithWebSocketPath() [NEW]                          │   │
│  │   └─ WithWebSocketSubprotocol() [NEW]                   │   │
│  │                                                          │   │
│  │ • IResponseBuilder                                       │   │
│  │   ├─ WithStatusCode(), WithBody(), WithCallback()       │   │
│  │   └─ WithWebSocket() [NEW]                              │   │
│  │   └─ WithWebSocketCallback() [NEW]                      │   │
│  │                                                          │   │
│  │ • IWebSocketResponseBuilder [NEW]                       │   │
│  │   ├─ WithMessage(string)                                │   │
│  │   ├─ WithJsonMessage(object)                            │   │
│  │   ├─ WithBinaryMessage(byte[])                          │   │
│  │   └─ WithTransformer()                                  │   │
│  └──────────────────────────────────────────────────────────┘   │
│                          ▲                                       │
│                          │ implements                            │
│                          │                                       │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Implementation Layer (WireMock.Net.Minimal)             │   │
│  ├──────────────────────────────────────────────────────────┤   │
│  │                                                          │   │
│  │ Request Building                                        │   │
│  │ ├─ Request.cs (core builder)                            │   │
│  │ ├─ Request.With*.cs (HTTP extensions)                   │   │
│  │ └─ Request.WithWebSocket.cs [NEW]                       │   │
│  │                                                          │   │
│  │ Response Building                                       │   │
│  │ ├─ Response.cs (core builder)                           │   │
│  │ ├─ Response.With*.cs (HTTP extensions)                  │   │
│  │ ├─ Response.WithWebSocket.cs [NEW]                      │   │
│  │ └─ WebSocketResponseBuilder.cs [NEW]                    │   │
│  │                                                          │   │
│  │ Domain Models                                           │   │
│  │ ├─ RequestMessage, ResponseMessage (existing)           │   │
│  │ ├─ WebSocketMessage [NEW]                               │   │
│  │ └─ WebSocketResponse [NEW]                              │   │
│  │                                                          │   │
│  │ Mapping Management                                      │   │
│  │ ├─ MappingBuilder.cs                                    │   │
│  │ ├─ RespondWithAProvider.cs                              │   │
│  │ └─ Mapping.cs                                           │   │
│  │                                                          │   │
│  │ Server Integration [NEW]                                │   │
│  │ ├─ WireMockServer.cs (update for WebSocket)             │   │
│  │ ├─ WireMockMiddleware.cs (upgrade handler)              │   │
│  │ ├─ MappingMatcher.cs (WebSocket routing)                │   │
│  │ └─ WebSocketConnectionManager [NEW]                     │   │
│  └──────────────────────────────────────────────────────────┘   │
│                          ▲                                       │
│                          │                                       │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Integration Layers                                       │   │
│  ├──────────────────────────────────────────────────────────┤   │
│  │ • WireMock.Net (extends Minimal)                         │   │
│  │ • WireMock.Net.StandAlone (OWIN hosting)                │   │
│  │ • WireMock.Net.AspNetCore.Middleware (ASP.NET Core)    │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Request Handling Flow (HTTP vs WebSocket)

### HTTP Request Flow

```
Client                  Server
  │                       │
  ├──── HTTP Request ────>│
  │                       │ Request.Create()
  │                       │   .WithPath("/api")
  │                       │   .UsingPost()
  │                       │ Match request matchers
  │                       │
  │<─── HTTP Response ────┤ Response.Create()
  │                       │   .WithStatusCode(200)
  │                       │   .WithBody(...)
  │                       │
  └───── Connection closed
```

### WebSocket Request Flow

```
Client                  Server
  │                       │
  ├─ WebSocket Upgrade ──>│
  │ (HTTP with headers)   │ Request.Create()
  │                       │   .WithWebSocketPath("/ws")
  │                       │ Match request matchers
  │                       │
  │<─ 101 Switching ──────┤ Upgrade to WebSocket
  │ Protocols             │
  │                       │
  │ ◄─────── Message 1 ───│ WebSocketResponse
  │                       │   .Messages[0]
  │ ◄─────── Message 2 ───│   .Messages[1]
  │                       │   (delayed 500ms)
  │ ◄─────── Message 3 ───│   .Messages[2]
  │                       │   (delayed 1000ms)
  │                       │
  │ ◄─── Close Frame ─────│ .WithClose(1000)
  │                       │
  └───── Connection closed
```

---

## Data Model Diagram

```
HTTP Request/Response Models          WebSocket Models
═══════════════════════════════════   ═══════════════════════════

RequestMessage                        IWebSocketMessage
├─ Path                               ├─ DelayMs
├─ Method (GET, POST, etc.)           ├─ BodyAsString
├─ Headers                            ├─ BodyAsBytes
├─ Body                               ├─ IsText
├─ Query Params                       ├─ Id
└─ Cookies                            └─ CorrelationId

ResponseMessage                       IWebSocketResponse
├─ StatusCode                         ├─ Messages[]
├─ Headers                            │   └─ IWebSocketMessage
├─ Body                               ├─ UseTransformer
├─ BodyAsJson                         ├─ TransformerType
└─ ContentType                        ├─ CloseCode
                                      ├─ CloseMessage
                                      ├─ Subprotocol
                                      └─ AutoCloseDelayMs

                                      WebSocketResponse
                                      (implements IWebSocketResponse)
```

---

## Builder Pattern Hierarchy

```
IRequestBuilder (interface)
    ▲
    │
    └── Request (class)
            │
            ├── Request.cs (core)
            ├── Request.WithPath.cs
            ├── Request.WithHeaders.cs
            ├── Request.UsingMethods.cs
            ├── Request.WithBody.cs
            ├── Request.WithParam.cs
            └── Request.WithWebSocket.cs [NEW]
                ├── WithWebSocketUpgrade()
                ├── WithWebSocketPath()
                ├── WithWebSocketSubprotocol()
                ├── WithWebSocketVersion()
                └── WithWebSocketOrigin()


IResponseBuilder (interface)
    ▲
    │
    └── Response (class)
            │
            ├── Response.cs (core)
            ├── Response.WithStatusCode.cs
            ├── Response.WithHeaders.cs
            ├── Response.WithBody.cs
            ├── Response.WithCallback.cs
            ├── Response.WithTransformer.cs
            ├── Response.WithProxy.cs
            ├── Response.WithFault.cs
            └── Response.WithWebSocket.cs [NEW]
                ├── WithWebSocket(builder)
                ├── WithWebSocketMessage()
                ├── WithWebSocketJsonMessage()
                ├── WithWebSocketBinaryMessage()
                ├── WithWebSocketCallback()
                ├── WithWebSocketTransformer()
                ├── WithWebSocketClose()
                ├── WithWebSocketSubprotocol()
                └── WithWebSocketAutoClose()


IWebSocketResponseBuilder (interface) [NEW]
    ▲
    │
    └── WebSocketResponseBuilder (class) [NEW]
            ├── WithMessage()
            ├── WithJsonMessage()
            ├── WithBinaryMessage()
            ├── WithTransformer()
            ├── WithClose()
            ├── WithSubprotocol()
            ├── WithAutoClose()
            └── Build() → IWebSocketResponse
```

---

## Mapping Configuration Chain

```
server.Given(request)
    ↓
IRespondWithAProvider
    ├── AtPriority(int)          ✓ HTTP & WebSocket
    ├── WithTitle(string)         ✓ HTTP & WebSocket
    ├── WithDescription(string)   ✓ HTTP & WebSocket
    ├── WithPath(string)          ✓ HTTP & WebSocket
    ├── InScenario(string)        ✓ HTTP & WebSocket
    ├── WhenStateIs(string)       ✓ HTTP & WebSocket
    ├── WillSetStateTo(string)    ✓ HTTP & WebSocket
    ├── WithWebhook(...)          ✓ HTTP & WebSocket
    ├── WithTimeSettings(...)     ✓ HTTP & WebSocket
    ├── WithGuid(Guid)            ✓ HTTP & WebSocket
    ├── WithData(object)          ✓ HTTP & WebSocket
    └── RespondWith(provider)
         ↓
         IResponseProvider
         ├── Response (HTTP)
         │   ├── WithStatusCode()
         │   ├── WithBody()
         │   ├── WithCallback()
         │   └── ...
         └── Response (WebSocket) [NEW]
             ├── WithWebSocket()
             ├── WithWebSocketCallback()
             └── ...
```

---

## Fluent API Method Chains

### Simple Echo Server

```
Request.Create()
    .WithWebSocketPath("/echo")
        ↓
Response.Create()
    .WithWebSocketCallback(async request =>
        new[] { new WebSocketMessage { BodyAsString = request.Body } }
    )
```

### Stream with Multiple Messages

```
Request.Create()
    .WithWebSocketPath("/stream")
        ↓
Response.Create()
    .WithWebSocket(ws => ws
        .WithMessage("Start", 0)
        .WithMessage("Middle", 500)
        .WithMessage("End", 1000)
        .WithClose(1000, "Complete")
    )
```

### Dynamic with Templates

```
Request.Create()
    .WithWebSocketPath("/api")
    .WithWebSocketSubprotocol("v2")
        ↓
Response.Create()
    .WithWebSocketSubprotocol("v2")
    .WithWebSocket(ws => ws
        .WithJsonMessage(new { 
            user = "{{request.headers.X-User}}" 
        })
        .WithTransformer()
    )
```

---

## Transformer Integration

```
WebSocket Response
    ├─ Raw Content
    │   └─ "Hello {{user}}, timestamp: {{now}}"
    │
    └─ WithTransformer() [Enable Handlebars/Scriban]
        ↓
        Transformer Engine (existing)
        ├─ Request context injection
        ├─ Helper methods (Math, String, etc.)
        └─ Custom helpers
            ↓
        Transformed Content
        └─ "Hello Alice, timestamp: 2024-01-15T10:30:00Z"
```

---

## Message Delivery Timeline

```
Client connects → WebSocket Upgrade
                     ↓
              Message Queue Created
                     ↓
┌──────────────────────────────────────────────────────────┐
│ Message 1 (delayMs: 0)                                   │
│ ═════════════════════════════════════════════════════► │
│ Sent immediately                                         │
└──────────────────────────────────────────────────────────┘
            ↓ (wait 500ms)
┌──────────────────────────────────────────────────────────┐
│ Message 2 (delayMs: 500)                                 │
│                 ═════════════════════════════════════► │
│ Sent at T+500ms                                          │
└──────────────────────────────────────────────────────────┘
            ↓ (wait 1000ms from start)
┌──────────────────────────────────────────────────────────┐
│ Message 3 (delayMs: 1000)                                │
│                         ═════════════════════════════► │
│ Sent at T+1000ms                                         │
└──────────────────────────────────────────────────────────┘
            ↓ (Close connection)
            Close Frame (1000, "Complete")
                     ↓
              Connection Closed
```

---

## File Organization

```
src/WireMock.Net.Abstractions/
│
├── Models/
│   ├── IWebSocketMessage.cs [NEW]
│   ├── IWebSocketResponse.cs [NEW]
│   └── ...existing models
│
├── Admin/Mappings/
│   ├── WebSocketModel.cs [NEW]
│   ├── ResponseModel.cs (extend for WebSocket)
│   ├── RequestModel.cs (extend for WebSocket)
│   └── ...existing models
│
├── BuilderExtensions/
│   ├── IWebSocketResponseBuilder.cs [NEW]
│   ├── WebSocketResponseModelBuilder.cs [NEW]
│   └── ...existing builders
│
└── ...rest of abstractions


src/WireMock.Net.Minimal/
│
├── Models/
│   ├── WebSocketMessage.cs [NEW]
│   ├── WebSocketResponse.cs [NEW]
│   └── ...existing models
│
├── RequestBuilders/
│   ├── Request.cs (update interfaces)
│   ├── Request.WithWebSocket.cs [NEW]
│   ├── Request.WithPath.cs
│   ├── Request.WithHeaders.cs
│   └── ...existing builders
│
├── ResponseBuilders/
│   ├── Response.cs (update interfaces)
│   ├── Response.WithWebSocket.cs [NEW]
│   ├── WebSocketResponseBuilder.cs [NEW]
│   ├── Response.WithStatusCode.cs
│   ├── Response.WithBody.cs
│   └── ...existing builders
│
├── Server/
│   ├── WireMockServer.cs (update for WebSocket)
│   ├── WireMockServer.Fluent.cs
│   ├── MappingBuilder.cs
│   ├── RespondWithAProvider.cs
│   └── ...existing server code
│
├── Owin/
│   ├── WireMockMiddleware.cs (add WebSocket upgrade)
│   ├── MappingMatcher.cs (add WebSocket routing)
│   └── ...existing OWIN code
│
└── ...rest of implementation
```

---

## Dependency Graph

```
┌─────────────────────────────────────────┐
│ External Dependencies                   │
├─────────────────────────────────────────┤
│ • .NET Standard 2.0 / .NET Framework    │
│ • ASP.NET Core (WebSocket support)      │
│ • Newtonsoft.Json (serialization)       │
│ • Handlebars.Core (transformers)        │
│ • Scriban (transformers)                │
└──────────────────┬──────────────────────┘
                   ▲
                   │
┌──────────────────┴──────────────────────┐
│ WireMock.Net.Abstractions                │
├──────────────────────────────────────────┤
│ • Interfaces (IRequestBuilder, etc.)    │
│ • Models (RequestModel, ResponseModel)  │
│ • WebSocket abstractions [NEW]          │
└──────────────────┬──────────────────────┘
                   ▲
                   │
┌──────────────────┴──────────────────────┐
│ WireMock.Net.Minimal                     │
├──────────────────────────────────────────┤
│ • Request builders                      │
│ • Response builders                     │
│ • WebSocket builders [NEW]              │
│ • Server core                           │
│ • OWIN middleware                       │
└──────────────────┬──────────────────────┘
                   ▲
                   │
┌──────────────────┴──────────────────────┐
│ WireMock.Net (Full)                      │
│ WireMock.Net.StandAlone (OWIN)          │
│ Application Code                         │
└──────────────────────────────────────────┘
```

---

## Test Coverage Areas

```
Unit Tests (Request/Response Builders)
├── WebSocketResponseBuilder tests
│   ├── Message ordering
│   ├── Delay handling
│   ├── Transformer support
│   └── Close frame handling
├── Request builder tests
│   ├── WebSocket path matching
│   ├── Subprotocol matching
│   └── Upgrade header validation
└── Integration tests
    ├── Client connection handling
    ├── Message delivery
    ├── Scenario state management
    ├── Concurrent connections
    ├── Connection timeout
    └── Error scenarios
```

---

## Phase Implementation Timeline

```
Week 1: Phase 1-2 (Abstractions & Models)
├─ Mon-Tue: Abstractions (IWebSocketMessage, etc.)
├─ Wed: Domain Models (WebSocketMessage, etc.)
└─ Thu: Code review & refinement

Week 2: Phase 3 (Request Builder)
├─ Mon-Tue: Request.WithWebSocket.cs
├─ Wed: Request matching tests
└─ Thu: Integration with server

Week 3: Phase 4 (Response Builder)
├─ Mon-Wed: Response.WithWebSocket.cs
├─ Wed-Thu: WebSocketResponseBuilder
└─ Fri: Message delivery tests

Week 4: Phase 5 (Server Integration)
├─ Mon-Tue: WireMockMiddleware updates
├─ Wed: Connection lifecycle management
├─ Thu: Integration tests
└─ Fri: Documentation & release prep
```

---

## Quick Reference: What's New vs What's Extended

```
┌─────────────────────┬─────────────────────┬──────────────────┐
│ Component           │ New [NEW]           │ Extended         │
├─────────────────────┼─────────────────────┼──────────────────┤
│ Request Builder     │ Request.            │ Request.cs       │
│                     │ WithWebSocket.cs    │ (interfaces)     │
├─────────────────────┼─────────────────────┼──────────────────┤
│ Response Builder    │ Response.           │ Response.cs      │
│                     │ WithWebSocket.cs    │ (interfaces)     │
│                     │ WebSocketResponse   │                  │
│                     │ Builder.cs          │                  │
├─────────────────────┼─────────────────────┼──────────────────┤
│ Domain Models       │ WebSocketMessage.cs │ None             │
│                     │ WebSocketResponse.  │                  │
│                     │ cs                  │                  │
├─────────────────────┼─────────────────────┼──────────────────┤
│ Admin API           │ WebSocketModel.cs   │ ResponseModel.cs │
│                     │                     │ RequestModel.cs  │
├─────────────────────┼─────────────────────┼──────────────────┤
│ Server              │ WebSocket           │ WireMock         │
│                     │ ConnectionManager   │ Server.cs        │
│                     │ [NEW]               │ WireMock         │
│                     │                     │ Middleware.cs    │
├─────────────────────┼─────────────────────┼──────────────────┤
│ Interfaces          │ IWebSocketMessage   │ IRequestBuilder  │
│                     │ IWebSocketResponse  │ IResponseBuilder │
│                     │ IWebSocketResponse  │                  │
│                     │ Builder             │                  │
└─────────────────────┴─────────────────────┴──────────────────┘
```

---

This visual guide helps understand the architecture, data flow, and implementation scope of the WebSocket support proposal.
