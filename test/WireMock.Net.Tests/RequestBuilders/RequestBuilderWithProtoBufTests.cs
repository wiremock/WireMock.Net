// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithProtoBufTests
{
    private const string MessageType = "greet.HelloRequest";
    private const string TestProtoDefinition = @"
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
";

    [Fact]
    public void RequestBuilder_WithGrpcProto_Without_JsonMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithBodyAsProtoBuf(TestProtoDefinition, MessageType);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);

        var protoBufMatcher = (ProtoBufMatcher)((RequestMessageProtoBufMatcher)matchers[0]).Matcher!;
        protoBufMatcher.ProtoDefinition().Texts.Should().Contain(TestProtoDefinition);
        protoBufMatcher.MessageType.Should().Be(MessageType);
        protoBufMatcher.Matcher.Should().BeNull();
    }

    [Fact]
    public void RequestBuilder_WithGrpcProto_With_JsonMatcher()
    {
        // Act
        var jsonMatcher = new JsonMatcher(new { name = "stef" });
        var requestBuilder = (Request)Request.Create().WithBodyAsProtoBuf(TestProtoDefinition, MessageType, jsonMatcher);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);

        var protoBufMatcher = (ProtoBufMatcher)((RequestMessageProtoBufMatcher)matchers[0]).Matcher!;
        protoBufMatcher.ProtoDefinition().Texts.Should().Contain(TestProtoDefinition);
        protoBufMatcher.MessageType.Should().Be(MessageType);
        protoBufMatcher.Matcher.Should().BeOfType<JsonMatcher>();
    }
}