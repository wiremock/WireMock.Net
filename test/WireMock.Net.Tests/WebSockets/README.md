# WebSocket Integration Tests - Summary

## Overview
Comprehensive integration tests for the WebSockets implementation in WireMock.Net. These tests are based on Examples 1, 2, and 3 from `WireMock.Net.WebSocketExamples` and use `ClientWebSocket` to perform real WebSocket connections.

## Test File
- **Location**: `test\WireMock.Net.Tests\WebSockets\WebSocketIntegrationTests.cs`
- **Test Count**: 21 integration tests
- **Test Framework**: xUnit with FluentAssertions

## Test Coverage Summary

| Category | Tests | Description |
|----------|-------|-------------|
| **Example 1: Echo Server** | 4 | Basic echo functionality with text/binary messages |
| **Example 2: Custom Handlers** | 8 | Command processing and custom message handlers |
| **Example 3: JSON (SendJsonAsync)** | 3 | JSON serialization and complex object handling |
| **Broadcast** | 6 | Multi-client broadcasting functionality |
| **Total** | **21** | |

## Detailed Test Descriptions

### Example 1: Echo Server Tests (4 tests)
Tests the basic WebSocket echo functionality where messages are echoed back to the sender.

1. **Example1_EchoServer_Should_Echo_Text_Messages**
   - ✅ Single text message echo
   - ✅ Verifies message type and content

2. **Example1_EchoServer_Should_Echo_Multiple_Messages**
   - ✅ Multiple sequential messages
   - ✅ Each message echoed correctly

3. **Example1_EchoServer_Should_Echo_Binary_Messages**
   - ✅ Binary data echo
   - ✅ Byte array verification

4. **Example1_EchoServer_Should_Handle_Empty_Messages**
   - ✅ Edge case: empty messages
   - ✅ Graceful handling

### Example 2: Custom Message Handler Tests (8 tests)
Tests custom message processing with various commands.

1. **Example2_CustomHandler_Should_Handle_Help_Command**
   - ✅ `/help` → Returns list of available commands

2. **Example2_CustomHandler_Should_Handle_Time_Command**
   - ✅ `/time` → Returns current server time

3. **Example2_CustomHandler_Should_Handle_Echo_Command**
   - ✅ `/echo <text>` → Echoes the text

4. **Example2_CustomHandler_Should_Handle_Upper_Command**
   - ✅ `/upper <text>` → Converts to uppercase

5. **Example2_CustomHandler_Should_Handle_Reverse_Command**
   - ✅ `/reverse <text>` → Reverses the text

6. **Example2_CustomHandler_Should_Handle_Quit_Command**
   - ✅ `/quit` → Sends goodbye and closes connection

7. **Example2_CustomHandler_Should_Handle_Unknown_Command**
   - ✅ Invalid commands → Error message

8. **Example2_CustomHandler_Should_Handle_Multiple_Commands_In_Sequence**
   - ✅ All commands in sequence
   - ✅ State consistency verification

### Example 3: SendJsonAsync Tests (3 tests)
Tests JSON serialization and the `SendJsonAsync` functionality.

1. **Example3_JsonEndpoint_Should_Send_Json_Response**
   - ✅ Basic JSON response
   - ✅ Structure: `{ timestamp, message, length, type }`
   - ✅ Proper serialization

2. **Example3_JsonEndpoint_Should_Handle_Multiple_Json_Messages**
   - ✅ Sequential JSON messages
   - ✅ Each properly serialized

3. **Example3_JsonEndpoint_Should_Serialize_Complex_Objects**
   - ✅ Nested objects
   - ✅ Arrays within objects
   - ✅ Complex structures

### Broadcast Tests (6 tests)
Tests the broadcast functionality with multiple simultaneous clients.

1. **Broadcast_Should_Send_Message_To_All_Connected_Clients**
   - ✅ 3 connected clients
   - ✅ All receive same broadcast
   - ✅ Timestamp in messages

2. **Broadcast_Should_Only_Send_To_Open_Connections**
   - ✅ Closed connections skipped
   - ✅ Only active clients receive

