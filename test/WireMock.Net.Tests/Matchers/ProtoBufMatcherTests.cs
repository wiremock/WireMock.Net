// Copyright © WireMock.Net

//#if PROTOBUF
using System;
using System.Threading.Tasks;
using FluentAssertions;
using ProtoBuf;
using WireMock.Matchers;
using WireMock.Models;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class ProtoBufMatcherTests
{
    private const string MessageType = "greet.HelloRequest";

    private static IdOrTexts ProtoDefinition => new(null, @"
syntax = ""proto3"";

package greet;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
" + "\r\n// Dummy " + Guid.NewGuid());

    [Fact]
    public async Task ProtoBufMatcher_For_ValidProtoBuf_And_ValidMethod_DecodeAsync()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(() => ProtoDefinition, MessageType);
        var result = await matcher.DecodeAsync(bytes).ConfigureAwait(false);

        // Assert
        result.Should().BeEquivalentTo(new { name = "stef" });
    }

    [Fact]
    public async Task ProtoBufMatcher_For_ValidProtoBuf_And_ValidMethod_NoJsonMatchers_IsMatchAsync()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(() => ProtoDefinition, MessageType);
        var result = await matcher.IsMatchAsync(bytes).ConfigureAwait(false);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public async Task ProtoBufMatcher_For_ValidProtoBuf_And_ValidMethod_Using_JsonMatcher_IsMatchAsync()
    {
        // Arrange
        var jsonMatcher = new JsonMatcher(new { name = "stef" });
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(() => ProtoDefinition, MessageType, matcher: jsonMatcher);
        var result = await matcher.IsMatchAsync(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public async Task ProtoBufMatcher_For_InvalidProtoBuf_IsNoMatch()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act
        var matcher = new ProtoBufMatcher(() => ProtoDefinition, MessageType);
        var result = await matcher.IsMatchAsync(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeOfType<ProtoException>();
    }

    [Fact]
    public async Task ProtoBufMatcher_For_InvalidMethod_IsNoMatchAsync()
    {
        // Arrange
        var bytes = Convert.FromBase64String("CgRzdGVm");

        // Act
        var matcher = new ProtoBufMatcher(() => ProtoDefinition, "greet.Greeter.X");
        var result = await matcher.IsMatchAsync(bytes);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeOfType<ArgumentException>();
    }
}
//#endif