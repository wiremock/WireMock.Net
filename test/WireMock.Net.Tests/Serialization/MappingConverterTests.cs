// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public partial class MappingConverterTests
{
    private readonly Guid _guid = new("c8eeaf99-d5c4-4341-8543-4597c3fd40d9");
    private readonly DateTime _updatedAt = new(2022, 12, 4, 11, 12, 13);
    private readonly WireMockServerSettings _settings = new();
    private const string ProtoDefinition = @"
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

    private readonly MappingConverter _sut;

    public MappingConverterTests()
    {
        _sut = new MappingConverter(new MatcherMapper(_settings));
    }

    [Fact]
    public Task ToMappingModel_With_SingleWebHook()
    {
        // Assign
        var request = Request.Create();
        var response = Response.Create();
        var webhooks = new IWebhook[]
        {
            new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = "https://test.com",
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        { "Single", new WireMockList<string>("x") },
                        { "Multi", new WireMockList<string>("a", "b") }
                    },
                    Method = "post",
                    BodyData = new BodyData
                    {
                        BodyAsString = "b",
                        DetectedBodyType = BodyType.String,
                        DetectedBodyTypeFromContentType = BodyType.String
                    }
                }
            }
        };
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 0, null, null, null, null, webhooks, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Priority.Should().BeNull();

        model.Response.BodyAsJsonIndented.Should().BeNull();
        model.Response.UseTransformer.Should().BeNull();
        model.Response.Headers.Should().BeNull();
        model.UseWebhooksFireAndForget.Should().BeFalse();

        model.Webhooks.Should().BeNull();

        model.Webhook!.Request.Method.Should().Be("post");
        model.Webhook.Request.Url.Should().Be("https://test.com");
        model.Webhook.Request.Headers.Should().HaveCount(2);
        model.Webhook.Request.Body.Should().Be("b");
        model.Webhook.Request.BodyAsJson.Should().BeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_With_MultipleWebHooks()
    {
        // Assign
        var request = Request.Create();
        var response = Response.Create();
        var webhooks = new IWebhook[]
        {
            new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = "https://test1.com",
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        { "One", new WireMockList<string>("x") }
                    },
                    Method = "post",
                    BodyData = new BodyData
                    {
                        BodyAsString = "1",
                        DetectedBodyType = BodyType.String,
                        DetectedBodyTypeFromContentType = BodyType.String
                    }
                }
            },

            new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = "https://test2.com",
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        { "First", new WireMockList<string>("x") },
                        { "Second", new WireMockList<string>("a", "b") }
                    },
                    Method = "post",
                    BodyData = new BodyData
                    {
                        BodyAsString = "2",
                        DetectedBodyType = BodyType.String,
                        DetectedBodyTypeFromContentType = BodyType.String
                    }
                }
            }
        };
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 0, null, null, null, null, webhooks, true, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Priority.Should().BeNull();

        model.Response.BodyAsJsonIndented.Should().BeNull();
        model.Response.UseTransformer.Should().BeNull();
        model.Response.Headers.Should().BeNull();
        model.UseWebhooksFireAndForget.Should().BeTrue();

        model.Webhook.Should().BeNull();

        model.Webhooks![0].Request.Method.Should().Be("post");
        model.Webhooks[0].Request.Url.Should().Be("https://test1.com");
        model.Webhooks[0].Request.Headers.Should().HaveCount(1);
        model.Webhooks[0].Request.Body.Should().Be("1");

        model.Webhooks[1].Request.Method.Should().Be("post");
        model.Webhooks[1].Request.Url.Should().Be("https://test2.com");
        model.Webhooks[1].Request.Headers.Should().HaveCount(2);
        model.Webhooks[1].Request.Body.Should().Be("2");

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_WithTitle_And_Description_ReturnsCorrectModel()
    {
        // Assign
        var title = "my-title";
        var description = "my-description";
        var request = Request.Create();
        var response = Response.Create();
        var mapping = new Mapping(_guid, _updatedAt, title, description, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Title.Should().Be(title);
        model.Description.Should().Be(description);

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_WithPriority_ReturnsPriority()
    {
        // Assign
        var request = Request.Create();
        var response = Response.Create().WithBodyAsJson(new { x = "x" }).WithTransformer();
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Priority.Should().Be(42);
        model.Response.UseTransformer.Should().BeTrue();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_WithTimeSettings_ReturnsCorrectTimeSettings()
    {
        // Assign
        var start = new DateTime(2023, 1, 14, 15, 16, 17, DateTimeKind.Utc);
        var ttl = 100;
        var end = start.AddSeconds(ttl);
        var request = Request.Create();
        var response = Response.Create();
        var timeSettings = new TimeSettings
        {
            Start = start,
            End = end,
            TTL = ttl
        };
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, timeSettings, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.TimeSettings!.Should().NotBeNull();
        model.TimeSettings!.Start.Should().Be(start);
        model.TimeSettings.End.Should().Be(end);
        model.TimeSettings.TTL.Should().Be(ttl);

        // Verify
        return Verifier.Verify(model, VerifySettings);
    }

    [Fact]
    public void ToMappingModel_WithDelayAsTimeSpan_ReturnsCorrectModel()
    {
        // Arrange
        var tests = new[]
        {
            new { Delay = Timeout.InfiniteTimeSpan, Expected = (int) TimeSpan.MaxValue.TotalMilliseconds },
            new { Delay = TimeSpan.FromSeconds(1), Expected = 1000 },
            new { Delay = TimeSpan.MaxValue, Expected = (int) TimeSpan.MaxValue.TotalMilliseconds }
        };

        foreach (var test in tests)
        {
            var request = Request.Create();
            var response = Response.Create().WithDelay(test.Delay);
            var mapping = new Mapping(Guid.NewGuid(), _updatedAt, string.Empty, string.Empty, string.Empty, _settings, request, response, 42, null, null, null, null, null, false, null, data: null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Response.Delay.Should().Be(test.Expected);
        }
    }

    [Fact]
    public Task ToMappingModel_WithDelay_ReturnsCorrectModel()
    {
        // Assign
        var delay = 1000;
        var request = Request.Create();
        var response = Response.Create().WithDelay(delay);
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Response.Delay.Should().Be(delay);

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_WithRandomMinimumDelay_ReturnsCorrectModel()
    {
        // Assign
        int minimumDelay = 1000;
        var request = Request.Create();
        var response = Response.Create().WithRandomDelay(minimumDelay);
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Response.Delay.Should().BeNull();
        model.Response.MinimumRandomDelay.Should().Be(minimumDelay);
        model.Response.MaximumRandomDelay.Should().Be(60_000);

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_WithRandomDelay_ReturnsCorrectModel()
    {
        // Assign
        int minimumDelay = 1000;
        int maximumDelay = 2000;
        var request = Request.Create();
        var response = Response.Create().WithRandomDelay(minimumDelay, maximumDelay);
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Response.Delay.Should().BeNull();
        model.Response.MinimumRandomDelay.Should().Be(minimumDelay);
        model.Response.MaximumRandomDelay.Should().Be(maximumDelay);

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_WithProbability_ReturnsCorrectModel()
    {
        // Assign
        double probability = 0.4;
        var request = Request.Create();
        var response = Response.Create();
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, data: null)
            .WithProbability(probability);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();
        model.Probability.Should().Be(0.4);

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Request_WithClientIP_ReturnsCorrectModel()
    {
        // Arrange
        var request = Request.Create().WithClientIP("1.2.3.4");
        var response = Response.Create();
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Request_WithHeader_And_Cookie_ReturnsCorrectModel()
    {
        // Assign
        var request = Request.Create()
            .WithHeader("MatchBehaviour.RejectOnMatch", "hv-1", MatchBehaviour.RejectOnMatch)
            .WithHeader("MatchBehaviour.AcceptOnMatch", "hv-2", MatchBehaviour.AcceptOnMatch)
            .WithHeader("IgnoreCase_false", "hv-3", false)
            .WithHeader("IgnoreCase_true", "hv-4")
            .WithHeader("ExactMatcher", new ExactMatcher("h-exact"))

            .WithCookie("MatchBehaviour.RejectOnMatch", "cv-1", MatchBehaviour.RejectOnMatch)
            .WithCookie("MatchBehaviour.AcceptOnMatch", "cv-2", MatchBehaviour.AcceptOnMatch)
            .WithCookie("IgnoreCase_false", "cv-3", false)
            .WithCookie("IgnoreCase_true", "cv-4")
            .WithCookie("ExactMatcher", new ExactMatcher("c-exact"))
            ;

        var response = Response.Create();

        var mapping = new Mapping(_guid, _updatedAt, null, null, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Response_WithHeader_ReturnsCorrectModel()
    {
        // Assign
        var request = Request.Create();

        var response = Response.Create()
            .WithHeader("x1", "y")
            .WithHeader("x2", "y", "z")
            .WithHeaders(new Dictionary<string, string> { { "d", "test" } })
            .WithHeaders(new Dictionary<string, string[]> { { "d[]", new[] { "v1", "v2" } } })
            .WithHeaders(new Dictionary<string, WireMockList<string>> { { "w", new WireMockList<string>("x") } })
            .WithHeaders(new Dictionary<string, WireMockList<string>> { { "w[]", new WireMockList<string>("x", "y") } });

        var mapping = new Mapping(_guid, _updatedAt, null, null, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Response_WithHeaders_ReturnsCorrectModel()
    {
        // Assign
        var request = Request.Create();

        var response = Response.Create()
            .WithHeaders(new Dictionary<string, WireMockList<string>> { { "w[]", new WireMockList<string>("x", "y") } });

        var mapping = new Mapping(_guid, _updatedAt, null, null, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

#if TRAILINGHEADERS
    [Fact]
    public Task ToMappingModel_Response_WithTrailingHeader_ReturnsCorrectModel()
    {
        // Assign
        var request = Request.Create();

        var response = Response.Create()
            .WithTrailingHeader("x1", "y")
            .WithTrailingHeader("x2", "y", "z");

        var mapping = new Mapping(_guid, _updatedAt, null, null, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Response_WithTrailingHeaders_ReturnsCorrectModel()
    {
        // Assign
        var request = Request.Create();

        var response = Response.Create()
            .WithTrailingHeaders(new Dictionary<string, WireMockList<string>> { { "w[]", new WireMockList<string>("x", "y") } });

        var mapping = new Mapping(_guid, _updatedAt, null, null, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }
#endif

    [Fact]
    public Task ToMappingModel_WithParam_ReturnsCorrectModel()
    {
        // Assign
        var request = Request.Create()
                .WithParam("MatchBehaviour.RejectOnMatch", MatchBehaviour.RejectOnMatch)
                .WithParam("MatchBehaviour.RejectOnMatch|IgnoreCase_false", false, MatchBehaviour.RejectOnMatch)
                .WithParam("IgnoreCase_false", false, "pv-3a", "pv-3b")
                .WithParam("IgnoreCase_true", true, "pv-3a", "pv-3b")
                .WithParam("ExactMatcher", new ExactMatcher("exact"))
            ;
        var response = Response.Create();
        var mapping = new Mapping(_guid, _updatedAt, null, null, null, _settings, request, response, 0, null, null, null, null, null, false, null, data: null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

#if GRAPHQL
    [Fact]
    public Task ToMappingModel_Request_WithBodyAsGraphQLSchema_ReturnsCorrectModel()
    {
        // Arrange
        const string schema = @"
  type Query {
   greeting:String
   students:[Student]
   studentById(id:ID!):Student
  }

  type Student {
   id:ID!
   firstName:String
   lastName:String
   fullName:String 
  }";
        var request = Request.Create().WithGraphQLSchema(schema);
        var response = Response.Create();
        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, false, null, null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }
#endif

#if PROTOBUF
    [Fact]
    public Task ToMappingModel_Request_WithBodyAsProtoBuf_ReturnsCorrectModel()
    {
        // Arrange
        var jsonMatcher = new JsonMatcher(new { name = "stef" });

        var request = Request.Create()
            .UsingPost()
            .WithPath("/grpc/greet.Greeter/SayHello")
            .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloRequest", jsonMatcher);

        var response = Response.Create();

        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 41, null, null, null, null, null, false, null, null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Response_WithBodyAsProtoBuf_ReturnsCorrectModel()
    {
        // Arrange
        var protobufResponse = new
        {
            message = "hello"
        };

        var request = Request.Create();

        var response = Response.Create()
            .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloReply", protobufResponse)
            .WithTrailingHeader("grpc-status", "0");

        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 43, null, null, null, null, null, false, null, null);

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }

    [Fact]
    public Task ToMappingModel_Mapping_WithBodyAsProtoBuf_ReturnsCorrectModel()
    {
        // Arrange
        var jsonMatcher = new JsonMatcher(new { name = "stef" });
        var protobufResponse = new
        {
            message = "hello"
        };

        var request = Request.Create()
            .UsingPost()
            .WithPath("/grpc/greet.Greeter/SayHello")
            .WithBodyAsProtoBuf("greet.HelloRequest", jsonMatcher);

        var response = Response.Create()
            .WithBodyAsProtoBuf("greet.HelloReply", protobufResponse)
            .WithTrailingHeader("grpc-status", "0");

        var mapping = new Mapping(_guid, _updatedAt, string.Empty, string.Empty, null, _settings, request, response, 41, null, null, null, null, null, false, null, null)
            .WithProtoDefinition(new (null, ProtoDefinition));

        ((Request)request).Mapping = mapping;
        ((Response)response).Mapping = mapping;

        // Act
        var model = _sut.ToMappingModel(mapping);

        // Assert
        model.Should().NotBeNull();

        // Verify
        return Verifier.Verify(model);
    }
#endif
}
#endif