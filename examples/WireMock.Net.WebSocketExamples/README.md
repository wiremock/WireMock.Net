# WireMock.Net WebSocket Examples

This project demonstrates all the WebSocket capabilities of WireMock.Net.

## Prerequisites

- .NET 8.0 SDK
- Optional: `wscat` for manual testing (`npm install -g wscat`)

## Running the Examples

```bash
cd examples/WireMock.Net.WebSocketExamples
dotnet run
```

## Available Examples

### 1. Echo Server
Simple WebSocket echo server that returns all messages back to the client.

**Test with:**
```bash
wscat -c ws://localhost:9091/ws/echo
```

### 2. Custom Message Handler
Chat server with commands: `/help`, `/time`, `/echo`, `/upper`, `/reverse`, `/quit`

**Test with:**
```bash
wscat -c ws://localhost:9091/ws/chat
> /help
> /time
> /echo Hello World
> /upper test
> /reverse hello
```

### 3. Broadcast Server
Messages sent by any client are broadcast to all connected clients.

**Test with multiple terminals:**
```bash
# Terminal 1
wscat -c ws://localhost:9091/ws/broadcast

# Terminal 2
wscat -c ws://localhost:9091/ws/broadcast

# Terminal 3
wscat -c ws://localhost:9091/ws/broadcast
```

Type messages in any terminal and see them appear in all terminals.

### 4. Scenario/State Machine
Game server with state transitions: Lobby -> Playing -> GameOver

**Test with:**
```bash
wscat -c ws://localhost:9091/ws/game
> ready
> attack
> defend
> quit
```

### 5. WebSocket Proxy
Proxies WebSocket connections to echo.websocket.org

**Test with:**
```bash
wscat -c ws://localhost:9091/ws/proxy
```

### 6. Multiple Endpoints
Runs multiple WebSocket endpoints simultaneously:
- `/ws/echo` - Echo server
- `/ws/time` - Returns server time
- `/ws/json` - Returns JSON responses
- `/ws/protocol` - Protocol-specific endpoint

### 7. All Examples
Runs all endpoints at once with connection statistics.

## Features Demonstrated

- ✅ **Echo Server** - Simple message echo
- ✅ **Custom Handlers** - Complex message processing
- ✅ **Broadcast** - Multi-client communication
- ✅ **Scenarios** - State machine patterns
- ✅ **Proxy** - Forwarding to real WebSocket servers
- ✅ **Protocol Negotiation** - Sec-WebSocket-Protocol support
- ✅ **JSON Messaging** - Structured data exchange
- ✅ **Connection Management** - Track and manage connections
- ✅ **Configuration** - Custom WebSocket settings

## Testing with wscat

Install wscat globally:
```bash
npm install -g wscat
```

Basic usage:
```bash
# Connect to endpoint
wscat -c ws://localhost:9091/ws/echo

# Connect with protocol
wscat -c ws://localhost:9091/ws/protocol -s chat

# Connect with headers
wscat -c ws://localhost:9091/ws/echo -H "X-Custom-Header: value"
```

## Testing with C# Client

The examples include built-in C# WebSocket clients for automated testing.
Select options 1 or 2 and press any key to run the automated tests.

## Configuration

WebSocket settings can be configured:

```csharp
var server = WireMockServer.Start(new WireMockServerSettings
{
    Port = 9091,
    WebSocketSettings = new WebSocketSettings
    {
        MaxConnections = 100,
        ReceiveBufferSize = 8192,
        MaxMessageSize = 1048576,
        KeepAliveInterval = TimeSpan.FromSeconds(30),
        CloseTimeout = TimeSpan.FromMinutes(10),
        EnableCompression = true
    }
});
```

## Monitoring

When running "All Examples" (option 7), press any key to view:
- Active connection count
- Connection IDs
- Request paths
- WebSocket states

## Notes

- All examples run on port 9091 by default
- Press CTRL+C to stop the server
- Multiple clients can connect simultaneously
- Connection states are tracked and can be queried
