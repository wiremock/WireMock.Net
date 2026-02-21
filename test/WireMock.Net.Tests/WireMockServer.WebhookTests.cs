// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using AwesomeAssertions;
using Moq;
using WireMock.Logging;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests;

public class WireMockServerWebhookTests
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    [Fact]
    public async Task WireMockServer_WithWebhooks_Should_Send_Message_To_Webhooks()
    {
        // Assign
        var serverReceivingTheWebhook1 = WireMockServer.Start();
        serverReceivingTheWebhook1.Given(Request.Create().UsingPost()).RespondWith(Response.Create().WithStatusCode(200));

        var serverReceivingTheWebhook2 = WireMockServer.Start();
        serverReceivingTheWebhook2.Given(Request.Create().UsingPost()).RespondWith(Response.Create().WithStatusCode(200));

        var webhook1 = new Webhook
        {
            Request = new WebhookRequest
            {
                Url = serverReceivingTheWebhook1.Urls[0],
                Method = "post",
                BodyData = new BodyData
                {
                    BodyAsString = "1",
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = BodyType.String
                }
            }
        };

        var webhook2 = new Webhook
        {
            Request = new WebhookRequest
            {
                Url = serverReceivingTheWebhook2.Urls[0],
                Method = "post",
                BodyData = new BodyData
                {
                    BodyAsString = "2",
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = BodyType.String
                }
            }
        };

        // Act
        var server = WireMockServer.Start();
        server.Given(Request.Create().UsingPost())
            .WithWebhook(webhook1, webhook2)
            .RespondWith(Response.Create().WithBody("a-response"));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/TST"),
            Content = new StringContent("test")
        };

        // Assert
        var response = await new HttpClient().SendAsync(request, _ct);
        var content = await response.Content.ReadAsStringAsync(_ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("a-response");

        serverReceivingTheWebhook1.LogEntries.Should().HaveCount(1);
        serverReceivingTheWebhook2.LogEntries.Should().HaveCount(1);

        server.Dispose();
        serverReceivingTheWebhook1.Dispose();
        serverReceivingTheWebhook2.Dispose();
    }

    [Fact]
    public async Task WireMockServer_WithWebhook_Should_Send_Message_To_Webhook()
    {
        // Assign
        var serverReceivingTheWebhook = WireMockServer.Start();
        serverReceivingTheWebhook.Given(Request.Create().WithPath("/x").UsingPost()).RespondWith(Response.Create().WithStatusCode(200));

        // Act
        var server = WireMockServer.Start();
        server.Given(Request.Create().UsingPost())
            .WithWebhook(new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = serverReceivingTheWebhook.Url! + "/{{request.Query.q}}",
                    Method = "post",
                    BodyData = new BodyData
                    {
                        BodyAsString = "abc",
                        DetectedBodyType = BodyType.String,
                        DetectedBodyTypeFromContentType = BodyType.String
                    },
                    UseTransformer = true
                }
            })
            .RespondWith(Response.Create().WithBody("a-response"));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/TST?q=x"),
            Content = new StringContent("test")
        };

        // Assert
        var response = await new HttpClient().SendAsync(request, _ct);
        var content = await response.Content.ReadAsStringAsync(_ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("a-response");

        serverReceivingTheWebhook.LogEntries.Should().HaveCount(1);
        serverReceivingTheWebhook.LogEntries.First().MappingGuid.Should().NotBeNull();

        server.Dispose();
        serverReceivingTheWebhook.Dispose();
    }

    [Fact]
    public async Task WireMockServer_WithWebhook_When_WebhookEndPointReturnsError_Should_LogWarning()
    {
        // Arrange
        var serverReceivingTheWebhook = WireMockServer.Start();
        serverReceivingTheWebhook.Given(Request.Create().WithPath("/x").UsingPost()).RespondWith(Response.Create().WithBody("!Server Error!").WithStatusCode(500));

        var loggerMock = new Mock<IWireMockLogger>();
        var settings = new WireMockServerSettings
        {
            Logger = loggerMock.Object
        };

        // Act
        var guid = "942cb963-c9a3-4e9c-8e71-c1b26d2a4a05";
        var server = WireMockServer.Start(settings);
        server.Given(Request.Create().UsingPost())
            .WithWebhook(new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = serverReceivingTheWebhook.Url! + "/{{request.Query.q}}",
                    Method = "post",
                    BodyData = new BodyData
                    {
                        BodyAsString = "abc",
                        DetectedBodyType = BodyType.String,
                        DetectedBodyTypeFromContentType = BodyType.String
                    },
                    UseTransformer = true
                }
            })
            .WithGuid(guid)
            .RespondWith(Response.Create().WithBody("a-response"));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/TST?q=x"),
            Content = new StringContent("test")
        };

        // Assert
        var response = await new HttpClient().SendAsync(request, _ct);
        var content = await response.Content.ReadAsStringAsync(_ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("a-response");

        loggerMock.Verify(l => l.Warn("Sending message to Webhook [0] from Mapping '942cb963-c9a3-4e9c-8e71-c1b26d2a4a05' failed. HttpStatusCode: InternalServerError Content: !Server Error!"));

        serverReceivingTheWebhook.LogEntries.Should().HaveCount(1);
        serverReceivingTheWebhook.LogEntries.First().MappingGuid.Should().NotBeNull();

        server.Dispose();
        serverReceivingTheWebhook.Dispose();
    }

    [Fact]
    public async Task WireMockServer_WithWebhook_When_WebhookEndPointDoesNotExists_Should_LogError()
    {
        // Arrange
        var loggerMock = new Mock<IWireMockLogger>();
        var settings = new WireMockServerSettings
        {
            Logger = loggerMock.Object
        };

        // Act
        var guid = "942cb963-c9a3-4e9c-8e71-c1b26d2a4a05";
        var server = WireMockServer.Start(settings);
        server.Given(Request.Create().UsingPost())
            .WithWebhook(new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = "http://error-not-does-exist-" + Guid.NewGuid(),
                    Method = "post",
                    BodyData = new BodyData
                    {
                        BodyAsString = "abc",
                        DetectedBodyType = BodyType.String,
                        DetectedBodyTypeFromContentType = BodyType.String
                    },
                    UseTransformer = true
                }
            })
            .WithGuid(guid)
            .RespondWith(Response.Create().WithBody("a-response"));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(server.Urls[0]),
            Content = new StringContent("test")
        };

        // Assert
        var response = await new HttpClient().SendAsync(request, _ct);
        var content = await response.Content.ReadAsStringAsync(_ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("a-response");

        loggerMock.Verify(l => l.Error(It.Is<string>(formatString => formatString.Contains("Sending message to Webhook [0] from Mapping '942cb963-c9a3-4e9c-8e71-c1b26d2a4a05' failed. Exception"))));

        server.Dispose();
    }

    [Fact]
    public async Task WireMockServer_WithWebhookArgs_Should_Send_StringMessage_To_Webhook()
    {
        // Assign
        var serverReceivingTheWebhook = WireMockServer.Start();
        serverReceivingTheWebhook.Given(Request.Create().UsingPost()).RespondWith(Response.Create().WithStatusCode(200));

        // Act
        var server = WireMockServer.Start();
        server.Given(Request.Create().UsingPost())
            .WithWebhook(serverReceivingTheWebhook.Urls[0], "post", null, "OK !")
            .RespondWith(Response.Create().WithBody("a-response"));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/TST"),
            Content = new StringContent("test")
        };

        // Assert
        var response = await new HttpClient().SendAsync(request, _ct);
        var content = await response.Content.ReadAsStringAsync(_ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("a-response");

        serverReceivingTheWebhook.LogEntries.Should().HaveCount(1);
        serverReceivingTheWebhook.LogEntries.First().RequestMessage!.Body.Should().Be("OK !");

        server.Dispose();
        serverReceivingTheWebhook.Dispose();
    }

    [Fact]
    public async Task WireMockServer_WithWebhookArgs_Should_Send_JsonMessage_To_Webhook()
    {
        // Assign
        var serverReceivingTheWebhook = WireMockServer.Start();
        serverReceivingTheWebhook.Given(Request.Create().UsingPost()).RespondWith(Response.Create().WithStatusCode(200));

        // Act
        var server = WireMockServer.Start();
        server.Given(Request.Create().UsingPost())
            .WithWebhook(serverReceivingTheWebhook.Urls[0], "post", null, new { Status = "OK" })
            .RespondWith(Response.Create().WithBody("a-response"));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/TST"),
            Content = new StringContent("test")
        };

        // Assert
        var response = await new HttpClient().SendAsync(request, _ct);
        var content = await response.Content.ReadAsStringAsync(_ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("a-response");

        serverReceivingTheWebhook.LogEntries.Should().HaveCount(1);
        serverReceivingTheWebhook.LogEntries.First().RequestMessage!.Body.Should().Be("{\"Status\":\"OK\"}");

        server.Dispose();
        serverReceivingTheWebhook.Dispose();
    }
}