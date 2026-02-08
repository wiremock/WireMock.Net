# WebSocket Implementation Code Templates

This file contains ready-to-use code templates for implementing WebSocket support in WireMock.Net.Minimal.

---

## 1. Abstraction Layer (WireMock.Net.Abstractions)

### File: `Admin/Mappings/WebSocketModel.cs`

```csharp
// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// WebSocket message model for admin API serialization
/// </summary>
public class WebSocketMessageModel
{
    /// <summary>
    /// Delay before sending this message (in milliseconds)
    /// </summary>
    public int? DelayMs { get; set; }

    /// <summary>
    /// Text message payload
    /// </summary>
    public string? BodyAsString { get; set; }

    /// <summary>
    /// Binary message payload (base64 encoded when serialized)
    /// </summary>
    public byte[]? BodyAsBytes { get; set; }

    /// <summary>
    /// Indicates if message is text (true) or binary (false)
    /// </summary>
    public bool IsText { get; set; } = true;

    /// <summary>
    /// Message index (for ordering)
    /// </summary>
    public int Index { get; set; }
}

/// <summary>
/// WebSocket response configuration model
/// </summary>
public class WebSocketResponseModel
{
    /// <summary>
    /// Messages to send after WebSocket connection is established
    /// </summary>
    public List<WebSocketMessageModel> Messages { get; set; } = new();

    /// <summary>
    /// Whether to apply transformers to message content
    /// </summary>
    public bool UseTransformer { get; set; }

    /// <summary>
    /// Type of transformer to use (Handlebars, Scriban, etc.)
    /// </summary>
    public string? TransformerType { get; set; }

    /// <summary>
    /// Close code when connection is terminated (default 1000 = normal closure)
    /// </summary>
    public int? CloseCode { get; set; }

    /// <summary>
    /// Close reason/message
    /// </summary>
    public string? CloseMessage { get; set; }

    /// <summary>
    /// Indicates if a callback is used for dynamic message generation
    /// </summary>
    public bool HasCallback { get; set; }

    /// <summary>
    /// Subprotocol if negotiated
    /// </summary>
    public string? Subprotocol { get; set; }
}
```

### File: `BuilderExtensions/WebSocketResponseModelBuilder.cs`

```csharp
// Copyright © WireMock.Net

// ReSharper disable once CheckNamespace
namespace WireMock.Admin.Mappings;

/// <summary>
/// Fluent builder for WebSocket response models
/// </summary>
public partial class WebSocketResponseModelBuilder
{
    public WebSocketResponseModelBuilder WithMessage(string body, int? delayMs = null)
    {
        Messages.Add(new WebSocketMessageModel
        {
            BodyAsString = body,
            DelayMs = delayMs,
            IsText = true,
            Index = Messages.Count
        });

        return this;
    }

    public WebSocketResponseModelBuilder WithBinaryMessage(byte[] data, int? delayMs = null)
    {
        Messages.Add(new WebSocketMessageModel
        {
            BodyAsBytes = data,
            DelayMs = delayMs,
            IsText = false,
            Index = Messages.Count
        });

        return this;
    }

    public WebSocketResponseModelBuilder WithClose(int? closeCode = 1000, string? reason = null)
    {
        CloseCode = closeCode;
        CloseMessage = reason;

        return this;
    }

    public WebSocketResponseModelBuilder WithTransformer(string transformerType)
    {
        UseTransformer = true;
        TransformerType = transformerType;

        return this;
    }

    public List<WebSocketMessageModel> Messages { get; } = new();

    public bool UseTransformer { get; set; }

    public string? TransformerType { get; set; }

    public int? CloseCode { get; set; }

    public string? CloseMessage { get; set; }

    public WebSocketResponseModel Build()
    {
        return new WebSocketResponseModel
        {
            Messages = Messages,
            UseTransformer = UseTransformer,
            TransformerType = TransformerType,
            CloseCode = CloseCode,
            CloseMessage = CloseMessage
        };
    }
}
```

---

## 2. Domain Models (WireMock.Net.Minimal)

### File: `Models/WebSocketMessage.cs`

