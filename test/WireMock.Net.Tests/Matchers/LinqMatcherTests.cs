// Copyright © WireMock.Net

using FluentAssertions;
using Newtonsoft.Json.Linq;

using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class LinqMatcherTests
{
    [Fact]
    public void LinqMatcher_For_String_SinglePattern_IsMatch_Positive()
    {
        // Assign
        string input = "2018-08-31 13:59:59";

        // Act
        var matcher = new LinqMatcher("DateTime.Parse(it) > \"2018-08-01 13:50:00\"");

        // Assert
        var score = matcher.IsMatch(input).Score;
        score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void LinqMatcher_For_String_IsMatch_Negative()
    {
        // Assign
        string input = "2018-08-31 13:59:59";

        // Act
        var matcher = new LinqMatcher("DateTime.Parse(it) > \"2019-01-01 00:00:00\"");

        // Assert
        var score = matcher.IsMatch(input).Score;
        score.Should().Be(MatchScores.Mismatch);
    }

    [Fact]
    public void LinqMatcher_For_String_IsMatch_RejectOnMatch()
    {
        // Assign
        string input = "2018-08-31 13:59:59";

        // Act
        var matcher = new LinqMatcher(MatchBehaviour.RejectOnMatch, "DateTime.Parse(it) > \"2018-08-01 13:50:00\"");

        // Assert
        var score = matcher.IsMatch(input).Score;
        score.Should().Be(MatchScores.Mismatch);
    }

    [Fact]
    public void LinqMatcher_For_Object_IsMatch()
    {
        // Assign
        var input = new
        {
            Id = 9,
            Name = "Test"
        };

        // Act
        var matcher = new LinqMatcher("Id > 1 AND Name == \"Test\"");
        double match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void LinqMatcher_For_JObject_IsMatch()
    {
        // Assign
        var input = new JObject
        {
            { "IntegerId", new JValue(9) },
            { "LongId", new JValue(long.MaxValue) },
            { "Name", new JValue("Test") }
        };

        // Act
        var matcher = new LinqMatcher("IntegerId > 1 AND LongId > 1 && Name == \"Test\"");
        double match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void LinqMatcher_GetName()
    {
        // Assign
        var matcher = new LinqMatcher("x");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("LinqMatcher");
    }

    [Fact]
    public void LinqMatcher_GetPatterns()
    {
        // Assign
        var matcher = new LinqMatcher("x");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        patterns.Should().ContainExactly("x");
    }
}
