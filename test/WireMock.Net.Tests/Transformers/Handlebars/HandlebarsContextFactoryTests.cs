// Copyright © WireMock.Net

using FluentAssertions;
using Moq;
using WireMock.Handlers;
using WireMock.Settings;
using WireMock.Transformers.Handlebars;

namespace WireMock.Net.Tests.Transformers.Handlebars;

public class HandlebarsContextFactoryTests
{
    private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new();

    [Fact]
    public void Create_WithNullAction_DoesNotInvokeAction()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object
        };
        var sut = new HandlebarsContextFactory(settings);

        // Act
        var result = sut.Create();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithAction_InvokesAction()
    {
        // Arrange
        int num = 0;
        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            HandlebarsRegistrationCallback = (ctx, fs) =>
            {
                ctx.Should().NotBeNull();
                fs.Should().NotBeNull();

                num++;
            }
        };

        var sut = new HandlebarsContextFactory(settings);

        // Act
        var result = sut.Create();

        // Assert
        result.Should().NotBeNull();

        // Verify
        num.Should().Be(1);
    }
}