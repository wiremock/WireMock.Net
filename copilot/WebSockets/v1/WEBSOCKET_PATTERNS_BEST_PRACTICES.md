# WebSocket Design Patterns - Visual Guide & Best Practices

## Overview

This document provides visual examples and best practices for using WebSocket support in WireMock.Net following the established fluent interface patterns.

---

## Part 1: Pattern Evolution in WireMock.Net

### HTTP Request Matching Pattern

```
┌─────────────┐
│ Request.    │ Fluent methods return IRequestBuilder
│ Create()    │ for chaining
│             │
├─────────────┤
│ WithPath    │
├─────────────┤
│ WithHeader  │
├─────────────┤
│ UsingGet()  │  Each partial class file handles
├─────────────┤  one concern (path, headers, method)
│ WithParam   │
├─────────────┤
│ WithBody    │
└─────────────┘
```

### HTTP Response Building Pattern

```
┌──────────────┐
│ Response.    │
│ Create()     │
│              │
├──────────────┤
│ WithStatus   │
├──────────────┤
│ WithHeader   │
├──────────────┤
│ WithBody     │  Returns IResponseBuilder
├──────────────┤  for chaining
│ WithDelay    │
├──────────────┤
│ WithTransf.  │  Transformer for template
├──────────────┤  substitution
│ WithCallback │  Dynamic responses
└──────────────┘
```

### WebSocket Extension Pattern (New)

```
┌──────────────────────┐
│ Request.Create()     │
├──────────────────────┤
│ WithWebSocketPath    │
├──────────────────────┤
│ WithWebSocketSubprot │  Extends request builder
├──────────────────────┤  with WebSocket-specific
│ WithWebSocketOrigin  │  matching
└──────────────────────┘
           │
           ↓
┌──────────────────────┐
│ Response.Create()    │
├──────────────────────┤
│ WithWebSocket()      │
│ ├─ WithMessage()     │  Fluent builder for
│ ├─ WithJsonMessage() │  composing messages
│ └─ WithTransformer() │
├──────────────────────┤
│ WithWebSocketCallback│  Dynamic message gen
├──────────────────────┤
│ WithWebSocketClose() │  Graceful shutdown
└──────────────────────┘
```

---

## Part 2: Usage Pattern Comparison

### Pattern 1: Static Messages

**Analogy**: Pre-recorded HTTP responses

```csharp
// HTTP (existing)
server.Given(Request.Create().WithPath("/api/users"))
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithBodyAsJson(new { id = 1, name = "John" })
    );

// WebSocket (new - sequential messages)
server.Given(Request.Create().WithWebSocketPath("/api/users/stream"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { type = "connected" }, delayMs: 0)
            .WithJsonMessage(new { id = 1, name = "John" }, delayMs: 500)
            .WithJsonMessage(new { id = 2, name = "Jane" }, delayMs: 1000)
            .WithClose(1000, "Stream complete")
        )
    );
```

### Pattern 2: Dynamic Content (Request-Based)

**Analogy**: Response callbacks

```csharp
// HTTP (existing)
server.Given(Request.Create().WithPath("/api/echo"))
    .RespondWith(Response.Create()
        .WithCallback(request =>
        {
            return new ResponseMessage
            {
                BodyData = new BodyData
                {
                    BodyAsString = $"Echo: {request.Body}",
                    DetectedBodyType = BodyType.String
                },
                StatusCode = 200
            };
        })
    );

// WebSocket (new - message stream from request)
server.Given(Request.Create().WithWebSocketPath("/echo"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async request =>
        {
            // Generate messages based on request context
            return new[]
            {
                new WebSocketMessage
                {
                    BodyAsString = $"Echo: {request.Body}",
                    DelayMs = 100,
                    IsText = true
                }
            };
        })
    );
```

### Pattern 3: Templating (Dynamic Values)

**Analogy**: Handlebars/Scriban transformers

```csharp
// HTTP (existing)
server.Given(Request.Create().WithPath("/api/user"))
    .RespondWith(Response.Create()
        .WithBodyAsJson(new { 
            username = "{{request.headers.X-User-Name}}",
            timestamp = "{{now}}"
        })
        .WithTransformer()
    );

// WebSocket (new - template in messages)
server.Given(Request.Create().WithWebSocketPath("/notifications"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { 
                user = "{{request.headers.X-User-Name}}",
                connected = "{{now}}"
            })
            .WithJsonMessage(new { 
                message = "Hello {{request.headers.X-User-Name}}"
            }, delayMs: 1000)
            .WithTransformer()
        )
    );
```