```csharp
// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// Represents a single WebSocket message to be sent
/// </summary>
public class WebSocketMessage : IWebSocketMessage
{
    /// <summary>
    /// Delay before sending this message (milliseconds)
    /// </summary>
    public int DelayMs { get; set; }

    /// <summary>
    /// Text message payload
    /// </summary>
    public string? BodyAsString { get; set; }

    /// <summary>
    /// Binary message payload
    /// </summary>
    public byte[]? BodyAsBytes { get; set; }

    /// <summary>
    /// True if text message, false if binary
    /// </summary>
    public bool IsText { get; set; } = true;

    /// <summary>
    /// Optional message ID for tracking
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Optional correlation ID from request
    /// </summary>
    public string? CorrelationId { get; set; }
}
```

### File: `Models/WebSocketResponse.cs`

```csharp
// Copyright © WireMock.Net

using WireMock.Transformers;
using WireMock.Types;

namespace WireMock.Models;

/// <summary>
/// Defines the WebSocket response configuration
/// </summary>
public class WebSocketResponse : IWebSocketResponse
{
    /// <summary>
    /// Messages to send after connection established
    /// </summary>
    public List<IWebSocketMessage> Messages { get; set; } = new();

    /// <summary>
    /// Whether to apply transformers to messages
    /// </summary>
    public bool UseTransformer { get; set; }

    /// <summary>
    /// Type of transformer (Handlebars or Scriban)
    /// </summary>
    public TransformerType TransformerType { get; set; } = TransformerType.Handlebars;

    /// <summary>
    /// WebSocket close code
    /// </summary>
    public int? CloseCode { get; set; } = 1000;

    /// <summary>
    /// Close reason
    /// </summary>
    public string? CloseMessage { get; set; }

    /// <summary>
    /// Optional subprotocol negotiation
    /// </summary>
    public string? Subprotocol { get; set; }

    /// <summary>
    /// Time to wait before closing (if empty close message list, milliseconds)
    /// </summary>
    public int? AutoCloseDelayMs { get; set; }
}
```

---

## 3. Request Builder Extension (WireMock.Net.Minimal)

### File: `RequestBuilders/Request.WithWebSocket.cs`

```csharp
// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// WebSocket-specific request matching extensions
/// </summary>
public partial class Request
{
    /// <summary>
    /// Match WebSocket upgrade requests (Connection: Upgrade, Upgrade: websocket headers)
    /// </summary>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketUpgrade()
    {
        Add(new RequestMessageHeaderMatcher("Upgrade", "websocket", matchBehaviour: MatchBehaviour.AcceptOnMatch));
        Add(new RequestMessageHeaderMatcher("Connection", new WildcardMatcher("*Upgrade*")));

        return this;
    }

    /// <summary>
    /// Match WebSocket connection by path and automatically add upgrade headers
    /// </summary>
    /// <param name="path">WebSocket path (e.g., "/ws", "/api/chat")</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketPath(string path)
    {
        Guard.NotNullOrWhiteSpace(path);

        return WithPath(path).WithWebSocketUpgrade();
    }

    /// <summary>
    /// Match specific WebSocket subprotocol in Sec-WebSocket-Protocol header
    /// </summary>
    /// <param name="subprotocol">Subprotocol name (e.g., "chat", "superchat")</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketSubprotocol(string subprotocol)
    {
        Guard.NotNullOrWhiteSpace(subprotocol);

        Add(new RequestMessageHeaderMatcher("Sec-WebSocket-Protocol", subprotocol));

        return this;
    }

    /// <summary>
    /// Match WebSocket with specific version (typically 13)
    /// </summary>
    /// <param name="version">WebSocket version number</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketVersion(string version = "13")
    {
        Guard.NotNullOrWhiteSpace(version);

        Add(new RequestMessageHeaderMatcher("Sec-WebSocket-Version", version));

        return this;
    }

    /// <summary>
    /// Match WebSocket with client origin (CORS)
    /// </summary>
    /// <param name="origin">Origin URL</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketOrigin(string origin)
    {
        Guard.NotNullOrWhiteSpace(origin);

        Add(new RequestMessageHeaderMatcher("Origin", origin));

        return this;
    }
}
```

