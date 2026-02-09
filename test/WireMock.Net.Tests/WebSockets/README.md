# WebSocket Integration Tests - Summary

## Overview
I've successfully created comprehensive integration tests for the WebSockets implementation in WireMock.Net. These tests are based on Examples 1 and 2 from `WireMock.Net.WebSocketExamples` and use `ClientWebSocket` to perform real WebSocket connections.

## Test File Created
- **Location**: `test\WireMock.Net.Tests\WebSockets\WebSocketIntegrationTests.cs`
- **Test Count**: 13 integration tests
- **Test Framework**: xUnit with FluentAssertions

## Test Coverage

### Example 1: Echo Server Tests (5 tests)
1. **Example1_EchoServer_Should_Echo_Text_Messages**
   - Tests basic echo functionality with a single text message
   - Verifies message type and content

2. **Example1_EchoServer_Should_Echo_Multiple_Messages**
   - Tests echo functionality with multiple sequential messages
   - Ensures each message is echoed back correctly

3. **Example1_EchoServer_Should_Echo_Binary_Messages**
   - Tests echo functionality with binary data
   - Verifies binary message type and byte array content

4. **Example1_EchoServer_Should_Handle_Empty_Messages**
   - Tests edge case of empty messages
   - Ensures the server handles empty content gracefully

### Example 2: Custom Message Handler Tests (8 tests)
1. **Example2_CustomHandler_Should_Handle_Help_Command**
   - Tests `/help` command
   - Verifies the help text contains expected commands

2. **Example2_CustomHandler_Should_Handle_Time_Command**
   - Tests `/time` command
   - Verifies server time response format

3. **Example2_CustomHandler_Should_Handle_Echo_Command**
   - Tests `/echo <text>` command
   - Verifies text is echoed without the command prefix

4. **Example2_CustomHandler_Should_Handle_Upper_Command**
   - Tests `/upper <text>` command
   - Verifies text is converted to uppercase

5. **Example2_CustomHandler_Should_Handle_Reverse_Command**
   - Tests `/reverse <text>` command
   - Verifies text is reversed correctly

6. **Example2_CustomHandler_Should_Handle_Quit_Command**
   - Tests `/quit` command
   - Verifies goodbye message and proper WebSocket closure

7. **Example2_CustomHandler_Should_Handle_Unknown_Command**
   - Tests invalid commands
   - Verifies error message is sent to client

8. **Example2_CustomHandler_Should_Handle_Multiple_Commands_In_Sequence**
   - Integration test running multiple commands in sequence
   - Tests all commands together to verify state consistency

## Key Features

### Real WebSocket Testing
- Uses `ClientWebSocket` for authentic WebSocket connections
- Tests actual network communication, not mocked responses
- Verifies WebSocket protocol compliance

### Best Practices
- Each test is isolated with its own server instance
- Uses random ports (Port = 0) to avoid conflicts
- Proper cleanup with `IDisposable` pattern
- Uses FluentAssertions for readable test assertions

### Coverage
- Text and binary message types
- Multiple message sequences
- Command parsing and handling
- Error handling for invalid commands
- Proper connection closure

## Running the Tests

Run all WebSocket integration tests:
```bash
dotnet test --filter "FullyQualifiedName~WebSocketIntegrationTests"
```

Run only Example 1 tests:
```bash
dotnet test --filter "FullyQualifiedName~Example1"
```

Run only Example 2 tests:
```bash
dotnet test --filter "FullyQualifiedName~Example2"
```

## Dependencies
The tests rely on:
- `System.Net.WebSockets.ClientWebSocket`
- `WireMock.Server.WireMockServer`
- `FluentAssertions`
- `xUnit`

All dependencies are already included in the test project.

## Notes
- Tests use Port = 0 to automatically assign available ports
- Each test properly disposes of the server after completion
- Tests are independent and can run in parallel
- Binary message testing ensures support for non-text protocols