### Pattern 4: Metadata (Scenario State)

**Analogy**: Scenario state management

```csharp
// HTTP (existing)
server.Given(Request.Create().WithPath("/login"))
    .InScenario("UserWorkflow")
    .WillSetStateTo("LoggedIn")
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithBodyAsJson(new { success = true })
    );

// WebSocket (new - state in WebSocket flow)
server.Given(Request.Create().WithWebSocketPath("/chat"))
    .InScenario("ChatSession")
    .WhenStateIs("LoggedIn")
    .WillSetStateTo("ChatActive")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { type = "welcome" })
            .WithJsonMessage(new { type = "ready" })
        )
    );
```

### Pattern 5: Extensions (Webhooks)

**Analogy**: Side-effects during request handling

```csharp
// HTTP (existing) - Trigger external webhook
server.Given(Request.Create().WithPath("/process"))
    .WithWebhook(new Webhook
    {
        Request = new WebhookRequest
        {
            Url = "http://external-service/notify",
            Method = "post",
            BodyData = new BodyData { BodyAsString = "Processing..." }
        }
    })
    .RespondWith(Response.Create().WithStatusCode(200));

// WebSocket (new) - Webhook triggered by connection
server.Given(Request.Create().WithWebSocketPath("/events"))
    .WithWebhook(new Webhook
    {
        Request = new WebhookRequest
        {
            Url = "http://audit-log/event",
            Method = "post",
            BodyData = new BodyData { 
                BodyAsString = "WebSocket connected: {{request.url}}"
            }
        }
    })
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws.WithMessage("Connected"))
    );
```

---

## Part 3: Real-World Scenarios

### Scenario 1: Real-time Chat Server

```csharp
// Simulate multiple users joining a chat room
server.Given(Request.Create()
    .WithWebSocketPath("/chat")
    .WithHeader("X-Room-Id", "room123"))
    .InScenario("ChatRoom")
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(
                new { type = "user-joined", username = "{{request.headers.X-Username}}" },
                delayMs: 0)
            .WithJsonMessage(
                new { type = "message", from = "System", text = "Welcome to room123" },
                delayMs: 500)
            .WithJsonMessage(
                new { type = "users-online", count = 3 },
                delayMs: 1000)
            .WithTransformer()
        )
        .WithWebhook(new Webhook  // Audit log
        {
            Request = new WebhookRequest
            {
                Url = "http://audit/log",
                Method = "post",
                BodyData = new BodyData
                {
                    BodyAsString = "User {{request.headers.X-Username}} joined {{request.headers.X-Room-Id}}"
                }
            }
        })
    );

// Handle user messages (dynamic, simulates echo)
server.Given(Request.Create().WithWebSocketPath("/chat"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async request =>
        {
            var username = request.Headers["X-Username"]?.FirstOrDefault() ?? "Anonymous";
            var messageBody = request.Body ?? "";

            return new[]
            {
                new WebSocketMessage
                {
                    BodyAsString = JsonConvert.SerializeObject(new
                    {
                        type = "message-received",
                        from = username,
                        text = messageBody,
                        timestamp = DateTime.UtcNow
                    }),
                    DelayMs = 100
                },
                new WebSocketMessage
                {
                    BodyAsString = JsonConvert.SerializeObject(new
                    {
                        type = "acknowledgment",
                        status = "delivered"
                    }),
                    DelayMs = 200
                }
            };
        })
        .WithWebSocketTransformer()
    );
```

### Scenario 2: Real-time Data Streaming

