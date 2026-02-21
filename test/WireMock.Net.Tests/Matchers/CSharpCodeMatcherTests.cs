// Copyright © WireMock.Net

using AwesomeAssertions;
using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class CSharpCodeMatcherTests
{
    [Fact]
    public void CSharpCodeMatcher_For_String_SinglePattern_IsMatch_Positive()
    {
        // Assign
        string input = "x";
        var matcher = new CSharpCodeMatcher("return it == \"x\";");

        // Act
        var score = matcher.IsMatch(input).Score;

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void CSharpCodeMatcher_For_String_IsMatch_Negative()
    {
        // Assign
        string input = "y";
        var matcher = new CSharpCodeMatcher("return it == \"x\";");

        // Act
        var score = matcher.IsMatch(input).Score;

        // Assert
        score.Should().Be(MatchScores.Mismatch);
    }

    [Fact]
    public void CSharpCodeMatcher_For_String_IsMatch_RejectOnMatch()
    {
        // Assign
        string input = "x";
        var matcher = new CSharpCodeMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, "return it == \"x\";");

        // Act
        var score = matcher.IsMatch(input).Score;

        // Assert
        score.Should().Be(MatchScores.Mismatch);
    }

    [Fact]
    public void CSharpCodeMatcher_For_Object_IsMatch()
    {
        // Assign
        var input = new
        {
            Id = 9,
            Name = "Test"
        };

        var matcher = new CSharpCodeMatcher("return it.Id > 1 && it.Name == \"Test\";");

        // Act
        var score = matcher.IsMatch(input).Score;

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void CSharpCodeMatcher_GetName()
    {
        // Assign
        var matcher = new CSharpCodeMatcher("x");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("CSharpCodeMatcher");
    }

    [Fact]
    public void CSharpCodeMatcher_GetPatterns()
    {
        // Assign
        var matcher = new CSharpCodeMatcher("x");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("x");
    }
}