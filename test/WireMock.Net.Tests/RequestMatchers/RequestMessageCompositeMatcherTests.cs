// Copyright © WireMock.Net

using Moq;
using WireMock.Constants;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageCompositeMatcherTests
{
    private class Helper : RequestMessageCompositeMatcher
    {
        public Helper(
            IEnumerable<IRequestMatcher> requestMatchers,
            CompositeMatcherType type = CompositeMatcherType.And,
            RequestMatcherType? earlyMatcherType = null) : base(requestMatchers, type)
        {
            EarlyMatcherType = earlyMatcherType;
        }
    }

    [Fact]
    public void RequestMessageCompositeMatcher_GetMatchingScore_EmptyArray()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1");
        var matcher = new Helper(Enumerable.Empty<IRequestMatcher>());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(0.0d);
    }

    [Fact]
    public void RequestMessageCompositeMatcher_GetMatchingScore_CompositeMatcherType_And()
    {
        // Assign
        var requestMatcher1Mock = new Mock<IRequestMatcher>();
        requestMatcher1Mock.Setup(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>())).Returns(1.0d);
        var requestMatcher2Mock = new Mock<IRequestMatcher>();
        requestMatcher2Mock.Setup(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>())).Returns(0.8d);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1");
        var matcher = new Helper(new[] { requestMatcher1Mock.Object, requestMatcher2Mock.Object });

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(0.9d);

        // Verify
        requestMatcher1Mock.Verify(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>()), Times.Once);
        requestMatcher2Mock.Verify(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>()), Times.Once);
    }

    [Fact]
    public void RequestMessageCompositeMatcher_GetMatchingScore_CompositeMatcherType_Or()
    {
        // Assign
        var requestMatcher1Mock = new Mock<IRequestMatcher>();
        requestMatcher1Mock.Setup(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>())).Returns(1.0d);
        var requestMatcher2Mock = new Mock<IRequestMatcher>();
        requestMatcher2Mock.Setup(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>())).Returns(0.8d);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1");
        var matcher = new Helper(new[] { requestMatcher1Mock.Object, requestMatcher2Mock.Object }, CompositeMatcherType.Or);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(1.0d);

        // Verify
        requestMatcher1Mock.Verify(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>()), Times.Once);
        requestMatcher2Mock.Verify(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>()), Times.Once);
    }

    [Fact]
    public void RequestMessageCompositeMatcher_GetMatchingScore_EarlyMismatch()
    {
        // Assign
        var requestMatcher1Mock = new Mock<IRequestMatcher>();
        requestMatcher1Mock.Setup(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>())).Returns(1.0d);
        var requestMatcher2Mock = new Mock<IRequestMatcher>();
        requestMatcher2Mock.Setup(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>())).Returns(0.8d);
        var postMatcher = new RequestMessageMethodMatcher(HttpRequestMethod.POST);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), HttpRequestMethod.GET, "127.0.0.1");
        var matcher = new Helper(
            [requestMatcher1Mock.Object, requestMatcher2Mock.Object, postMatcher],
            earlyMatcherType: RequestMatcherType.Method);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(0.0d);

        // Verify
        requestMatcher1Mock.Verify(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>()), Times.Never);
        requestMatcher2Mock.Verify(rm => rm.GetMatchingScore(It.IsAny<RequestMessage>(), It.IsAny<RequestMatchResult>()), Times.Never);
    }

    [Fact]
    public void RequestMessageCompositeMatcher_GetMatchingScore_SeveralHeadersEarlyMismatch()
    {
        // Assign
        var headers = new Dictionary<string, string[]>
        {
            { "teST", new[] { "x" } },
            { "teST2", new[] { "z" } }
        };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", null, headers);
        var request = Request.Create()
            .WithEarlyMismatch(RequestMatcherType.Header)
            .UsingAnyMethod()
            .WithHeader("teST", "x")
            .WithHeader("teST1", ["xx", "yy"])
            .WithHeader("teST2", ["y", "z"], matchOperator: MatchOperator.And);

        // Act
        var score = request.GetMatchingScore(requestMessage, new RequestMatchResult());

        // Assert
        score.Should().Be(0.0d);
    }

    [Fact]
    public void RequestMessageCompositeMatcher_GetMatchingScore_SeveralParamEarlyMismatchSuccess()
    {
        // Assign
        var uriWithParams = new Uri("http://localhost?test1=1&test2=2");
        var requestMessage = new RequestMessage(new UrlDetails(uriWithParams), "GET", "127.0.0.1");
        var request = Request.Create()
            .WithEarlyMismatch(RequestMatcherType.Param)
            .UsingAnyMethod()
            .WithParam("test1", "1")
            .WithParam("test2", "2");

        // Act
        var score = request.GetMatchingScore(requestMessage, new RequestMatchResult());

        // Assert
        score.Should().Be(1.0d);
    }
}