```csharp
// Stream stock market data
server.Given(Request.Create()
    .WithWebSocketPath("/market-data")
    .WithWebSocketSubprotocol("market.v1"))
    .WithTitle("Market Data Stream")
    .WithDescription("Real-time stock market prices")
    .RespondWith(Response.Create()
        .WithWebSocketSubprotocol("market.v1")
        .WithWebSocketCallback(async request =>
        {
            var ticker = request.Headers.ContainsKey("X-Ticker") 
                ? request.Headers["X-Ticker"].First() 
                : "AAPL";

            var messages = new List<WebSocketMessage>();
            
            // Initial subscription confirmation
            messages.Add(new WebSocketMessage
            {
                BodyAsString = JsonConvert.SerializeObject(new
                {
                    type = "subscribed",
                    ticker = ticker,
                    timestamp = DateTime.UtcNow
                }),
                DelayMs = 0
            });

            // Simulate price updates
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                var price = 150.00m + (decimal)random.NextDouble() * 10;
                messages.Add(new WebSocketMessage
                {
                    BodyAsString = JsonConvert.SerializeObject(new
                    {
                        type = "price-update",
                        ticker = ticker,
                        price = price,
                        timestamp = DateTime.UtcNow
                    }),
                    DelayMs = (i + 1) * 1000  // 1 second between updates
                });
            }

            // Final close message
            messages.Add(new WebSocketMessage
            {
                BodyAsString = JsonConvert.SerializeObject(new
                {
                    type = "stream-end",
                    reason = "Demo ended"
                }),
                DelayMs = 6000
            });

            return messages;
        })
    );
```

### Scenario 3: Server Push Notifications

```csharp
// Long-lived connection for push notifications
server.Given(Request.Create()
    .WithWebSocketPath("/push")
    .WithHeader("Authorization", new WildcardMatcher("Bearer *")))
    .AtPriority(1)  // Higher priority
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithJsonMessage(new
            {
                type = "authenticated",
                user = "{{request.headers.Authorization}}",
                connectedAt = "{{now}}"
            }, delayMs: 0)
            .WithJsonMessage(new
            {
                type = "notification",
                title = "System Update",
                message = "A new update is available"
            }, delayMs: 3000)
            .WithJsonMessage(new
            {
                type = "notification",
                title = "New Message",
                message = "You have a new message from admin"
            }, delayMs: 6000)
            .WithTransformer()
            .WithClose(1000, "Connection closed by server")
        )
        .WithWebSocketAutoClose(30000)  // Auto-close after 30 seconds if idle
    );
```

### Scenario 4: GraphQL Subscription Simulation

```csharp
// Simulate GraphQL subscription (persistent query updates)
server.Given(Request.Create()
    .WithWebSocketPath("/graphql")
    .WithWebSocketSubprotocol("graphql-ws")
    .WithHeader("Content-Type", "application/json"))
    .RespondWith(Response.Create()
        .WithWebSocketSubprotocol("graphql-ws")
        .WithWebSocketCallback(async request =>
        {
            var messages = new List<WebSocketMessage>();

            // Parse subscription query from request
            var query = request.Body ?? "{}";

            // Connection ACK
            messages.Add(new WebSocketMessage
            {
                BodyAsString = JsonConvert.SerializeObject(new
                {
                    type = "connection_ack"
                }),
                DelayMs = 0,
                IsText = true
            });

            // Data messages
            for (int i = 0; i < 3; i++)
            {
                messages.Add(new WebSocketMessage
                {
                    BodyAsString = JsonConvert.SerializeObject(new
                    {
                        type = "data",
                        id = "1",
                        payload = new
                        {
                            data = new
                            {
                                userNotifications = new[] { new { id = i, message = $"Update {i}" } }
                            }
                        }
                    }),
                    DelayMs = (i + 1) * 2000,
                    IsText = true
                });
            }

            // Complete
            messages.Add(new WebSocketMessage
            {
                BodyAsString = JsonConvert.SerializeObject(new
                {
                    type = "complete",
                    id = "1"
                }),
                DelayMs = 6000,
                IsText = true
            });

            return messages;
        })
    );
```

---

## Part 4: Best Practices

### ✅ DO: Follow Request Matching Patterns

```csharp
// Good: Follows established request builder pattern
server.Given(Request.Create()
    .WithWebSocketPath("/api/notifications")
    .WithWebSocketSubprotocol("notifications")
    .WithHeader("Authorization", "Bearer *")
)
```

### ❌ DON'T: Overload builders with raw configuration

```csharp
// Bad: Breaks fluent pattern
var req = new Request(...);
req.webSocketSettings = new { ... };
```

### ✅ DO: Use callbacks for dynamic behavior

```csharp
// Good: Dynamic based on request context
.WithWebSocketCallback(async request =>
{
    var userId = request.Headers["X-User-Id"].First();
    return GetMessagesForUser(userId);
})
```

### ❌ DON'T: Mix static and dynamic in same mapping

