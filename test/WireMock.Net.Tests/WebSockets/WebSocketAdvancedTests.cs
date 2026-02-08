// Copyright © WireMock.Net

#if !NET452

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.WebSockets;

/// <summary>
/// Advanced and edge case tests for WebSocket functionality
/// </summary>
public class WebSocketAdvancedTests
{
    private const string ClientIp = "::1";

    [Fact]
    public void WebSocketMessage_SwitchingBetweenTextAndBinary()
    {
        // Arrange
        var message = new WebSocketMessage();

        // Act - Set as text first
        message.BodyAsString = "Hello";
        Check.That(message.IsText).IsTrue();
        Check.That(message.BodyAsString).IsEqualTo("Hello");
        Check.That(message.BodyAsBytes).IsNull();

        // Act - Switch to binary
        message.BodyAsBytes = new byte[] { 1, 2, 3 };
        Check.That(message.IsText).IsFalse();
        Check.That(message.BodyAsString).IsNull();
        Check.That(message.BodyAsBytes).IsEqualTo(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public void WebSocketMessage_GeneratesUniqueIds()
    {
        // Arrange & Act
        var msg1 = new WebSocketMessage { BodyAsString = "msg1" };
        var msg2 = new WebSocketMessage { BodyAsString = "msg2" };

        // Assert
        Check.That(msg1.Id).IsNotEqualTo(msg2.Id);
        Check.That(Guid.Parse(msg1.Id)).IsNotEqualTo(Guid.Empty);
        Check.That(Guid.Parse(msg2.Id)).IsNotEqualTo(Guid.Empty);
    }

    [Fact]
    public void WebSocketResponse_EmptyMessages()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act
        var response = builder.Build();

        // Assert
        Check.That(response.Messages).IsEmpty();
        Check.That(response.UseTransformer).IsFalse();
        Check.That(response.CloseCode).IsNull();
    }

    [Fact]
    public void WebSocketResponse_LargeNumberOfMessages()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act
        for (int i = 0; i < 100; i++)
        {
            builder.WithMessage($"Message {i}", delayMs: i * 10);
        }
        var response = builder.Build();

        // Assert
        Check.That(response.Messages).HasSize(100);
        Check.That(response.Messages[0].DelayMs).IsEqualTo(0);
        Check.That(response.Messages[50].DelayMs).IsEqualTo(500);
        Check.That(response.Messages[99].DelayMs).IsEqualTo(990);
    }

    [Fact]
    public void WebSocketResponse_LargeMessage()
    {
        // Arrange
        var largeMessage = new string('x', 1024 * 1024); // 1MB message
        var builder = new WebSocketResponseBuilder();

        // Act
        builder.WithMessage(largeMessage);
        var response = builder.Build();

        // Assert
        Check.That(response.Messages).HasSize(1);
        Check.That(response.Messages[0].BodyAsString!.Length).IsEqualTo(1024 * 1024);
    }

    [Fact]
    public void WebSocketResponse_LargeBinaryMessage()
    {
        // Arrange
        var largeData = new byte[1024 * 1024]; // 1MB binary data
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        var builder = new WebSocketResponseBuilder();

        // Act
        builder.WithBinaryMessage(largeData);
        var response = builder.Build();

        // Assert
        Check.That(response.Messages).HasSize(1);
        Check.That(response.Messages[0].BodyAsBytes!.Length).IsEqualTo(1024 * 1024);
    }

    [Fact]
    public void WebSocketResponse_SpecialCharactersInMessages()
    {
        // Arrange
        var specialMessages = new[]
        {
            "Message with \"quotes\"",
            "Message with 'apostrophes'",
            "Message with\nnewlines",
            "Message with\ttabs",
            "Unicode: 你好世界 🌍",
            "Emoji: 😀 😃 😄"
        };

        var builder = new WebSocketResponseBuilder();

        // Act
        foreach (var msg in specialMessages)
        {
            builder.WithMessage(msg);
        }
        var response = builder.Build();

        // Assert
        Check.That(response.Messages).HasSize(specialMessages.Length);
        for (int i = 0; i < specialMessages.Length; i++)
        {
            Check.That(response.Messages[i].BodyAsString).IsEqualTo(specialMessages[i]);
        }
    }

