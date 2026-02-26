// Copyright © WireMock.Net

using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class RegexMatcherTests
{
    [Fact]
    public void RegexMatcher_GetName()
    {
        // Assign
        var matcher = new RegexMatcher("");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("RegexMatcher");
    }

    [Fact]
    public void RegexMatcher_GetPatterns()
    {
        // Assign
        var matcher = new RegexMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        patterns.Should().ContainSingle("X");
    }

    [Fact]
    public void RegexMatcher_GetIgnoreCase()
    {
        // Act
        bool case1 = new RegexMatcher("X").IgnoreCase;
        bool case2 = new RegexMatcher("X", true).IgnoreCase;

        // Assert
        case1.Should().BeFalse();
        case2.Should().BeTrue();
    }

    [Fact]
    public void RegexMatcher_IsMatch()
    {
        // Assign
        var matcher = new RegexMatcher("H.*o");

        // Act
        double result = matcher.IsMatch("Hello world!").Score;

        // Assert
        result.Should().Be(1.0d);
    }

    [Fact]
    public void RegexMatcher_IsMatch_NullInput()
    {
        // Assign
        var matcher = new RegexMatcher("H.*o");

        // Act
        double result = matcher.IsMatch(null).Score;

        // Assert
        result.Should().Be(0.0d);
    }

    [Fact]
    public void RegexMatcher_IsMatch_RegexExtended_Guid()
    {
        // Assign
        var matcher = new RegexMatcher(@"\GUIDB", true);

        // Act
        double result = matcher.IsMatch(Guid.NewGuid().ToString("B")).Score;

        // Assert
        result.Should().Be(1.0);
    }

    [Fact]
    public void RegexMatcher_IsMatch_Regex_Guid()
    {
        // Assign
        var matcher = new RegexMatcher(@"\GUIDB", true, false);

        // Act
        double result = matcher.IsMatch(Guid.NewGuid().ToString("B")).Score;

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void RegexMatcher_IsMatch_IgnoreCase()
    {
        // Assign
        var matcher = new RegexMatcher("H.*o", true);

        // Act
        double result = matcher.IsMatch("hello").Score;

        // Assert
        result.Should().Be(1.0d);
    }

    [Fact]
    public void RegexMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        var matcher = new RegexMatcher(MatchBehaviour.RejectOnMatch, "h.*o");

        // Act
        double result = matcher.IsMatch("hello").Score;

        // Assert
        result.Should().Be(0.0);
    }
}