// Copyright © WireMock.Net

using System.Linq;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class RequestMatchResultTests
{
    [Fact]
    public void CompareTo_WithDifferentAverageScore_ReturnsBestMatch()
    {
        // Arrange
        var result1 = new RequestMatchResult();
        result1.AddMatchResult(typeof(WildcardMatcher), 1, null);
        result1.AddMatchResult(typeof(WildcardMatcher), 0.9, null);

        var result2 = new RequestMatchResult();
        result2.AddMatchResult(typeof(JsonMatcher), 1, null);
        result2.AddMatchResult(typeof(JsonMatcher), 1, null);

        var results = new[] { result1, result2 };

        // Act
        var best = results.OrderBy(x => x).First();

        // Assert
        best.Should().Be(result2);
    }

    [Fact]
    public void CompareTo_WithSameAverageScoreButMoreMatchers_ReturnsMatchWithMoreMatchers()
    {
        // Arrange
        var result1 = new RequestMatchResult();
        result1.AddMatchResult(typeof(WildcardMatcher), 1, null);
        result1.AddMatchResult(typeof(WildcardMatcher), 1, null);

        var result2 = new RequestMatchResult();
        result2.AddMatchResult(typeof(JsonMatcher), 1, null);
        result2.AddMatchResult(typeof(JsonMatcher), 1, null);
        result2.AddMatchResult(typeof(JsonMatcher), 1, null);

        var results = new[] { result1, result2 };

        // Act
        var best = results.OrderBy(x => x).First();

        // Assert
        best.Should().Be(result2);
    }
}