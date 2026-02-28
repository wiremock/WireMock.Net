// Copyright © WireMock.Net

using Moq;
using WireMock.Matchers.Request;
using WireMock.Models;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageCompositeMatcherTests
{
    private class Helper : RequestMessageCompositeMatcher
    {
        public Helper(IEnumerable<IRequestMatcher> requestMatchers, CompositeMatcherType type = CompositeMatcherType.And) : base(requestMatchers, type)
        {
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
}