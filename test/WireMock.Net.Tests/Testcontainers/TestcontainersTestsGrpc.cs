// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using Greet;
using Grpc.Net.Client;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;
using WireMock.Constants;
using WireMock.Net.Testcontainers;
using WireMock.Util;

namespace WireMock.Net.Tests.Testcontainers;

[Collection("Grpc")]
public class TestcontainersTestsGrpc(ITestOutputHelper testOutputHelper)
{
    private readonly ILogger _logger = new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), nameof(TestcontainersTestsGrpc), new XUnitLoggerOptions
    {
        IncludeCategory = true,
        TimestampFormat = "yyy-MM-dd HH:mm:ss.fff"
    });

    [Fact]
    public async Task WireMockContainer_Build_Grpc_TestPortsAndUrls1()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var port = PortUtils.FindFreeTcpPort();

        // Act
        var wireMockContainer = new WireMockContainerBuilder()
            .WithLogger(_logger)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword)
            .WithCommand("--UseHttp2")
            .WithCommand("--Urls", $"http://*:80 grpc://*:{port}")
            .WithPortBinding(port, true)
            .Build();

        try
        {
            await wireMockContainer.StartAsync();

            // Assert
            using (new AssertionScope())
            {
                var logs = await wireMockContainer.GetLogsAsync(DateTime.MinValue);
                logs.Should().NotBeNull();

                var url = wireMockContainer.GetPublicUrl();
                url.Should().NotBeNullOrWhiteSpace();

                var urls = wireMockContainer.GetPublicUrls();
                urls.Should().HaveCount(2);

                var httpPort = wireMockContainer.GetMappedPublicPort(80);
                httpPort.Should().BeGreaterThan(0);

                var httpUrl = wireMockContainer.GetMappedPublicUrl(80);
                httpUrl.Should().StartWith("http://");

                var grpcPort = wireMockContainer.GetMappedPublicPort(port);
                grpcPort.Should().BeGreaterThan(0);

                var grpcUrl = wireMockContainer.GetMappedPublicUrl(port);
                grpcUrl.Should().StartWith("http://");

                var adminClient = wireMockContainer.CreateWireMockAdminClient();

                var settings = await adminClient.GetSettingsAsync();
                settings.Should().NotBeNull();
            }
        }
        finally
        {
            await StopAsync(wireMockContainer);
        }
    }

    [Fact]
    public async Task WireMockContainer_Build_Grpc_TestPortsAndUrls2()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var ports = PortUtils.FindFreeTcpPorts(3);

        // Act
        var wireMockContainer = new WireMockContainerBuilder()
            .WithLogger(_logger)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword)
            .AddUrl($"http://*:{ports[0]}")
            .AddUrl($"grpc://*:{ports[1]}")
            .AddUrl($"grpc://*:{ports[2]}")
            .Build();

        try
        {
            await wireMockContainer.StartAsync();

            // Assert
            using (new AssertionScope())
            {
                var logs = await wireMockContainer.GetLogsAsync(DateTime.MinValue);
                logs.Should().NotBeNull();

                var url = wireMockContainer.GetPublicUrl();
                url.Should().NotBeNullOrWhiteSpace();

                var urls = wireMockContainer.GetPublicUrls();
                urls.Should().HaveCount(4);

                foreach (var internalPort in new[] { ports[0], ports[1], ports[2], 80 })
                {
                    var publicPort = wireMockContainer.GetMappedPublicPort(internalPort);
                    publicPort.Should().BeGreaterThan(0);

                    var publicUrl = wireMockContainer.GetMappedPublicUrl(internalPort);
                    publicUrl.Should().StartWith("http://");
                }

                var adminClient = wireMockContainer.CreateWireMockAdminClient();

                var settings = await adminClient.GetSettingsAsync();
                settings.Should().NotBeNull();
            }
        }
        finally
        {
            await StopAsync(wireMockContainer);
        }
    }

    [Fact]
    public async Task WireMockContainer_Build_Grpc_ProtoDefinitionFromJson_UsingGrpcGeneratedClient()
    {
        var wireMockContainer = await Given_WireMockContainerIsStartedForHttpAndGrpcAsync();

        await Given_ProtoBufMappingIsAddedViaAdminInterfaceAsync(wireMockContainer, "protobuf-mapping-1.json");

        var reply = await When_GrpcClient_Calls_SayHelloAsync(wireMockContainer);

        Then_ReplyMessage_Should_BeCorrect(reply);

        await StopAsync(wireMockContainer);
    }

    [Fact]
    public async Task WireMockContainer_Build_Grpc_ProtoDefinitionAtServerLevel_UsingGrpcGeneratedClient()
    {
        var wireMockContainer = await Given_WireMockContainerWithProtoDefinitionAtServerLevelIsStartedForHttpAndGrpcAsync();

        await Given_ProtoBufMappingIsAddedViaAdminInterfaceAsync(wireMockContainer, "protobuf-mapping-4.json");

        var reply = await When_GrpcClient_Calls_SayHelloAsync(wireMockContainer);

        Then_ReplyMessage_Should_BeCorrect(reply);

        await StopAsync(wireMockContainer);
    }

    [Fact]
    public async Task WireMockContainer_Build_Grpc_ProtoDefinitionAtServerLevel_UsingGrpcGeneratedClient_AndWithWatchStaticMappings()
    {
        var wireMockContainer = await Given_WireMockContainerWithProtoDefinitionAtServerLevelWithWatchStaticMappingsIsStartedForHttpAndGrpcAsync();

        var reply = await When_GrpcClient_Calls_SayHelloAsync(wireMockContainer);

        Then_ReplyMessage_Should_BeCorrect(reply);

        await StopAsync(wireMockContainer);
    }

    private async Task<HelloReply> When_GrpcClient_Calls_SayHelloAsync(WireMockContainer wireMockContainer)
    {
        var address = wireMockContainer.GetPublicUrls().First(x => x.Key != 80).Value;
        var channel = GrpcChannel.ForAddress(address);

        var client = new Greeter.GreeterClient(channel);

        try
        {
            return await client.SayHelloAsync(new HelloRequest { Name = "stef" });
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine("Exception during GrpcClient Call to {0}. Exception = {1}.", address, ex);

            testOutputHelper.WriteLine("Dumping WireMock.Net logs:");
            var (stdOut, stdError) = await wireMockContainer.GetLogsAsync(DateTime.MinValue);
            testOutputHelper.WriteLine("Out  :\r\n{0}", stdOut);
            testOutputHelper.WriteLine("Error:\r\n{0}", stdError);

            testOutputHelper.WriteLine("Dumping WireMock.Net mappings:");
            using var httpClient = wireMockContainer.CreateClient();
            using var response = await httpClient.GetAsync("/__admin/mappings");
            var mappings = await response.Content.ReadAsStringAsync();
            testOutputHelper.WriteLine("Mappings:\r\n{0}", mappings);
            throw;
        }
    }

    private async Task StopAsync(WireMockContainer wireMockContainer)
    {
        try
        {
            await wireMockContainer.StopAsync();
        }
        catch (Exception ex)
        {
            // Sometimes we get this exception, so for now ignore it.
            /*
            Failed WireMock.Net.Tests.Testcontainers.TestcontainersTests.WireMockContainer_Build_WithImageAsText_And_StartAsync_and_StopAsync [9 s]
               Error Message:
                System.NullReferenceException : Object reference not set to an instance of an object.
               Stack Trace:
                  at DotNet.Testcontainers.Containers.DockerContainer.UnsafeStopAsync(CancellationToken ct) in /_/src/Testcontainers/Containers/DockerContainer.cs:line 567
                at DotNet.Testcontainers.Containers.DockerContainer.StopAsync(CancellationToken ct) in /_/src/Testcontainers/Containers/DockerContainer.cs:line 319
            */

            testOutputHelper.WriteLine($"Exception during StopAsync: {ex}");
        }
    }

    private async Task<WireMockContainer> Given_WireMockContainerIsStartedForHttpAndGrpcAsync()
    {
        var port = PortUtils.FindFreeTcpPort();
        var wireMockContainer = new WireMockContainerBuilder()
            .WithLogger(_logger)
            .AddUrl($"grpc://*:{port}")
            .Build();

        await wireMockContainer.StartAsync();

        return wireMockContainer;
    }

    private async Task<WireMockContainer> Given_WireMockContainerWithProtoDefinitionAtServerLevelIsStartedForHttpAndGrpcAsync()
    {
        var port = PortUtils.FindFreeTcpPort();
        var wireMockContainer = new WireMockContainerBuilder()
            .WithLogger(_logger)
            .AddUrl($"grpc://*:{port}")
            .AddProtoDefinition("my-greeter", ReadFile("greet.proto"))
            .Build();

        await wireMockContainer.StartAsync();

        return wireMockContainer;
    }

    private async Task<WireMockContainer> Given_WireMockContainerWithProtoDefinitionAtServerLevelWithWatchStaticMappingsIsStartedForHttpAndGrpcAsync()
    {
        var port = PortUtils.FindFreeTcpPort();
        var wireMockContainer = new WireMockContainerBuilder()
            .WithLogger(_logger)
            .AddUrl($"grpc://*:{port}")
            .AddProtoDefinition("my-greeter", ReadFile("greet.proto"))
            .WithMappings(Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings"))
            .Build();

        await wireMockContainer.StartAsync();

        return wireMockContainer;
    }

    private static async Task Given_ProtoBufMappingIsAddedViaAdminInterfaceAsync(WireMockContainer wireMockContainer, string filename)
    {
        var mappingsJson = ReadFile(filename);

        using var httpClient = wireMockContainer.CreateClient();

        var result = await httpClient.PostAsync("/__admin/mappings", new StringContent(mappingsJson, Encoding.UTF8, WireMockConstants.ContentTypeJson));
        result.EnsureSuccessStatusCode();
    }

    private static void Then_ReplyMessage_Should_BeCorrect(HelloReply reply)
    {
        reply.Message.Should().Be("hello stef POST");
    }

    private static string ReadFile(string filename)
    {
        return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", filename));
    }
}
#endif