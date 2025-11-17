// Copyright © WireMock.Net

using System.Net.Sockets;
using FluentAssertions;
using Moq;

namespace WireMock.Net.Aspire.Tests;

public class WireMockServerBuilderExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void AddWireMock_WithNullOrWhiteSpaceName_ShouldThrowException(string? name)
    {
        // Arrange
        var builder = Mock.Of<IDistributedApplicationBuilder>();

        // Act
        Action act = () => builder.AddWireMock(name!, 12345);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void AddWireMock_WithInvalidPort_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int invalidPort = -1;
        var builder = Mock.Of<IDistributedApplicationBuilder>();

        // Act
        Action act = () => builder.AddWireMock("ValidName", invalidPort);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Specified argument was out of the range of valid values. (Parameter 'port')");
    }

    [Fact]
    public void AddWireMock()
    {
        // Arrange
        var name = $"apiservice{Guid.NewGuid()}";
        const int port = 12345;
        const string username = "admin";
        const string password = "test";
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var wiremock = builder
            .AddWireMock(name, port)
            .WithAdminUserNameAndPassword(username, password)
            .WithReadStaticMappings();

        // Assert
        wiremock.Resource.Should().NotBeNull();
        wiremock.Resource.Name.Should().Be(name);
        wiremock.Resource.Arguments.Should().BeEquivalentTo(new WireMockServerArguments
        {
            AdminPassword = password,
            AdminUsername = username,
            ReadStaticMappings = true,
            WatchStaticMappings = false,
            MappingsPath = null,
            HttpPort = port
        });
        wiremock.Resource.Annotations.Should().HaveCount(6);

        var containerImageAnnotation = wiremock.Resource.Annotations.OfType<ContainerImageAnnotation>().FirstOrDefault();
        containerImageAnnotation.Should().BeEquivalentTo(new ContainerImageAnnotation
        {
            Image = "sheyenrath/wiremock.net-alpine",
            Registry = null,
            Tag = "latest"
        });

        var endpointAnnotation = wiremock.Resource.Annotations.OfType<EndpointAnnotation>().FirstOrDefault();
        endpointAnnotation.Should().BeEquivalentTo(new EndpointAnnotation(
            protocol: ProtocolType.Tcp,
            uriScheme: "http",
            transport: null,
            name: null,
            port: port,
            targetPort: 80,
            isExternal: null,
            isProxied: true
        ));

        wiremock.Resource.Annotations.OfType<EnvironmentCallbackAnnotation>().FirstOrDefault().Should().NotBeNull();

        wiremock.Resource.Annotations.OfType<CommandLineArgsCallbackAnnotation>().FirstOrDefault().Should().NotBeNull();

        wiremock.Resource.Annotations.OfType<ResourceCommandAnnotation>().FirstOrDefault().Should().NotBeNull();
    }
}