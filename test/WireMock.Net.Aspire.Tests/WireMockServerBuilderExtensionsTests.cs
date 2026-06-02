// Copyright © WireMock.Net

using System.Net.Sockets;
using AwesomeAssertions;
using Moq;
using WireMock.Util;

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
    public void AddWireMock_WithInvalidAdditionalUrls_ShouldThrowArgumentException()
    {
        // Arrange
        string[] invalidUrls = { "err" };
        var builder = Mock.Of<IDistributedApplicationBuilder>();

        // Act
        Action act = () => builder.AddWireMock("ValidName", invalidUrls);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The URL 'err' is not valid.");
    }

    [Fact]
    public void AddWireMockWithPort()
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
            HttpPorts = [port]
        });
        wiremock.Resource.Annotations.Should().HaveCount(6);

        var containerImageAnnotation = wiremock.Resource.Annotations.OfType<ContainerImageAnnotation>().FirstOrDefault();
        containerImageAnnotation.Should().BeEquivalentTo(new ContainerImageAnnotation
        {
            Image = "sheyenrath/wiremock.net-alpine",
            Registry = null,
            Tag = "latest"
        });

        var endpointAnnotation = wiremock.Resource.Annotations.OfType<EndpointAnnotation>().First();
        endpointAnnotation.Protocol.Should().Be(ProtocolType.Tcp);
        endpointAnnotation.UriScheme.Should().Be("http");
        endpointAnnotation.Port.Should().Be(port);
        endpointAnnotation.TargetPort.Should().Be(80);

        wiremock.Resource.Annotations.OfType<EnvironmentCallbackAnnotation>().FirstOrDefault().Should().NotBeNull();
        wiremock.Resource.Annotations.OfType<CommandLineArgsCallbackAnnotation>().FirstOrDefault().Should().NotBeNull();
        wiremock.Resource.Annotations.OfType<ResourceCommandAnnotation>().FirstOrDefault().Should().NotBeNull();
    }

    [Fact]
    public void AddWireMockWithAdditionalUrls()
    {
        // Arrange
        var name = $"apiservice{Guid.NewGuid()}";
        var freePorts = PortUtils.FindFreeTcpPorts(2).ToList();
        string[] additionalUrls = { $"http://*:{freePorts[0]}", $"grpc://*:{freePorts[1]}" };
        const string username = "admin";
        const string password = "test";
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var wiremock = builder
            .AddWireMock(name, additionalUrls)
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
            HttpPorts = freePorts,
            AdditionalUrls = additionalUrls.ToList()
        });
        wiremock.Resource.Annotations.Should().HaveCount(9);

        var containerImageAnnotation = wiremock.Resource.Annotations.OfType<ContainerImageAnnotation>().FirstOrDefault();
        containerImageAnnotation.Should().BeEquivalentTo(new ContainerImageAnnotation
        {
            Image = "sheyenrath/wiremock.net-alpine",
            Registry = null,
            Tag = "latest"
        });

        var endpointAnnotations = wiremock.Resource.Annotations.OfType<EndpointAnnotation>().ToArray();
        endpointAnnotations.Should().HaveCount(3);

        var endpointAnnotationForHttp80 = endpointAnnotations[0];
        endpointAnnotationForHttp80.Protocol.Should().Be(ProtocolType.Tcp);
        endpointAnnotationForHttp80.UriScheme.Should().Be("http");
        endpointAnnotationForHttp80.Port.Should().BeNull();
        endpointAnnotationForHttp80.TargetPort.Should().Be(80);

        var endpointAnnotationForHttpFreePort = endpointAnnotations[1];
        endpointAnnotationForHttpFreePort.Protocol.Should().Be(ProtocolType.Tcp);
        endpointAnnotationForHttpFreePort.UriScheme.Should().Be("http");
        endpointAnnotationForHttpFreePort.Name.Should().Be($"http-{freePorts[0]}");
        endpointAnnotationForHttpFreePort.Port.Should().Be(freePorts[0]);
        endpointAnnotationForHttpFreePort.TargetPort.Should().Be(freePorts[0]);

        var endpointAnnotationForGrpcFreePort = endpointAnnotations[2];
        endpointAnnotationForGrpcFreePort.Protocol.Should().Be(ProtocolType.Tcp);
        endpointAnnotationForGrpcFreePort.UriScheme.Should().Be("grpc");
        endpointAnnotationForGrpcFreePort.Name.Should().Be($"grpc-{freePorts[1]}");
        endpointAnnotationForGrpcFreePort.Port.Should().Be(freePorts[1]);
        endpointAnnotationForGrpcFreePort.TargetPort.Should().Be(freePorts[1]);

        wiremock.Resource.Annotations.OfType<EnvironmentCallbackAnnotation>().FirstOrDefault().Should().NotBeNull();
        wiremock.Resource.Annotations.OfType<CommandLineArgsCallbackAnnotation>().FirstOrDefault().Should().NotBeNull();
        wiremock.Resource.Annotations.OfType<ResourceCommandAnnotation>().FirstOrDefault().Should().NotBeNull();
    }
}