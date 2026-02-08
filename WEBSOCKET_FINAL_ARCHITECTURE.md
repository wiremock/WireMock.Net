# WebSocket Implementation - Final Architecture Summary

## ✅ REFACTORED TO EXTENSION METHODS PATTERN

The WebSocket implementation has been restructured to follow the **exact same pattern as WireMock.Net.ProtoBuf**, using extension methods instead of modifying core classes.

---

## 📐 Architecture Pattern

### Before (Incorrect)
```
WireMock.Net.Minimal/
├── RequestBuilders/Request.WebSocket.cs        ❌ Direct modification
└── ResponseBuilders/Response.WebSocket.cs      ❌ Direct modification
```

### After (Correct - Following ProtoBuf Pattern)
```
WireMock.Net.WebSockets/
├── RequestBuilders/IRequestBuilderExtensions.cs  ✅ Extension methods
└── ResponseBuilders/IResponseBuilderExtensions.cs ✅ Extension methods
```

---

## 🔌 Extension Methods Pattern

### Request Builder Extensions

```csharp
public static class IRequestBuilderExtensions
{
    public static IRequestBuilder WithWebSocketPath(this IRequestBuilder requestBuilder, string path)
    public static IRequestBuilder WithWebSocketSubprotocol(this IRequestBuilder requestBuilder, params string[] subProtocols)
    public static IRequestBuilder WithCustomHandshakeHeaders(this IRequestBuilder requestBuilder, params (string Key, string Value)[] headers)
}
```

### Response Builder Extensions

```csharp
public static class IResponseBuilderExtensions
{
    public static IResponseBuilder WithWebSocketHandler(this IResponseBuilder responseBuilder, Func<WebSocketHandlerContext, Task> handler)
    public static IResponseBuilder WithWebSocketHandler(this IResponseBuilder responseBuilder, Func<WebSocket, Task> handler)
    public static IResponseBuilder WithWebSocketMessageHandler(this IResponseBuilder responseBuilder, Func<WebSocketMessage, Task<WebSocketMessage?>> handler)
    public static IResponseBuilder WithWebSocketKeepAlive(this IResponseBuilder responseBuilder, TimeSpan interval)
    public static IResponseBuilder WithWebSocketTimeout(this IResponseBuilder responseBuilder, TimeSpan timeout)
    public static IResponseBuilder WithWebSocketMessage(this IResponseBuilder responseBuilder, WebSocketMessage message)
}
```

---

## 📦 Project Dependencies

### WireMock.Net.WebSockets
```xml
<ProjectReference Include="..\WireMock.Net.Shared\WireMock.Net.Shared.csproj" />
```
- **Only Dependency**: WireMock.Net.Shared
- **External Packages**: None (zero dependencies)
- **Target Frameworks**: netstandard2.1, net462, net6.0, net8.0

### WireMock.Net.Minimal
```xml
<!-- NO WebSocket dependency -->
<ProjectReference Include="..\WireMock.Net.Shared\WireMock.Net.Shared.csproj" />
```
- WebSockets is **completely optional**
- No coupling to WebSocket code

### WireMock.Net (main package)
```xml
<ProjectReference Include="../WireMock.Net.WebSockets/WireMock.Net.WebSockets.csproj" />
```
- Includes WebSockets for .NET 3.1+ when needed

---

## ✨ Benefits of Extension Method Pattern

1. **✅ Zero Coupling** - WebSocket code is completely separate
2. **✅ Optional Dependency** - Users can opt-in to WebSocket support
3. **✅ Clean API** - No modifications to core Request/Response classes
4. **✅ Discoverable** - Extension methods appear naturally in IntelliSense
5. **✅ Maintainable** - All WebSocket code lives in WebSockets project
6. **✅ Testable** - Can be tested independently
7. **✅ Consistent** - Matches ProtoBuf, GraphQL, and other optional features

---

## 📝 Usage Example

