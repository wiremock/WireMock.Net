// Copyright © WireMock.Net

using Moq;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageGraphQLMatcherTests
{
    [Fact]
    public void RequestMessageGraphQLMatcher_GetMatchingScore_BodyAsString_IStringMatcher()
    {
        // Arrange
        var body = new BodyData
        {
            BodyAsString = "b",
            DetectedBodyType = BodyType.String
        };
        var stringMatcherMock = new Mock<IStringMatcher>();
        stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), 1d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageGraphQLMatcher(stringMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(MatchScores.Perfect);

        // Verify
        stringMatcherMock.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock.Verify(m => m.IsMatch("b"), Times.Once);
    }

    [Theory]
    [InlineData(1d, 1d, 1d)]
    [InlineData(0d, 1d, 1d)]
    [InlineData(1d, 0d, 1d)]
    [InlineData(0d, 0d, 0d)]
    public void RequestMessageGraphQLMatcher_GetMatchingScore_BodyAsString_IStringMatchers_Or(double one, double two, double expected)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "b",
            DetectedBodyType = BodyType.String
        };
        var stringMatcherMock1 = new Mock<IStringMatcher>();
        stringMatcherMock1.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), one));

        var stringMatcherMock2 = new Mock<IStringMatcher>();
        stringMatcherMock2.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), two));

        var matchers = new[] { stringMatcherMock1.Object, stringMatcherMock2.Object };

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageGraphQLMatcher(MatchOperator.Or, matchers.Cast<IMatcher>().ToArray());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);

        // Verify
        stringMatcherMock1.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock1.Verify(m => m.IsMatch("b"), Times.Once);

        stringMatcherMock2.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock2.Verify(m => m.IsMatch("b"), Times.Once);
        stringMatcherMock2.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(1d, 1d, 1d)]
    [InlineData(0d, 1d, 0d)]
    [InlineData(1d, 0d, 0d)]
    [InlineData(0d, 0d, 0d)]
    public void RequestMessageGraphQLMatcher_GetMatchingScore_BodyAsString_IStringMatchers_And(double one, double two, double expected)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "b",
            DetectedBodyType = BodyType.String
        };
        var stringMatcherMock1 = new Mock<IStringMatcher>();
        stringMatcherMock1.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), one));

        var stringMatcherMock2 = new Mock<IStringMatcher>();
        stringMatcherMock2.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), two));

        var matchers = new[] { stringMatcherMock1.Object, stringMatcherMock2.Object };

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageGraphQLMatcher(MatchOperator.And, matchers.Cast<IMatcher>().ToArray());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);

        // Verify
        stringMatcherMock1.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock1.Verify(m => m.IsMatch("b"), Times.Once);

        stringMatcherMock2.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock2.Verify(m => m.IsMatch("b"), Times.Once);
        stringMatcherMock2.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(1d, 1d, 1d)]
    [InlineData(0d, 1d, 0.5d)]
    [InlineData(1d, 0d, 0.5d)]
    [InlineData(0d, 0d, 0d)]
    public void RequestMessageGraphQLMatcher_GetMatchingScore_BodyAsString_IStringMatchers_Average(double one, double two, double expected)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "b",
            DetectedBodyType = BodyType.String
        };
        var stringMatcherMock1 = new Mock<IStringMatcher>();
        stringMatcherMock1.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), one));

        var stringMatcherMock2 = new Mock<IStringMatcher>();
        stringMatcherMock2.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), two));

        var matchers = new[] { stringMatcherMock1.Object, stringMatcherMock2.Object };

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageGraphQLMatcher(MatchOperator.Average, matchers.Cast<IMatcher>().ToArray());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);

        // Verify
        stringMatcherMock1.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock1.Verify(m => m.IsMatch("b"), Times.Once);

        stringMatcherMock2.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock2.Verify(m => m.IsMatch("b"), Times.Once);
    }

    [Fact]
    public void RequestMessageGraphQLMatcher_GetMatchingScore_BodyAsBytes_IStringMatcher_ReturnMisMatch()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = new byte[] { 1 },
            DetectedBodyType = BodyType.Bytes
        };
        var stringMatcherMock = new Mock<IStringMatcher>();
        stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), 0.5d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageGraphQLMatcher(stringMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(MatchScores.Mismatch);

        // Verify
        stringMatcherMock.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Never);
    }
}