---

## 4. Response Builder Extension (WireMock.Net.Minimal)

### File: `ResponseBuilders/Response.WithWebSocket.cs`

```csharp
// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Models;
using WireMock.Transformers;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// WebSocket response builder extensions
/// </summary>
public partial class Response
{
    /// <summary>
    /// WebSocket response configuration
    /// </summary>
    public IWebSocketResponse? WebSocketResponse { get; private set; }

    /// <summary>
    /// Indicates if WithWebSocket was used
    /// </summary>
    public bool WithWebSocketUsed { get; private set; }

    /// <summary>
    /// Callback for dynamic WebSocket message generation
    /// </summary>
    [MemberNotNullWhen(true, nameof(WithWebSocketCallbackUsed))]
    public Func<IRequestMessage, Task<IEnumerable<IWebSocketMessage>>>? WebSocketCallbackAsync { get; private set; }

    /// <summary>
    /// Indicates if WebSocket callback is used
    /// </summary>
    public bool WithWebSocketCallbackUsed => WebSocketCallbackAsync != null;

    /// <summary>
    /// Subprotocol to negotiate in WebSocket handshake
    /// </summary>
    public string? WebSocketSubprotocol { get; private set; }

    /// <summary>
    /// Configure WebSocket response with fluent builder
    /// </summary>
    /// <param name="configure">Configuration action</param>
    /// <returns>Response builder for chaining</returns>
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
    /// Configure WebSocket response with a single message
    /// </summary>
    /// <param name="message">Message text</param>
    /// <param name="delayMs">Optional delay before sending (milliseconds)</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketMessage(string message, int? delayMs = null)
    {
        Guard.NotNullOrWhiteSpace(message);

        return WithWebSocket(b => b.WithMessage(message, delayMs));
    }

    /// <summary>
    /// Configure WebSocket response with JSON message
    /// </summary>
    /// <param name="data">Object to serialize as JSON</param>
    /// <param name="delayMs">Optional delay before sending (milliseconds)</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketJsonMessage(object data, int? delayMs = null)
    {
        Guard.NotNull(data);

        return WithWebSocket(b => b.WithJsonMessage(data, delayMs));
    }

    /// <summary>
    /// Configure WebSocket response with binary message
    /// </summary>
    /// <param name="data">Binary payload</param>
    /// <param name="delayMs">Optional delay before sending (milliseconds)</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketBinaryMessage(byte[] data, int? delayMs = null)
    {
        Guard.NotNull(data);

        return WithWebSocket(b => b.WithBinaryMessage(data, delayMs));
    }

    /// <summary>
    /// Configure dynamic WebSocket messages via callback
    /// </summary>
    /// <param name="handler">Async handler to generate messages based on request</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketCallback(
        Func<IRequestMessage, Task<IEnumerable<IWebSocketMessage>>> handler)
    {
        Guard.NotNull(handler);

        WithWebSocketUsed = true;
        WebSocketCallbackAsync = handler;

        return this;
    }

    /// <summary>
    /// Configure dynamic WebSocket messages via synchronous callback
    /// </summary>
    /// <param name="handler">Handler to generate messages based on request</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketCallback(
        Func<IRequestMessage, IEnumerable<IWebSocketMessage>> handler)
    {
        Guard.NotNull(handler);

        return WithWebSocketCallback(req =>
            Task.FromResult(handler(req))
        );
    }

    /// <summary>
    /// Enable transformer for WebSocket messages
    /// </summary>
    /// <param name="use">Whether to apply transformer</param>
    /// <param name="transformerType">Transformer type (Handlebars or Scriban)</param>
    /// <returns>Response builder for chaining</returns>
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
    /// Configure WebSocket close frame for graceful disconnect
    /// </summary>
    /// <param name="closeCode">WebSocket close code (default 1000 = normal)</param>
    /// <param name="reason">Optional close reason</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketClose(int closeCode = 1000, string? reason = null)
    {
        if (WebSocketResponse != null)
        {
            WebSocketResponse.CloseCode = closeCode;
            WebSocketResponse.CloseMessage = reason;
        }

        return this;
    }

    /// <summary>
    /// Configure WebSocket subprotocol for negotiation
    /// </summary>
    /// <param name="subprotocol">Subprotocol name</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketSubprotocol(string subprotocol)
    {
        Guard.NotNullOrWhiteSpace(subprotocol);

        WebSocketSubprotocol = subprotocol;

        if (WebSocketResponse != null)
        {
            WebSocketResponse.Subprotocol = subprotocol;
        }

        return this;
    }

    /// <summary>
    /// Auto-close WebSocket connection after delay if no messages
    /// </summary>
    /// <param name="delayMs">Delay before auto-close (milliseconds)</param>
    /// <returns>Response builder for chaining</returns>
    public IResponseBuilder WithWebSocketAutoClose(int delayMs)
    {
        Guard.GreaterThan(delayMs, 0);

        if (WebSocketResponse != null)
        {
            WebSocketResponse.AutoCloseDelayMs = delayMs;
        }

        return this;
    }
}
```