```csharp
// Extension methods automatically available when WebSockets package is included
server
    .Given(Request.Create()
        .WithPath("/ws")
        .WithWebSocketSubprotocol("chat")  // ← Extension method
    )
    .RespondWith(Response.Create()
        .WithWebSocketHandler(async ctx => {})  // ← Extension method
        .WithWebSocketKeepAlive(TimeSpan.FromSeconds(30))  // ← Extension method
    );
```

---

## 🗂️ File Structure

```
src/WireMock.Net.WebSockets/
├── WireMock.Net.WebSockets.csproj
├── GlobalUsings.cs
├── README.md
├── Models/
│   ├── WebSocketMessage.cs
│   ├── WebSocketHandlerContext.cs
│   └── WebSocketConnectRequest.cs
├── Matchers/
│   └── WebSocketRequestMatcher.cs
├── ResponseProviders/
│   └── WebSocketResponseProvider.cs
├── RequestBuilders/
│   └── IRequestBuilderExtensions.cs           ✅ Extension methods
└── ResponseBuilders/
    └── IResponseBuilderExtensions.cs          ✅ Extension methods
```

---

## ✅ Project References

| Project | Before | After |
|---------|--------|-------|
| **WireMock.Net.Minimal** | References WebSockets ❌ | No WebSocket ref ✅ |
| **WireMock.Net** | References WebSockets ✅ | References WebSockets ✅ |
| **WireMock.Net.WebSockets** | N/A | Only refs Shared ✅ |

---

## 🎯 Pattern Consistency

### Comparison with Existing Optional Features

| Feature | Pattern | Location | Dependency |
|---------|---------|----------|------------|
| **ProtoBuf** | Extension methods | WireMock.Net.ProtoBuf | Optional |
| **GraphQL** | Extension methods | WireMock.Net.GraphQL | Optional |
| **MimePart** | Extension methods | WireMock.Net.MimePart | Optional |
| **WebSockets** | Extension methods | WireMock.Net.WebSockets | **Optional** ✅ |

---

## 🚀 How It Works

### 1. User installs `WireMock.Net`
   - Gets HTTP/REST mocking
   - WebSocket support included but optional

### 2. User uses WebSocket extensions
   ```csharp
   using WireMock.WebSockets;  // Brings in extension methods
   
   // Extension methods now available
   server.Given(Request.Create().WithWebSocketPath("/ws"))
   ```

### 3. Behind the scenes
   - Extension methods call WebSocket matchers
   - WebSocket configuration stored separately
   - Middleware can check for WebSocket config
   - Handler invoked if WebSocket is configured

---

## 📊 Code Organization

### Extension Method Storage

Response builder uses `ConditionalWeakTable<IResponseBuilder, WebSocketConfiguration>` to store WebSocket settings without modifying the original Response class:

```csharp
private static readonly ConditionalWeakTable<IResponseBuilder, WebSocketConfiguration> WebSocketConfigs = new();

internal class WebSocketConfiguration
{
    public Func<WebSocketHandlerContext, Task>? Handler { get; set; }
    public Func<WebSocketMessage, Task<WebSocketMessage?>>? MessageHandler { get; set; }
    public TimeSpan? KeepAliveInterval { get; set; }
    public TimeSpan? Timeout { get; set; }
}
```

This allows:
- Zero modifications to Response class ✅
- Clean separation of concerns ✅
- No performance impact on non-WebSocket code ✅
- Thread-safe configuration storage ✅

---

## ✅ Compilation Status

- **Errors**: 0
- **Warnings**: 0  
- **Dependencies**: Only WireMock.Net.Shared
- **External Packages**: None
- **Pattern**: Matches ProtoBuf exactly ✅

---

## 🎓 Summary

The WebSocket implementation now:

1. ✅ Follows the **ProtoBuf extension method pattern**
2. ✅ Has **zero external dependencies**
3. ✅ Is **completely optional** (no WireMock.Net.Minimal coupling)
4. ✅ Uses **ConditionalWeakTable** for configuration storage
5. ✅ Provides a **clean, discoverable API**
6. ✅ Maintains **full backward compatibility**
7. ✅ **Compiles without errors or warnings**

The implementation is now properly architected, following WireMock.Net's established patterns for optional features!

