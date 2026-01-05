// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Enums;
using Moq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

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
        Func<Task> action = () => responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        action.Should().ThrowAsync<HandlebarsRuntimeException>();
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
                    .Concat(new[] { Category.Environment })
                    .ToArray()
            }
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("User: {{Environment.GetEnvironmentVariable \"USERNAME\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, settingsWithEnv).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).Not.Contains("{{Environment.GetEnvironmentVariable");
        Check.That(response.Message.BodyData.BodyAsString).StartsWith("User: ");
    }

    [Fact]
    public void DefaultAllowedHandlebarsHelpers_Should_Not_Include_Environment()
    {
        // Assert
        Check.That(HandlebarsSettings.DefaultAllowedHandlebarsHelpers).Not.Contains(Category.Environment);
    }
}