---

## 5. WebSocket Response Builder (WireMock.Net.Minimal)

### File: `ResponseBuilders/WebSocketResponseBuilder.cs`

```csharp
// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Models;
using WireMock.Transformers;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// Fluent builder for WebSocket responses
/// </summary>
public class WebSocketResponseBuilder : IWebSocketResponseBuilder
{
    private readonly List<IWebSocketMessage> _messages = new();
    private bool _useTransformer;
    private TransformerType _transformerType = TransformerType.Handlebars;
    private int? _closeCode = 1000;
    private string? _closeMessage;
    private string? _subprotocol;
    private int? _autoCloseDelayMs;

    /// <summary>
    /// Add a text message
    /// </summary>
    public IWebSocketResponseBuilder WithMessage(string message, int? delayMs = null)
    {
        Guard.NotNullOrWhiteSpace(message);

        _messages.Add(new WebSocketMessage
        {
            BodyAsString = message,
            DelayMs = delayMs ?? 0,
            IsText = true,
            Id = Guid.NewGuid().ToString()
        });

        return this;
    }

    /// <summary>
    /// Add binary message
    /// </summary>
    public IWebSocketResponseBuilder WithBinaryMessage(byte[] data, int? delayMs = null)
    {
        Guard.NotNull(data);

        _messages.Add(new WebSocketMessage
        {
            BodyAsBytes = data,
            DelayMs = delayMs ?? 0,
            IsText = false,
            Id = Guid.NewGuid().ToString()
        });

        return this;
    }

    /// <summary>
    /// Add JSON message (auto-serialized)
    /// </summary>
    public IWebSocketResponseBuilder WithJsonMessage(object data, int? delayMs = null)
    {
        Guard.NotNull(data);

        var json = JsonConvert.SerializeObject(data);

        _messages.Add(new WebSocketMessage
        {
            BodyAsString = json,
            DelayMs = delayMs ?? 0,
            IsText = true,
            Id = Guid.NewGuid().ToString()
        });

        return this;
    }

    /// <summary>
    /// Enable message transformation
    /// </summary>
    public IWebSocketResponseBuilder WithTransformer(
        bool use = true,
        TransformerType transformerType = TransformerType.Handlebars)
    {
        _useTransformer = use;
        _transformerType = transformerType;

        return this;
    }

    /// <summary>
    /// Configure connection close
    /// </summary>
    public IWebSocketResponseBuilder WithClose(int closeCode = 1000, string? reason = null)
    {
        _closeCode = closeCode;
        _closeMessage = reason;

        return this;
    }

    /// <summary>
    /// Set subprotocol for negotiation
    /// </summary>
    public IWebSocketResponseBuilder WithSubprotocol(string subprotocol)
    {
        Guard.NotNullOrWhiteSpace(subprotocol);

        _subprotocol = subprotocol;

        return this;
    }

    /// <summary>
    /// Auto-close after delay if no more messages
    /// </summary>
    public IWebSocketResponseBuilder WithAutoClose(int delayMs)
    {
        Guard.GreaterThan(delayMs, 0);

        _autoCloseDelayMs = delayMs;

        return this;
    }

    /// <summary>
    /// Build the WebSocket response
    /// </summary>
    public IWebSocketResponse Build()
    {
        return new WebSocketResponse
        {
            Messages = _messages,
            UseTransformer = _useTransformer,
            TransformerType = _transformerType,
            CloseCode = _closeCode,
            CloseMessage = _closeMessage,
            Subprotocol = _subprotocol,
            AutoCloseDelayMs = _autoCloseDelayMs
        };
    }
}
```

