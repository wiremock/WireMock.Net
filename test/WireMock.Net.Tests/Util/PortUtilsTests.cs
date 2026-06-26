// Copyright © WireMock.Net

using WireMock.Util;

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
        port.Should().Be(default);
    }

    [Fact]
    public void PortUtils_TryExtract_DefaultPort_UnknownScheme_Returns_False()
    {
        // Assign
        var url = "grpc://0.0.0.0";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeFalse();
        isHttps.Should().BeFalse();
        isGrpc.Should().BeFalse();
        scheme.Should().BeNull();
        host.Should().BeNull();
        port.Should().Be(default);
    }

    [Fact]
    public void PortUtils_TryExtract_DefaultPort_Http_Returns_True()
    {
        // Assign
        var url = "http://0.0.0.0";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeTrue();
        isHttps.Should().BeFalse();
        isGrpc.Should().BeFalse();
        scheme.Should().Be("http");
        host.Should().Be("0.0.0.0");
        port.Should().Be(80);
    }

    [Fact]
    public void PortUtils_TryExtract_DefaultPort_Https_Returns_True()
    {
        // Assign
        var url = "https://0.0.0.0";

        // Act
        var result = PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var scheme, out var host, out var port);

        // Assert
        result.Should().BeTrue();
        isHttps.Should().BeTrue();
        isGrpc.Should().BeFalse();
        scheme.Should().Be("https");
        host.Should().Be("0.0.0.0");
        port.Should().Be(443);
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