```csharp
// Bad: Confusing multiple patterns
.WithWebSocket(ws => ws.WithMessage("Static"))
.WithWebSocketCallback(async r => new[] { ... })  // Which wins?
```

### ✅ DO: Use transformers for templating

```csharp
// Good: Dynamic values via templates
.WithJsonMessage(new 
{ 
    userId = "{{request.headers.X-User-Id}}"
})
.WithTransformer()
```

### ❌ DON'T: Hardcode request values

```csharp
// Bad: Doesn't adapt to different requests
.WithJsonMessage(new { userId = "hardcoded-user-123" })
```

### ✅ DO: Set appropriate delays for realistic simulation

```csharp
// Good: Simulates realistic network latency
.WithJsonMessage(msg1, delayMs: 0)       // Immediate
.WithJsonMessage(msg2, delayMs: 500)     // 500ms later
.WithJsonMessage(msg3, delayMs: 2000)    // 2 seconds later
```

### ❌ DON'T: Use excessively long delays

```csharp
// Bad: Test hangs unnecessarily
.WithJsonMessage(msg, delayMs: 60000)  // 1 minute?
```

### ✅ DO: Use subprotocol negotiation for versioning

```csharp
// Good: Version the API
.WithWebSocketPath("/api")
.WithWebSocketSubprotocol("api.v2")
```

### ❌ DON'T: Embed version in path alone

```csharp
// Bad: Less testable for version negotiation
.WithWebSocketPath("/api/v2")
```

### ✅ DO: Chain metadata methods logically

```csharp
// Good: Clear order (matching → metadata → response)
server.Given(Request.Create().WithWebSocketPath("/api"))
    .AtPriority(1)
    .WithTitle("WebSocket API")
    .InScenario("ActiveConnections")
    .WithWebhook(...)
    .RespondWith(Response.Create()...);
```

### ✅ DO: Test both happy path and error scenarios

```csharp
// Connection accepted
server.Given(Request.Create().WithWebSocketPath("/api").WithHeader("Auth", "*"))
    .RespondWith(Response.Create().WithWebSocket(...));

// Connection rejected
server.Given(Request.Create().WithWebSocketPath("/api").WithHeader("Auth", "invalid"))
    .RespondWith(Response.Create().WithStatusCode(401));
```

---

## Part 5: Fluent Chain Examples

### Example 1: Minimal Setup

```csharp
server.Given(Request.Create()
    .WithWebSocketPath("/ws"))
    .RespondWith(Response.Create()
        .WithWebSocketMessage("Connected")
    );
```

### Example 2: Full-Featured Setup

```csharp
server.Given(Request.Create()
    .WithWebSocketPath("/api/events")
    .WithWebSocketSubprotocol("events.v1")
    .WithHeader("Authorization", "Bearer *")
    .WithHeader("X-Client-Id", "*")
)
    .AtPriority(10)
    .WithTitle("Event Stream API")
    .WithDescription("Real-time event streaming for client ID")
    .InScenario("EventStreaming")
    .WhenStateIs("Connected")
    .WillSetStateTo("StreamActive")
    .WithWebhook(new Webhook
    {
        Request = new WebhookRequest
        {
            Url = "http://audit/connections",
            Method = "post",
            BodyData = new BodyData
            {
                BodyAsString = "Client {{request.headers.X-Client-Id}} connected"
            }
        }
    })
    .RespondWith(Response.Create()
        .WithWebSocketSubprotocol("events.v1")
        .WithWebSocket(ws => ws
            .WithJsonMessage(new
            {
                type = "connected",
                clientId = "{{request.headers.X-Client-Id}}",
                timestamp = "{{now}}"
            }, delayMs: 0)
            .WithJsonMessage(new
            {
                type = "status",
                status = "ready"
            }, delayMs: 100)
            .WithTransformer()
            .WithClose(1000, "Graceful shutdown")
        )
        .WithWebSocketAutoClose(300000)  // 5 minute timeout
    );
```

---

## Summary

The WebSocket fluent interface design:

1. **Extends, not replaces** existing request/response builders
2. **Follows established patterns** (partial classes, method chaining)
3. **Enables composition** (messages, transformers, callbacks)
4. **Maintains readability** (clear fluent chains)
5. **Supports testing** (realistic delays, state, scenarios)
6. **Integrates seamlessly** (webhooks, priority, metadata)

This ensures developers have a consistent, intuitive API for mocking WebSocket behavior.
