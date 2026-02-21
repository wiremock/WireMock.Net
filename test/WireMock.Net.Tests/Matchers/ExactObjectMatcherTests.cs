// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class ExactObjectMatcherTests
{
    [Fact]
    public void ExactObjectMatcher_GetName()
    {
        // Assign
        object obj = 1;

        // Act
        var matcher = new ExactObjectMatcher(obj);
        var name = matcher.Name;

        // Assert
        Check.That(name).Equals("ExactObjectMatcher");
    }

    [Fact]
    public void ExactObjectMatcher_IsMatch_ByteArray()
    {
        // Assign
        object checkValue = new byte[] { 1, 2 };

        // Act
        var matcher = new ExactObjectMatcher([1, 2]);
        var score = matcher.IsMatch(checkValue).Score;

        // Assert
        Check.That(score).IsEqualTo(1.0);
    }

    [Fact]
    public void ExactObjectMatcher_IsMatch_AcceptOnMatch()
    {
        // Assign
        object obj = new { x = 500, s = "s" };

        // Act
        var matcher = new ExactObjectMatcher(obj);
        var score = matcher.IsMatch(new { x = 500, s = "s" }).Score;

        // Assert
        Check.That(score).IsEqualTo(1.0);
    }

    [Fact]
    public void ExactObjectMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        object obj = new { x = 500, s = "s" };

        // Act
        var matcher = new ExactObjectMatcher(MatchBehaviour.RejectOnMatch, obj);
        var score = matcher.IsMatch(new { x = 500, s = "s" }).Score;

        // Assert
        Check.That(score).IsEqualTo(0.0);
    }
}