3. **BroadcastJson_Should_Send_Json_To_All_Clients**
   - ✅ JSON broadcasting
   - ✅ Multiple clients receive
   - ✅ Sender identification

4. **Broadcast_Should_Handle_Multiple_Sequential_Messages**
   - ✅ Sequential broadcasts
   - ✅ Message ordering
   - ✅ All clients receive all messages

5. **Broadcast_Should_Work_With_Many_Clients**
   - ✅ 5 simultaneous clients
   - ✅ Scalability test
   - ✅ Parallel message reception

6. **Broadcast Integration**
   - ✅ Complete flow testing

## Key Testing Features

### 🔌 Real WebSocket Connections
- Uses `System.Net.WebSockets.ClientWebSocket`
- Actual network communication
- Protocol compliance verification

### 📤 SendJsonAsync Coverage
```csharp
await ctx.SendJsonAsync(new {
    timestamp = DateTime.UtcNow,
    message = msg.Text,
    data = complexObject
});
```
- Simple objects
- Complex nested structures
- Arrays and collections

### 📡 Broadcast Coverage
```csharp
await ctx.BroadcastTextAsync("Message to all");
await ctx.BroadcastJsonAsync(jsonObject);
```
- Multiple simultaneous clients
- Text and JSON broadcasts
- Connection state handling
- Scalability testing

### ✨ Best Practices
- ✅ Test isolation (each test has own server)
- ✅ Random ports (Port = 0)
- ✅ Proper cleanup (`IDisposable`)
- ✅ FluentAssertions for readability
- ✅ Async/await throughout
- ✅ No test interdependencies

## Running the Tests

### All WebSocket Tests
```bash
dotnet test --filter "FullyQualifiedName~WebSocketIntegrationTests"
```

### By Example
```bash
# Example 1: Echo
dotnet test --filter "FullyQualifiedName~Example1"

# Example 2: Custom Handlers
dotnet test --filter "FullyQualifiedName~Example2"

# Example 3: JSON
dotnet test --filter "FullyQualifiedName~Example3"
```

### By Feature
```bash
# Broadcast tests
dotnet test --filter "FullyQualifiedName~Broadcast"

# JSON tests
dotnet test --filter "FullyQualifiedName~Json"
```

### Run Specific Test
```bash
dotnet test --filter "FullyQualifiedName~Example1_EchoServer_Should_Echo_Text_Messages"
```

## Dependencies

| Package | Purpose |
|---------|---------|
| `System.Net.WebSockets.ClientWebSocket` | Real WebSocket client |
| `WireMock.Server` | WireMock server instance |
| `FluentAssertions` | Readable assertions |
| `xUnit` | Test framework |
| `Newtonsoft.Json` | JSON parsing in assertions |

All dependencies are included in `WireMock.Net.Tests.csproj`.

## Implementation Details

### JSON Testing Pattern
```csharp
// Send text message
await client.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

// Receive JSON response
var result = await client.ReceiveAsync(buffer, CancellationToken.None);
var json = JObject.Parse(received);

// Assert structure
json["message"].ToString().Should().Be(expectedMessage);
json["timestamp"].Should().NotBeNull();
```

### Broadcast Testing Pattern
```csharp
// Connect multiple clients
var clients = new[] { new ClientWebSocket(), new ClientWebSocket() };
foreach (var c in clients)
    await c.ConnectAsync(uri, CancellationToken.None);

// Send from one client
await clients[0].SendAsync(message, ...);

// All clients receive
foreach (var c in clients) {
    var result = await c.ReceiveAsync(buffer, ...);
    // Assert all received the same message
}
```

## Test Timing Notes
- Connection registration delays: 100-200ms
- Ensures all clients are registered before broadcasting
- Prevents race conditions in multi-client tests
- Production code does not require delays

## Coverage Metrics
- ✅ Text messages
- ✅ Binary messages
- ✅ Empty messages
- ✅ JSON serialization (simple & complex)
- ✅ Multiple sequential messages
- ✅ Multiple simultaneous clients
- ✅ Connection state transitions
- ✅ Broadcast to all clients
- ✅ Closed connection handling
- ✅ Error scenarios
