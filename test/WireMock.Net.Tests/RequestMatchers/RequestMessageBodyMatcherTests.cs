// Copyright © WireMock.Net

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageBodyMatcherTests
{
    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_IStringMatcher()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "b",
            DetectedBodyType = BodyType.String
        };
        var stringMatcherMock = new Mock<IStringMatcher>();
        stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), 1d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1d);

        // Verify
        stringMatcherMock.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock.Verify(m => m.IsMatch("b"), Times.Once);
    }

    [Theory]
    [InlineData(1d, 1d, 1d)]
    [InlineData(0d, 1d, 1d)]
    [InlineData(1d, 0d, 1d)]
    [InlineData(0d, 0d, 0d)]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_IStringMatchers_Or(double one, double two, double expected)
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

        var matcher = new RequestMessageBodyMatcher(MatchOperator.Or, matchers.Cast<IMatcher>().ToArray());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(expected);

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
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_IStringMatchers_And(double one, double two, double expected)
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

        var matcher = new RequestMessageBodyMatcher(MatchOperator.And, matchers.Cast<IMatcher>().ToArray());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(expected);

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
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_IStringMatchers_Average(double one, double two, double expected)
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

        var matcher = new RequestMessageBodyMatcher(MatchOperator.Average, matchers.Cast<IMatcher>().ToArray());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(expected);

        // Verify
        stringMatcherMock1.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock1.Verify(m => m.IsMatch("b"), Times.Once);

        stringMatcherMock2.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock2.Verify(m => m.IsMatch("b"), Times.Once);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsBytes_IStringMatcher()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = [1],
            DetectedBodyType = BodyType.Bytes
        };
        var stringMatcherMock = new Mock<IStringMatcher>();
        stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), 0.5d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(0.0d);

        // Verify
        stringMatcherMock.Verify(m => m.GetPatterns(), Times.Never);
        stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_IStringMatcher()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new { value = 42 },
            DetectedBodyType = BodyType.Json
        };
        var stringMatcherMock = new Mock<IStringMatcher>();
        stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), 1.0d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);

        // Verify
        stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_and_BodyAsString_IStringMatcher()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new { value = 42 },
            BodyAsString = "orig",
            DetectedBodyType = BodyType.Json
        };
        var stringMatcherMock = new Mock<IStringMatcher>();
        stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(MatchResult.From(nameof(IStringMatcher), 1d));
        stringMatcherMock.SetupGet(m => m.MatchOperator).Returns(MatchOperator.Or);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1d);

        // Verify
        stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_IObjectMatcher()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = 42,
            DetectedBodyType = BodyType.Json
        };
        var objectMatcherMock = new Mock<IObjectMatcher>();
        objectMatcherMock.Setup(m => m.IsMatch(It.IsAny<object>())).Returns(MatchResult.From(nameof(IStringMatcher), 1d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(objectMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1d);

        // Verify
        objectMatcherMock.Verify(m => m.IsMatch(42), Times.Once);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_CSharpCodeMatcher()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new { value = 42 },
            DetectedBodyType = BodyType.Json
        };

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(new CSharpCodeMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, "return it.value == 42;"));

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_JmesPathMatchers()
    {
        // Arrange
        var body = new BodyData
        {
            BodyAsJson = new { requestId = "1", value = "A" },
            DetectedBodyType = BodyType.Json
        };

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var jmesMatcher1 = new JmesPathMatcher("requestId == '1'");
        var jmesMatcher2 = new JmesPathMatcher("value == 'A'");

        var bodyMatcher = new RequestMessageBodyMatcher(MatchOperator.And, jmesMatcher1, jmesMatcher2);

        // Act
        var result = new RequestMatchResult();
        double score = bodyMatcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData(new byte[0], 0.0)]
    [InlineData(new byte[] { 48 }, 1.0)]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsBytes_NotNullOrEmptyObjectMatcher(byte[] bytes, double expected)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = bytes,
            DetectedBodyType = BodyType.Bytes
        };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(new NotNullOrEmptyMatcher());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData("", 0.0)]
    [InlineData("x", 1.0)]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_NotNullOrEmptyObjectMatcher(string data, double expected)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = data,
            DetectedBodyType = BodyType.String
        };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(new NotNullOrEmptyMatcher());

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(expected);
    }

    [Theory]
    [InlineData(new byte[] { 1 })]
    [InlineData(new byte[] { 48 })]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsBytes_IObjectMatcher(byte[] bytes)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = bytes,
            DetectedBodyType = BodyType.Bytes
        };
        var objectMatcherMock = new Mock<IObjectMatcher>();
        objectMatcherMock.Setup(m => m.IsMatch(It.IsAny<object>())).Returns(MatchResult.From(nameof(IStringMatcher), 1.0d));

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(objectMatcherMock.Object);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);

        // Verify
        objectMatcherMock.Verify(m => m.IsMatch(It.IsAny<byte[]>()), Times.Once);
    }

    [Theory]
    [InlineData(new byte[] { 1 })]
    [InlineData(new byte[] { 48 })]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyTypeBytes_BodyAsBytes_ExactObjectMapper(byte[] bytes)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = bytes,
            DetectedBodyType = BodyType.Bytes
        };
        var exactObjectMapper = new ExactObjectMatcher(bytes);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(exactObjectMapper);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyTypeString_BodyAsBytes_ExactObjectMapper()
    {
        // Assign
        var bytes = Encoding.UTF8.GetBytes("hello");
        var body = new BodyData
        {
            BodyAsBytes = bytes,
            DetectedBodyType = BodyType.String
        };
        var exactObjectMapper = new ExactObjectMatcher(bytes);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(exactObjectMapper);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyTypeJson_BodyAsBytes_ExactObjectMapper()
    {
        // Assign
        var bytes = Encoding.UTF8.GetBytes("""{"value":42}""");
        var body = new BodyData
        {
            BodyAsBytes = bytes,
            DetectedBodyType = BodyType.Json
        };
        var exactObjectMapper = new ExactObjectMatcher(bytes);

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

        var matcher = new RequestMessageBodyMatcher(exactObjectMapper);

        // Act
        var result = new RequestMatchResult();
        double score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        Check.That(score).IsEqualTo(1.0d);
    }

    [Theory]
    [MemberData(nameof(MatchingScoreData))]
    public async Task RequestMessageBodyMatcher_GetMatchingScore_Funcs_Matching(object body, RequestMessageBodyMatcher matcher, bool shouldMatch)
    {
        // assign
        BodyData bodyData;
        if (body is byte[] b)
        {
            var bodyParserSettings = new BodyParserSettings
            {
                Stream = new MemoryStream(b),
                ContentType = null,
                DeserializeJson = true
            };
            bodyData = await BodyParser.ParseAsync(bodyParserSettings);
        }
        else if (body is string s)
        {
            var bodyParserSettings = new BodyParserSettings
            {
                Stream = new MemoryStream(Encoding.UTF8.GetBytes(s)),
                ContentType = null,
                DeserializeJson = true
            };
            bodyData = await BodyParser.ParseAsync(bodyParserSettings);
        }
        else
        {
            throw new Exception();
        }

        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", bodyData);

        // act
        var result = new RequestMatchResult();
        var score = matcher.GetMatchingScore(requestMessage, result);

        // assert
        Check.That(score).IsEqualTo(shouldMatch ? 1d : 0d);
    }

    public static TheoryData<object, RequestMessageBodyMatcher, bool> MatchingScoreData
    {
        get
        {
            var json = "{'a':'b'}";
            var str = "HelloWorld";
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00 };

            return new TheoryData<object, RequestMessageBodyMatcher, bool>
            {
                // JSON match +++
                {json, new RequestMessageBodyMatcher((object? o) => ((dynamic) o!).a == "b"), true},
                {json, new RequestMessageBodyMatcher((string? s) => s == json), true},
                {json, new RequestMessageBodyMatcher((byte[]? b) => b?.SequenceEqual(Encoding.UTF8.GetBytes(json)) == true), true},

                // JSON no match ---
                {json, new RequestMessageBodyMatcher((object? o) => false), false},
                {json, new RequestMessageBodyMatcher((string? s) => false), false},
                {json, new RequestMessageBodyMatcher((byte[]? b) => false), false},
                {json, new RequestMessageBodyMatcher(), false },

                // string match +++
                {str, new RequestMessageBodyMatcher((object? o) => o == null), true},
                {str, new RequestMessageBodyMatcher((string? s) => s == str), true},
                {str, new RequestMessageBodyMatcher((byte[]? b) => b?.SequenceEqual(Encoding.UTF8.GetBytes(str)) == true), true},

                // string no match ---
                {str, new RequestMessageBodyMatcher((object? o) => false), false},
                {str, new RequestMessageBodyMatcher((string? s) => false), false},
                {str, new RequestMessageBodyMatcher((byte[]? b) => false), false},
                {str, new RequestMessageBodyMatcher(), false },

                // binary match +++
                {bytes, new RequestMessageBodyMatcher((object? o) => o == null), true},
                {bytes, new RequestMessageBodyMatcher((string? s) => s == null), true},
                {bytes, new RequestMessageBodyMatcher((byte[]? b) => b?.SequenceEqual(bytes) == true), true},

                // binary no match ---
                {bytes, new RequestMessageBodyMatcher((object? o) => false), false},
                {bytes, new RequestMessageBodyMatcher((string? s) => false), false},
                {bytes, new RequestMessageBodyMatcher((byte[]? b) => false), false},
                {bytes, new RequestMessageBodyMatcher(), false }
            };
        }
    }
}