---

## 6. Interfaces (WireMock.Net.Abstractions)

### File: `Models/IWebSocketMessage.cs`

```csharp
// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// Represents a WebSocket message to be sent
/// </summary>
public interface IWebSocketMessage
{
    int DelayMs { get; set; }
    string? BodyAsString { get; set; }
    byte[]? BodyAsBytes { get; set; }
    bool IsText { get; set; }
    string? Id { get; set; }
    string? CorrelationId { get; set; }
}
```

### File: `Models/IWebSocketResponse.cs`

```csharp
// Copyright © WireMock.Net

using WireMock.Transformers;
using WireMock.Types;

namespace WireMock.Models;

/// <summary>
/// Defines WebSocket response configuration
/// </summary>
public interface IWebSocketResponse
{
    List<IWebSocketMessage> Messages { get; set; }
    bool UseTransformer { get; set; }
    TransformerType TransformerType { get; set; }
    int? CloseCode { get; set; }
    string? CloseMessage { get; set; }
    string? Subprotocol { get; set; }
    int? AutoCloseDelayMs { get; set; }
}
```

### File: `BuilderExtensions/WebSocketResponseBuilder.cs`

```csharp
// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Models;
using WireMock.Transformers;
using WireMock.Types;

// ReSharper disable once CheckNamespace
namespace WireMock.ResponseBuilders;

/// <summary>
/// Interface for WebSocket response builder
/// </summary>
public interface IWebSocketResponseBuilder
{
    IWebSocketResponseBuilder WithMessage(string message, int? delayMs = null);
    IWebSocketResponseBuilder WithBinaryMessage(byte[] data, int? delayMs = null);
    IWebSocketResponseBuilder WithJsonMessage(object data, int? delayMs = null);
    IWebSocketResponseBuilder WithTransformer(bool use = true, TransformerType transformerType = TransformerType.Handlebars);
    IWebSocketResponseBuilder WithClose(int closeCode = 1000, string? reason = null);
    IWebSocketResponseBuilder WithSubprotocol(string subprotocol);
    IWebSocketResponseBuilder WithAutoClose(int delayMs);
    IWebSocketResponse Build();
}
```

---

## 7. Integration Points

### Update to `Response.cs` base class

```csharp
// In Response.cs, add to IResponseBuilder interface implementation:

public interface IResponseBuilder
{
    // ... existing members ...
    
    IResponseBuilder WithWebSocket(Action<IWebSocketResponseBuilder> configure);
    IResponseBuilder WithWebSocketMessage(string message, int? delayMs = null);
    IResponseBuilder WithWebSocketJsonMessage(object data, int? delayMs = null);
    IResponseBuilder WithWebSocketBinaryMessage(byte[] data, int? delayMs = null);
    IResponseBuilder WithWebSocketCallback(Func<IRequestMessage, Task<IEnumerable<IWebSocketMessage>>> handler);
    IResponseBuilder WithWebSocketCallback(Func<IRequestMessage, IEnumerable<IWebSocketMessage>> handler);
    IResponseBuilder WithWebSocketTransformer(bool use = true, TransformerType transformerType = TransformerType.Handlebars);
    IResponseBuilder WithWebSocketClose(int closeCode = 1000, string? reason = null);
    IResponseBuilder WithWebSocketSubprotocol(string subprotocol);
    IResponseBuilder WithWebSocketAutoClose(int delayMs);
}
```

### Update to `Request.cs` base class

