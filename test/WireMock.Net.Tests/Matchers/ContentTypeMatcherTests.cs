// Copyright © WireMock.Net

using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class ContentTypeMatcherTests
{
    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json; charset=ascii")]
    [InlineData("application/json; charset=utf-8")]
    [InlineData("application/json; charset=UTF-8")]
    public void ContentTypeMatcher_IsMatchWithIgnoreCaseFalse_Positive(string contentType)
    {
        var matcher = new ContentTypeMatcher("application/json");

        // Act
        var score = matcher.IsMatch(contentType).Score;

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/JSON")]
    [InlineData("application/json; CharSet=ascii")]
    [InlineData("application/json; charset=utf-8")]
    [InlineData("application/json; charset=UTF-8")]
    public void ContentTypeMatcher_IsMatchWithIgnoreCaseTrue_Positive(string contentType)
    {
        var matcher = new ContentTypeMatcher("application/json", true);

        // Act
        var score = matcher.IsMatch(contentType).Score;

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void ContentTypeMatcher_GetName()
    {
        // Assign
        var matcher = new ContentTypeMatcher("x");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("ContentTypeMatcher");
    }

    [Fact]
    public void ContentTypeMatcher_GetPatterns()
    {
        // Assign
        var matcher = new ContentTypeMatcher("x");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        patterns.Should().ContainSingle("x");
    }
}