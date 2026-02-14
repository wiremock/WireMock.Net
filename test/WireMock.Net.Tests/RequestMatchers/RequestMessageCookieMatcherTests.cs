// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageCookieMatcherTests
{
    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_AcceptOnMatch_CookieDoesNotExists()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1");
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "c", false, "x");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(0.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_RejectOnMatch_CookieDoesNotExists()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1");
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.RejectOnMatch, "c", false, "x");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_AcceptOnMatch()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "h", "x" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "h", false, "x");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_RejectOnMatch()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "h", "x" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.RejectOnMatch, "h", false, "x");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(0.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_IStringMatcher_Match()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "cook", "x" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "cook", false, new ExactMatcher("x"));

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_WithMissingCookie_When_RejectOnMatch_Is_True_Should_Match()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "cook", "x" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.RejectOnMatch, "uhuh", false, new ExactMatcher("x"));

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_Func_Match()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "cook", "x" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(x => x.ContainsKey("cook"));

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_CaseIgnoreForCookieValue()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "cook", "teST" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "cook", "test", true);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageCookieMatcher_GetMatchingScore_CaseIgnoreForCookieName()
    {
        // Assign
        var cookies = new Dictionary<string, string> { { "cook", "teST" } };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
        var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "CooK", "test", true);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }
}