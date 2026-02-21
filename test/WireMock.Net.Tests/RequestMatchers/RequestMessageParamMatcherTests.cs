// Copyright © WireMock.Net

using AwesomeAssertions;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Types;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageParamMatcherTests
{
    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_IgnoreCaseKeyWithValuesPresentInUrl_And_With1StringValues_Returns1_0()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "KeY", true, new[] { "test1" });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWith1ValuePresentInUrl_And_With2Strings_Or_Returns0_5()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(0.5d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWithNoValuePresentInUrl_Returns1_0()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key="), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, "");

        // Act
        var result = new RequestMatchResult();
        var score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWith3ValuesPresentInUrl_And_With1ExactStringWith2Patterns_Returns0_66()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2,test3"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new IStringMatcher[] { new ExactMatcher("test1", "test2") });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsCloseTo(0.66d, 0.1d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWith2ValuesPresentInUrl_And_With1ExactStringWith3Patterns_Returns0_66()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new IStringMatcher[] { new ExactMatcher("test1", "test2", "test3") });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsCloseTo(0.66d, 0.1d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWith2ValuesPresentInUrl_And_With2Strings_Returns1_0()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWith2ValuesPresentInUrl_And_With2ExactStringMatchers_Returns1_0()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new IStringMatcher[] { new ExactMatcher("test1"), new ExactMatcher("test2") });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuesPresentInUrl_MatchOnKeyWithValues_PartialMatch()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test0,test2"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(0.5d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresentInUrl_MatchOnKeyWithValues_Fails()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(0.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresentInUrl_MatchOnKey()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresentInUrl_MatchOnKeyWithEmptyArray()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new string[] { });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuePresentInUrl_MatchOnKey()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=frank@contoso.com"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    // Issue #849
    [Fact]
    public void RequestMessageParamMatcher_With1ParamContainingComma_Using_QueryParameterMultipleValueSupport_NoComma()
    {
        // Assign
        var options = new WireMockMiddlewareOptions
        {
            QueryParameterMultipleValueSupport = QueryParameterMultipleValueSupport.NoComma
        };
        var requestMessage = new RequestMessage(options, new UrlDetails("http://localhost?query=SELECT id, value FROM table WHERE id = 1&test=42"), "GET", "127.0.0.1");
        var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "query", false, "SELECT id, value FROM table WHERE id = 1");

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(1.0);
    }
}