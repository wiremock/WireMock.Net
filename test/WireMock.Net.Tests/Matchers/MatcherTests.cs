using FluentAssertions;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class MatcherTests
{
    [Fact]
    public void ContentTypeMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new ContentTypeMatcher("application/json");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new ContentTypeMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, \"application/json\", false)");
    }

    [Fact]
    public void ExactMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new ExactMatcher("test");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new ExactMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, false, WireMock.Matchers.MatchOperator.Or, \"test\")");
    }

    [Fact]
    public void ExactObjectMatcher_GetCSharpCodeArguments_ShouldReturnNotImplemented()
    {
        // Arrange
        var matcher = new ExactObjectMatcher(new { Name = "test" });

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("NotImplemented");
    }

    [Fact]
    public void FormUrlEncodedMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new FormUrlEncodedMatcher("key=value");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new FormUrlEncodedMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, \"key=value\", false, WireMock.Matchers.MatchOperator.Or)");
    }

    [Fact]
    public void JmesPathMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new JmesPathMatcher("expression");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new JmesPathMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, WireMock.Matchers.MatchOperator.Or, \"expression\")");
    }

    [Fact]
    public void JsonMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new JsonMatcher(new { key = "value" });

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().StartWith("new JsonMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch,");
    }

    [Fact]
    public void JsonPartialMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new JsonPartialMatcher(new { key = "value" });

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().StartWith("new JsonPartialMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch,");
    }

    [Fact]
    public void JsonPartialWildcardMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new JsonPartialWildcardMatcher(new { key = "value" });

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().StartWith("new JsonPartialWildcardMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch,");
    }

    //[Fact]
    //public void LinqMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    //{
    //    // Arrange
    //    var matcher = new LinqMatcher("it.Contains(\"test\"");

    //    // Act
    //    var result = matcher.GetCSharpCodeArguments();

    //    // Assert
    //    result.Should().Be("new LinqMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, WireMock.Matchers.MatchOperator.Or, \"it.Contains(\\\"test\\\"\")");
    //}

    [Fact]
    public void RegexMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new RegexMatcher("pattern");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new RegexMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, \"pattern\", false, true, WireMock.Matchers.MatchOperator.Or)");
    }

    [Fact]
    public void SimMetricsMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new SimMetricsMatcher("test");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new SimMetricsMatcher.Levenstein(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, \"test\", SimMetrics.Net.SimMetricType.Levenstein, WireMock.Matchers.MatchOperator.Average)");
    }

    [Fact]
    public void WildcardMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new WildcardMatcher("pattern");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new WildcardMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, \"pattern\", false, WireMock.Matchers.MatchOperator.Or)");
    }

    [Fact]
    public void XPathMatcher_GetCSharpCodeArguments_ShouldReturnCorrectArguments()
    {
        // Arrange
        var matcher = new XPathMatcher("pattern1");

        // Act
        var result = matcher.GetCSharpCodeArguments();

        // Assert
        result.Should().Be("new XPathMatcher(WireMock.Matchers.MatchBehaviour.AcceptOnMatch, WireMock.Matchers.MatchOperator.Or, null, \"pattern1\")");
    }
}