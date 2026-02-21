// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class SimMetricsMatcherTests
{
    [Fact]
    public void SimMetricsMatcher_GetName()
    {
        // Assign
        var matcher = new SimMetricsMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("SimMetricsMatcher.Levenstein");
    }

    [Fact]
    public void SimMetricsMatcher_GetPatterns()
    {
        // Assign
        var matcher = new SimMetricsMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("X");
    }

    [Fact]
    public void SimMetricsMatcher_IsMatch_1()
    {
        // Assign
        var matcher = new SimMetricsMatcher("The cat walks in the street.");

        // Act
        double result = matcher.IsMatch("The car drives in the street.").Score;

        // Assert
        Check.That(result).IsStrictlyLessThan(1.0).And.IsStrictlyGreaterThan(0.5);
    }

    [Fact]
    public void SimMetricsMatcher_IsMatch_2()
    {
        // Assign
        var matcher = new SimMetricsMatcher("The cat walks in the street.");

        // Act
        double result = matcher.IsMatch("Hello").Score;

        // Assert
        Check.That(result).IsStrictlyLessThan(0.1).And.IsStrictlyGreaterThan(0.05);
    }

    [Fact]
    public void SimMetricsMatcher_IsMatch_AcceptOnMatch()
    {
        // Assign
        var matcher = new SimMetricsMatcher("test");

        // Act
        double result = matcher.IsMatch("test").Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void SimMetricsMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        var matcher = new SimMetricsMatcher(MatchBehaviour.RejectOnMatch, "test");

        // Act
        double result = matcher.IsMatch("test").Score;

        // Assert
        Check.That(result).IsEqualTo(0.0);
    }
}