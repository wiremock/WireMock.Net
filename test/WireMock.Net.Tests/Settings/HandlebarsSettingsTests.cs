// Copyright © WireMock.Net

using AwesomeAssertions;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Enums;
using Microsoft.AspNetCore.Http;
using Moq;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace WireMock.Net.Tests.Settings;

public class HandlebarsSettingsTests
{
    private const string ClientIp = "::1";

    private readonly WireMockServerSettings _settings;
    private readonly Mock<IMapping> _mappingMock;
    private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock;

    public HandlebarsSettingsTests()
    {
        _mappingMock = new Mock<IMapping>();

        _fileSystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);

        _settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object
        };
    }

    [Fact]
    public async Task Response_HandlebarsHelpers_Environment_NotAllowed_By_Default()
    {
        // Arrange
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("Username: {{Environment.GetEnvironmentVariable \"USERNAME\"}}")
            .WithTransformer();

        // Act
        Func<Task> action = () => responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        await action.Should().ThrowAsync<HandlebarsRuntimeException>();
    }

    [Fact]
    public async Task Response_HandlebarsHelpers_Environment_Allowed_When_Configured()
    {
        // Arrange
        var settingsWithEnv = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            HandlebarsSettings = new HandlebarsSettings
            {
                AllowedHandlebarsHelpers = HandlebarsSettings.DefaultAllowedHandlebarsHelpers
                    .Concat([Category.Environment])
                    .ToArray()
            }
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("User: {{Environment.GetEnvironmentVariable \"USERNAME\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, settingsWithEnv).ConfigureAwait(false);

        // Assert
        response.Message?.BodyData?.BodyAsString.Should().NotContain("{{Environment.GetEnvironmentVariable");
        response.Message?.BodyData?.BodyAsString.Should().StartWith("User: ");
    }

    [Fact]
    public void DefaultAllowedHandlebarsHelpers_Should_Not_Include_EnvironmentAndDynamicLinq()
    {
        // Assert
        HandlebarsSettings.DefaultAllowedHandlebarsHelpers.Should()
            .NotContain(Category.Environment)
            .And
            .NotContain(Category.DynamicLinq);
    }
}