// Copyright © WireMock.Net

using System.Net.Http;
using AwesomeAssertions;
using WireMock.Server;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    private const string RequestXml =
        """
        <xml>
        	<Contact FirstName = "Stef" />
        </xml>
        """;

    private readonly string _responseFilePath = Path.Combine(Environment.CurrentDirectory, "__admin", "mappings", "responseWithTransformer.xml");

    [Fact]
    public async Task WireMockServer_WithTransformer_WithBody_ShouldWork()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
            .WhenRequest(req => req
                .WithPath("/withbody")
                .UsingPost())
            .ThenRespondWith(rsp => rsp
                .WithSuccess()
                .WithBody(File.ReadAllText(_responseFilePath))
                .WithTransformer());

        // Act
        var response = await GetResponseAsync(server, "/withbody");

        // Assert
        response.Should().Contain("Hello, Stef!");
    }

    [Fact]
    public async Task WireMockServer_WithTransformer_WithJsonBodyAsArray_ShouldWork()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
            .WhenRequest(req => req
                .WithPath("/withbody")
                .UsingPost()
            )
            .ThenRespondWith(rsp => rsp
                .WithSuccess()
                .WithBodyAsJson(new[]
                    {
                        new
                        {
                            test = "test",
                            secret = true
                        },
                        new
                        {
                            test = "123",
                            secret = false
                        }
                    }
                )
                .WithTransformer()
            );

        // Act
        var response = await GetResponseAsync(server, "/withbody");

        // Assert
        response.Should().Be("""[{"test":"test","secret":true},{"test":"123","secret":false}]""");
    }

    [Fact]
    public async Task WireMockServer_WithTransformer_WithJsonBodyAsList_ShouldWork()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
            .WhenRequest(req => req
                .WithPath("/withbody")
                .UsingPost()
            )
            .ThenRespondWith(rsp => rsp
                .WithSuccess()
                .WithBodyAsJson(
                    new List<object>
                    {
                        new
                        {
                            test = "test",
                            secret = true
                        },
                        new
                        {
                            test = "123",
                            secret = false
                        }
                    })
                .WithTransformer()
            );

        // Act
        var response = await GetResponseAsync(server, "/withbody");

        // Assert
        response.Should().Be("""[{"test":"test","secret":true},{"test":"123","secret":false}]""");
    }

    [Fact]
    public async Task WireMockServer_WithTransformerBefore_WithBodyFromFile_ShouldWork()
    {
        // Arrange
        using var server = WireMockServer.Start();
        server
            .WhenRequest(req => req
                .WithPath("/withbodyfromfile")
                .UsingPost())
            .ThenRespondWith(rsp => rsp
                .WithSuccess()
                .WithTransformer(transformContentFromBodyAsFile: true)
                .WithBodyFromFile(_responseFilePath));

        // Act
        var response = await GetResponseAsync(server, "/withbodyfromfile");

        // Assert
        response.Should().Contain("Hello, Stef!");
    }

    [Fact]
    public void WireMockServer_WithTransformerAfter_WithBodyFromFile_ShouldThrow()
    {
        // Act
        var act = () =>
        {
            using var server = WireMockServer.Start();
            server
                .WhenRequest(req => req
                    .WithPath("/")
                    .UsingPost())
                .ThenRespondWith(rsp => rsp
                    .WithSuccess()
                    .WithBodyFromFile(_responseFilePath)
                    .WithTransformer(transformContentFromBodyAsFile: true));
        };

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("WithTransformer should be used before WithBodyFromFile.");
    }

    private static async Task<string> GetResponseAsync(WireMockServer server, string relativePath)
    {
        using var httpClient = server.CreateClient();

        using var requestContent = new StringContent(RequestXml);
        using var responseMsg = await httpClient.PostAsync(relativePath, requestContent);
        return await responseMsg.Content.ReadAsStringAsync();
    }
}