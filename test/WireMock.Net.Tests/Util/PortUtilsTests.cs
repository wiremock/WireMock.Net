// Copyright © WireMock.Net

using FluentAssertions;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class PortUtilsTests
{
    [Fact]
    public void PortUtils_TryExtract_InvalidUrl_Returns_False()
    {
        // Assign
        var url = "test";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeFalse();
        isHttps.Should().BeFalse();
        isGrpc.Should().BeFalse();
        scheme.Should().BeNull();
        host.Should().BeNull();
        port.Should().Be(default(int));
    }

    [Fact]
    public void PortUtils_TryExtract_UrlIsMissingPort_Returns_False()
    {
        // Assign
        var url = "http://0.0.0.0";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeFalse();
        isHttps.Should().BeFalse();
        isGrpc.Should().BeFalse();
        scheme.Should().BeNull();
        host.Should().BeNull();
        port.Should().Be(default(int));
    }

    [Fact]
    public void PortUtils_TryExtract_Http_Returns_True()
    {
        // Assign
        var url = "http://wiremock.net:1234";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeTrue();
        isHttps.Should().BeFalse();
        isGrpc.Should().BeFalse();
        scheme.Should().Be("http");
        host.Should().Be("wiremock.net");
        port.Should().Be(1234);
    }

    [Fact]
    public void PortUtils_TryExtract_Https_Returns_True()
    {
        // Assign
        var url = "https://wiremock.net:5000";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeTrue();
        isHttps.Should().BeTrue();
        isGrpc.Should().BeFalse();
        scheme.Should().Be("https");
        host.Should().Be("wiremock.net");
        port.Should().Be(5000);
    }

    [Fact]
    public void PortUtils_TryExtract_Grpc_Returns_True()
    {
        // Assign
        var url = "grpc://wiremock.net:1234";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeTrue();
        isHttps.Should().BeFalse();
        isGrpc.Should().BeTrue();
        scheme.Should().Be("grpc");
        host.Should().Be("wiremock.net");
        port.Should().Be(1234);
    }

    [Fact]
    public void PortUtils_TryExtract_Https0_0_0_0_Returns_True()
    {
        // Assign
        var url = "https://0.0.0.0:5000";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeTrue();
        isHttps.Should().BeTrue();
        isGrpc.Should().BeFalse();
        scheme.Should().Be("https");
        host.Should().Be("0.0.0.0");
        port.Should().Be(5000);
    }
}