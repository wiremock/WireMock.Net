// Copyright © WireMock.Net

using FluentAssertions;
using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class NotNullOrEmptyMatcherTests
{
    [Fact]
    public void NotNullOrEmptyMatcher_GetName()
    {
        // Act
        var matcher = new NotNullOrEmptyMatcher();
        var name = matcher.Name;

        // Assert
        Check.That(name).Equals("NotNullOrEmptyMatcher");
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData(new byte[0], 0.0)]
    [InlineData(new byte[] { 48 }, 1.0)]
    public void NotNullOrEmptyMatcher_IsMatch_ByteArray(byte[]? data, double expected)
    {
        // Act
        var matcher = new NotNullOrEmptyMatcher();
        var result = matcher.IsMatch(data).Score;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData("", 0.0)]
    [InlineData("x", 1.0)]
    public void NotNullOrEmptyMatcher_IsMatch_String(string? @string, double expected)
    {
        // Act
        var matcher = new NotNullOrEmptyMatcher();
        var result = matcher.IsMatch(@string).Score;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData("", 0.0)]
    [InlineData("x", 1.0)]
    public void NotNullOrEmptyMatcher_IsMatch_StringAsObject(string? @string, double expected)
    {
        // Act
        var matcher = new NotNullOrEmptyMatcher();
        var result = matcher.IsMatch((object?)@string).Score;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void NotNullOrEmptyMatcher_IsMatch_Json()
    {
        // Act
        var matcher = new NotNullOrEmptyMatcher();
        var result = matcher.IsMatch(new { x = "x" }).Score;

        // Assert
        result.Should().Be(1.0);
    }

    [Fact]
    public void NotNullOrEmptyMatcher_GetPatterns_Should_Return_EmptyArray()
    {
        // Act
        var patterns = new NotNullOrEmptyMatcher().GetPatterns();

        // Assert
        patterns.Should().BeEmpty();
    }
}