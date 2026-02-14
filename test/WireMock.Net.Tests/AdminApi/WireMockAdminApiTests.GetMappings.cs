// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System.Threading.Tasks;
using RestEase;
using VerifyXunit;
using WireMock.Client;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests.AdminApi;

public partial class WireMockAdminApiTests
{
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

    [Fact]
    public async Task IWireMockAdminApi_GetMappingsAsync_WithBodyAsProtoBuf_ShouldReturnCorrectMappingModels()
    {
        // Arrange
        using var server = Given_WithBodyAsProtoBuf_AddedToServer();

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);
        var getMappingsResult = await api.GetMappingsAsync();

        await Verify(getMappingsResult, VerifySettings);
    }

    [Fact]
    public async Task HttpClient_GetMappingsAsync_WithBodyAsProtoBuf_ShouldReturnCorrectMappingModels()
    {
        // Arrange
        using var server = Given_WithBodyAsProtoBuf_AddedToServer();

        // Act
        var client = server.CreateClient();
        var getMappingsResult = await client.GetStringAsync("/__admin/mappings");

        await VerifyJson(getMappingsResult, VerifySettings);
    }

    public WireMockServer Given_WithBodyAsProtoBuf_AddedToServer()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();

        var protoBufJsonMatcher = new JsonPartialWildcardMatcher(new { name = "*" });

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloRequest", protoBufJsonMatcher)
            )
            .WithGuid(_guidUtilsMock.Object.NewGuid())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithBodyAsProtoBuf(ProtoDefinition, "greet.HelloReply",
                    new
                    {
                        message = "hello {{request.BodyAsJson.name}}"
                    }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc2/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf("greet.HelloRequest", protoBufJsonMatcher)
            )
            .WithProtoDefinition(ProtoDefinition)
            .WithGuid(_guidUtilsMock.Object.NewGuid())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithBodyAsProtoBuf("greet.HelloReply",
                    new
                    {
                        message = "hello {{request.BodyAsJson.name}}"
                    }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        server
            .AddProtoDefinition("my-greeter", ProtoDefinition)
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc3/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf("greet.HelloRequest", protoBufJsonMatcher)
            )
            .WithProtoDefinition("my-greeter")
            .WithGuid(_guidUtilsMock.Object.NewGuid())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithBodyAsProtoBuf("greet.HelloReply",
                    new
                    {
                        message = "hello {{request.BodyAsJson.name}}"
                    }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        server
            .AddProtoDefinition("my-greeter", ProtoDefinition)
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/grpc4/greet.Greeter/SayHello")
                .WithBodyAsProtoBuf("greet.HelloRequest")
            )
            .WithProtoDefinition("my-greeter")
            .WithGuid(_guidUtilsMock.Object.NewGuid())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/grpc")
                .WithBodyAsProtoBuf("greet.HelloReply",
                    new
                    {
                        message = "hello {{request.BodyAsJson.name}}"
                    }
                )
                .WithTrailingHeader("grpc-status", "0")
                .WithTransformer()
            );

        return server;
    }
}
#endif