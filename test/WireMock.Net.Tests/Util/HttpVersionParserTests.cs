// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

[ExcludeFromCodeCoverage]
public class HttpVersionParserTests
{
    [Theory]
    [InlineData("HTTP/1.1", "1.1")]
    [InlineData("HTTP/2", "2")]
    [InlineData("http/1.0", "1.0")]
    [InlineData("HTTP/3", "3")]
    public void Parse_ValidHttpVersion_ReturnsCorrectVersion(string protocol, string expectedVersion)
    {
        // Act
        var version = HttpVersionParser.Parse(protocol);

        // Assert
        version.Should().Be(expectedVersion, "the input string is a valid HTTP protocol version");
    }

    [Theory]
    [InlineData("HTP/2")]
    [InlineData("HTTP/2.2.2")]
    [InlineData("HTTP/")]
    [InlineData("http//1.1")]
    [InlineData("")]
    public void Parse_InvalidHttpVersion_ReturnsEmptyString(string protocol)
    {
        // Act
        var version = HttpVersionParser.Parse(protocol);

        // Assert
        version.Should().BeEmpty("the input string is not a valid HTTP protocol version");
    }
}