```csharp
// In Request.cs, add to IRequestBuilder interface implementation:

public interface IRequestBuilder
{
    // ... existing members ...
    
    IRequestBuilder WithWebSocketUpgrade();
    IRequestBuilder WithWebSocketPath(string path);
    IRequestBuilder WithWebSocketSubprotocol(string subprotocol);
    IRequestBuilder WithWebSocketVersion(string version = "13");
    IRequestBuilder WithWebSocketOrigin(string origin);
}
```

---

## 8. Unit Test Templates

### File: `Tests/ResponseBuilders/WebSocketResponseBuilderTests.cs`

```csharp
using Xunit;
using WireMock.ResponseBuilders;

namespace WireMock.Net.Tests.ResponseBuilders;

public class WebSocketResponseBuilderTests
{
    [Fact]
    public void Build_WithSingleMessage_ReturnsValidResponse()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act
        var response = builder
            .WithMessage("Hello World")
            .Build();

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Messages);
        Assert.Equal("Hello World", response.Messages[0].BodyAsString);
    }

    [Fact]
    public void Build_WithMultipleMessages_MaintainsOrder()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act
        var response = builder
            .WithMessage("First", 0)
            .WithMessage("Second", 100)
            .WithMessage("Third", 200)
            .Build();

        // Assert
        Assert.Equal(3, response.Messages.Count);
        Assert.Equal("First", response.Messages[0].BodyAsString);
        Assert.Equal("Second", response.Messages[1].BodyAsString);
        Assert.Equal("Third", response.Messages[2].BodyAsString);
    }

    [Fact]
    public void Build_WithJsonMessage_SerializesObject()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();
        var testData = new { id = 1, name = "test" };

        // Act
        var response = builder
            .WithJsonMessage(testData)
            .Build();

        // Assert
        Assert.Single(response.Messages);
        Assert.Contains("\"id\"", response.Messages[0].BodyAsString);
    }

    [Fact]
    public void Build_WithTransformer_SetsTransformerFlag()
    {
        // Arrange & Act
        var response = new WebSocketResponseBuilder()
            .WithMessage("{{request.path}}")
            .WithTransformer()
            .Build();

        // Assert
        Assert.True(response.UseTransformer);
    }

    [Fact]
    public void Build_WithClose_SetsCloseCode()
    {
        // Arrange & Act
        var response = new WebSocketResponseBuilder()
            .WithMessage("Closing")
            .WithClose(1001, "Going away")
            .Build();

        // Assert
        Assert.Equal(1001, response.CloseCode);
        Assert.Equal("Going away", response.CloseMessage);
    }
}
```

---

## Quick Start Template

```csharp
// Basic echo server
server.Given(Request.Create().WithWebSocketPath("/echo"))
    .RespondWith(Response.Create()
        .WithWebSocket(ws => ws
            .WithMessage("Echo server ready")
        )
        .WithWebSocketCallback(async request =>
        {
            return new[]
            {
                new WebSocketMessage 
                { 
                    BodyAsString = $"Echo: {request.Body}" 
                }
            };
        })
    );

// Real-time notifications
server.Given(Request.Create()
    .WithWebSocketPath("/notifications")
    .WithWebSocketSubprotocol("notifications"))
    .RespondWith(Response.Create()
        .WithWebSocketSubprotocol("notifications")
        .WithWebSocket(ws => ws
            .WithJsonMessage(new { type = "connected" }, delayMs: 0)
            .WithJsonMessage(new { type = "notification", message = "New message" }, delayMs: 2000)
            .WithTransformer()
            .WithClose(1000, "Session ended")
        )
    );

// Data streaming
server.Given(Request.Create().WithWebSocketPath("/stream"))
    .RespondWith(Response.Create()
        .WithWebSocketCallback(async request =>
        {
            var messages = new List<WebSocketMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(new WebSocketMessage
                {
                    BodyAsString = $"{{\"index\":{i},\"timestamp\":\"{{now}}\"}}",
                    DelayMs = i * 1000,
                    IsText = true
                });
            }
            return messages;
        })
        .WithWebSocketTransformer()
    );
```

---

This implementation guide provides all the necessary templates to implement WebSocket support following WireMock.Net's established fluent interface patterns.
