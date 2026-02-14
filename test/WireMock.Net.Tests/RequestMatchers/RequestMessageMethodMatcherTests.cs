// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageMethodMatcherTests
{
    [Theory]
    [InlineData("get", 1d)]
    [InlineData("post", 1d)]
    [InlineData("trace", 0d)]
    public void RequestMessageMethodMatcherTests_GetMatchingScore_Or(string method, double expected)
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), method, "127.0.0.1");
        var matcher = new RequestMessageMethodMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, "Get", "Post");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);
    }

    [Theory]
    [InlineData("get", 0.5d)]
    [InlineData("post", 0.5d)]
    [InlineData("trace", 0d)]
    public void RequestMessageMethodMatcherTests_GetMatchingScore_Average(string method, double expected)
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), method, "127.0.0.1");
        var matcher = new RequestMessageMethodMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Average, "Get", "Post");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);
    }

    [Fact]
    public void RequestMessageMethodMatcherTests_GetMatchingScore_And()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), "get", "127.0.0.1");
        var matcher = new RequestMessageMethodMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.And, "Get", "Post");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(0d);
    }
}