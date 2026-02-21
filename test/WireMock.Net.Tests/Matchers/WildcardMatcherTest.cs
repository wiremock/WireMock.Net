// Copyright © WireMock.Net

using AnyOfTypes;
using AwesomeAssertions;
using NFluent;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Net.Tests.Matchers;

public class WildcardMatcherTest
{
    [Fact]
    public void WildcardMatcher_IsMatch_With_StringPattern()
    {
        // Arrange
        var pattern = new StringPattern
        {
            Pattern = "*",
            PatternAsFile = "pf"
        };

        var matcher = new WildcardMatcher(pattern);

        // Act
        var score = matcher.IsMatch("a").Score;

        // Assert
        score.Should().Be(1.0d);
    }

    [Fact]
    public void WildcardMatcher_IsMatch_With_StringPatterns()
    {
        // Arrange
        AnyOf<string, StringPattern> pattern1 = new StringPattern
        {
            Pattern = "a"
        };
        AnyOf<string, StringPattern> pattern2 = new StringPattern
        {
            Pattern = "b"
        };

        var matcher = new WildcardMatcher(new [] { pattern1, pattern2 });

        // Act
        var score = matcher.IsMatch("a").Score;

        // Assert
        score.Should().Be(1.0d);
    }

    [Fact]
    public void WildcardMatcher_IsMatch_Positive()
    {
        var tests = new[]
        {
            new { p = "*", i = "" },
            new { p = "?", i = " "},
            new { p = "*", i = "a "},
            new { p = "*", i = "ab" },
            new { p = "?", i = "a" },
            new { p = "*?", i = "abc" },
            new { p = "?*", i = "abc" },
            new { p = "abc", i = "abc" },
            new { p = "abc*", i = "abc" },
            new { p = "abc*", i = "abcd" },
            new { p = "*abc*", i = "abc" },
            new { p = "*a*bc*", i = "abc" },
            new { p = "*a*b?", i = "aXXXbc" }
        };

        foreach (var test in tests)
        {
            var matcher = new WildcardMatcher(test.p);

            // Act
            var score = matcher.IsMatch(test.i).Score;

            // Assert
            score.Should().Be(MatchScores.Perfect, $"Pattern '{test.p}' with value '{test.i}' should be 1.0");
        }
    }

    [Fact]
    public void WildcardMatcher_IsMatch_Negative()
    {
        var tests = new[]
        {
            new { p = "*a", i = "" },
            new { p = "a*", i = "" },
            new { p = "?", i = "" },
            new { p = "*b*", i = "a" },
            new { p = "b*a", i = "ab" },
            new { p = "??", i = "a" },
            new { p = "*?", i = "" },
            new { p = "??*", i = "a" },
            new { p = "*abc", i = "abX" },
            new { p = "*abc*", i = "Xbc" },
            new { p = "*a*bc*", i = "ac" }
        };

        foreach (var test in tests)
        {
            var matcher = new WildcardMatcher(test.p);

            // Act
            var score = matcher.IsMatch(test.i).Score;

            // Assert
            score.Should().Be(MatchScores.Mismatch);
        }
    }

    [Fact]
    public void WildcardMatcher_GetName()
    {
        // Assign
        var matcher = new WildcardMatcher("x");

        // Act
        var name = matcher.Name;

        // Assert
        Check.That(name).Equals("WildcardMatcher");
    }

    [Fact]
    public void WildcardMatcher_GetPatterns()
    {
        // Assign
        var matcher = new WildcardMatcher("x");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly(new AnyOf<string, StringPattern>("x"));
    }

    [Fact]
    public void WildcardMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        var matcher = new WildcardMatcher(MatchBehaviour.RejectOnMatch, "m");

        // Act
        var result = matcher.IsMatch("m").Score;

        Check.That(result).IsEqualTo(0.0);
    }
}