    [Fact]
    public void WebSocketResponse_JsonWithComplexObjects()
    {
        // Arrange
        var complexObject = new
        {
            id = 123,
            name = "Test",
            nested = new
            {
                array = new[] { 1, 2, 3 },
                innerObj = new { prop = "value" }
            },
            timestamp = DateTime.UtcNow
        };

        var builder = new WebSocketResponseBuilder();

        // Act
        builder.WithJsonMessage(complexObject);
        var response = builder.Build();

        // Assert
        Check.That(response.Messages).HasSize(1);
        Check.That(response.Messages[0].BodyAsString).Contains("\"id\"");
        Check.That(response.Messages[0].BodyAsString).Contains("\"nested\"");
        Check.That(response.Messages[0].BodyAsString).Contains("\"array\"");
    }

    [Fact]
    public void WebSocketResponse_NullJsonObject()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act & Assert
        Check.ThatCode(() => builder.WithJsonMessage(null!)).Throws<ArgumentException>();
    }

    [Fact]
    public void WebSocketResponse_CloseWithoutMessage()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act
        builder.WithClose(1000); // No message
        var response = builder.Build();

        // Assert
        Check.That(response.CloseCode).IsEqualTo(1000);
        Check.That(response.CloseMessage).IsNull();
    }

    [Fact]
    public void WebSocketResponse_CloseWithMessage()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act
        builder.WithClose(1001, "Going away");
        var response = builder.Build();

        // Assert
        Check.That(response.CloseCode).IsEqualTo(1001);
        Check.That(response.CloseMessage).IsEqualTo("Going away");
    }

    [Fact]
    public void WebSocketResponse_VariousCloseCodes()
    {
        // Arrange
        var closeCodes = new[] { 1000, 1001, 1002, 1003, 1008, 1011, 4000 };

        // Act & Assert
        foreach (var code in closeCodes)
        {
            var builder = new WebSocketResponseBuilder();
            builder.WithClose(code);
            var response = builder.Build();
            Check.That(response.CloseCode).IsEqualTo(code);
        }
    }

    [Fact]
    public void WebSocketResponse_ReusableBuilder()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act - First build
        builder.WithMessage("Message 1");
        var response1 = builder.Build();

        // Add more messages and build again
        builder.WithMessage("Message 2");
        var response2 = builder.Build();

        // Assert - response2 includes all messages
        Check.That(response1.Messages).HasSize(1);
        Check.That(response2.Messages).HasSize(2); // Both from builder
    }

    [Fact]
    public void WebSocketResponse_DelayProgressions()
    {
        // Arrange - Test various delay patterns
        var builder = new WebSocketResponseBuilder();

        // Act - Linear progression
        builder.WithMessage("0ms", delayMs: 0);
        builder.WithMessage("100ms", delayMs: 100);
        builder.WithMessage("200ms", delayMs: 200);
        builder.WithMessage("300ms", delayMs: 300);

        var response = builder.Build();

        // Assert
        for (int i = 0; i < response.Messages.Count; i++)
        {
            Check.That(response.Messages[i].DelayMs).IsEqualTo(i * 100);
        }
    }

    [Fact]
    public void WebSocketResponse_TransformerToggle()
    {
        // Arrange
        var builder = new WebSocketResponseBuilder();

        // Act - Enable transformer
        builder.WithMessage("Template {{variable}}");
        builder.WithTransformer(TransformerType.Handlebars);
        var responseWithTransformer = builder.Build();

        // Assert
        Check.That(responseWithTransformer.UseTransformer).IsTrue();
        Check.That(responseWithTransformer.TransformerType).IsEqualTo(TransformerType.Handlebars);
    }

    [Fact]
    public void WebSocketResponse_Subprotocol_Variations()
    {
        // Arrange
        var subprotocols = new[] { "v1", "chat-v1", "my-protocol", "x-custom" };

        // Act & Assert
        foreach (var subprotocol in subprotocols)
        {
            var builder = new WebSocketResponseBuilder();
            builder.WithSubprotocol(subprotocol);
            var response = builder.Build();
            Check.That(response.Subprotocol).IsEqualTo(subprotocol);
        }
    }

    [Fact]
    public void WebSocketResponse_AutoClose_Variations()
    {
        // Arrange
        var delays = new[] { 0, 100, 500, 1000, 5000 };

        // Act & Assert
        foreach (var delay in delays)
        {
            var builder = new WebSocketResponseBuilder();
            builder.WithAutoClose(delayMs: delay);
            var response = builder.Build();
            Check.That(response.AutoCloseDelayMs).IsEqualTo(delay);
        }
    }
}

#endif
