// Copyright © WireMock.Net

using System.Collections.Concurrent;
using FluentAssertions;
using Moq;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Services;

namespace WireMock.Net.Tests.Owin;

public class MappingMatcherTests
{
    private readonly Mock<IWireMockMiddlewareOptions> _optionsMock;
    private readonly Mock<IRandomizerDoubleBetween0And1> _randomizerDoubleBetween0And1Mock;

    private readonly MappingMatcher _sut;

    public MappingMatcherTests()
    {
        _optionsMock = new Mock<IWireMockMiddlewareOptions>();
        _optionsMock.SetupAllProperties();
        _optionsMock.Setup(o => o.Mappings).Returns(new ConcurrentDictionary<Guid, IMapping>());
        _optionsMock.Setup(o => o.LogEntries).Returns([]);
        _optionsMock.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());

        var loggerMock = new Mock<IWireMockLogger>();
        loggerMock.SetupAllProperties();
        loggerMock.Setup(l => l.Error(It.IsAny<string>()));
        _optionsMock.Setup(o => o.Logger).Returns(loggerMock.Object);

        _randomizerDoubleBetween0And1Mock = new Mock<IRandomizerDoubleBetween0And1>();
        _randomizerDoubleBetween0And1Mock.Setup(r => r.Generate()).Returns(0.5);

        _sut = new MappingMatcher(_optionsMock.Object, _randomizerDoubleBetween0And1Mock.Object);
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenNoMappingsDefined_ShouldReturnNull()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().BeNull();
        result.Partial.Should().BeNull();
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenMappingThrowsException_ShouldReturnNull()
    {
        // Assign
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.GetRequestMatchResult(It.IsAny<RequestMessage>(), It.IsAny<string>())).Throws<Exception>();

        var mappings = new ConcurrentDictionary<Guid, IMapping>();
        mappings.TryAdd(Guid.NewGuid(), mappingMock.Object);

        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().BeNull();
        result.Partial.Should().BeNull();
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenAllowPartialMappingIsFalse_ShouldReturnExactMatch()
    {
        // Assign
        var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var mappings = InitMappings
        (
            (guid1, [0.1], null),
            (guid2, [1.0], null)
        );
        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().NotBeNull();
        result.Match!.Mapping.Guid.Should().Be(guid2);
        result.Match.RequestMatchResult.AverageTotalScore.Should().Be(1.0);

        result.Partial.Should().NotBeNull();
        result.Partial!.Mapping.Guid.Should().Be(guid2);
        result.Partial.RequestMatchResult.AverageTotalScore.Should().Be(1.0);
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenAllowPartialMappingIsFalse_AndNoExactMatch_ShouldReturnNullExactMatch_And_PartialMatch()
    {
        // Assign
        var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var mappings = InitMappings
        (
            (guid1, [0.1], null),
            (guid2, [0.9], null)
        );
        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().BeNull();

        result.Partial.Should().NotBeNull();
        result.Partial!.Mapping.Guid.Should().Be(guid2);
        result.Partial.RequestMatchResult.AverageTotalScore.Should().Be(0.9);
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenAllowPartialMappingIsTrue_ShouldReturnAnyMatch()
    {
        // Assign
        var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");

        _optionsMock.SetupGet(o => o.AllowPartialMapping).Returns(true);
        var mappings = InitMappings(
            (guid1, [0.1], null),
            (guid2, [0.9], null)
        );
        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().NotBeNull();
        result.Match!.Mapping.Guid.Should().Be(guid2);
        result.Match.RequestMatchResult.AverageTotalScore.Should().Be(0.9);

        result.Partial.Should().NotBeNull();
        result.Partial!.Mapping.Guid.Should().Be(guid2);
        result.Partial.RequestMatchResult.AverageTotalScore.Should().Be(0.9);
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenAllowPartialMappingIsFalse_And_WithSameAverageScoreButMoreMatchers_ReturnsMatchWithMoreMatchers()
    {
        // Assign
        var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var mappings = InitMappings(
            (guid1, [1.0], null),
            (guid2, [1.0, 1.0], null)
        );
        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert and Verify
        result.Match.Should().NotBeNull();
        result.Match!.Mapping.Guid.Should().Be(guid2);
        result.Match.RequestMatchResult.AverageTotalScore.Should().Be(1.0);

        result.Partial.Should().NotBeNull();
        result.Partial!.Mapping.Guid.Should().Be(guid2);
        result.Partial.RequestMatchResult.AverageTotalScore.Should().Be(1.0);
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenProbabilityDoesNotMatch_ShouldReturnNormalMatch()
    {
        // Assign
        var withProbability = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var noProbability = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var mappings = InitMappings
        (
            (withProbability, [1.0], 0.4),
            (noProbability, [1.0], null)
        );
        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().NotBeNull();
        result.Match!.Mapping.Guid.Should().Be(noProbability);
    }

    [Fact]
    public void MappingMatcher_FindBestMatch_WhenProbabilityDoesMatch_ShouldReturnProbabilityMatch()
    {
        // Assign
        var withProbability = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var noProbability = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var mappings = InitMappings
        (
            (withProbability, [1.0], 0.6),
            (noProbability, [1.0], null)
        );
        _optionsMock.Setup(o => o.Mappings).Returns(mappings);

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

        // Act
        var result = _sut.FindBestMatch(request);

        // Assert
        result.Match.Should().NotBeNull();
        result.Match!.Mapping.Guid.Should().Be(withProbability);
    }

    private static ConcurrentDictionary<Guid, IMapping> InitMappings(params (Guid guid, double[] scores, double? probability)[] matches)
    {
        var mappings = new ConcurrentDictionary<Guid, IMapping>();

        foreach (var match in matches)
        {
            var mappingMock = new Mock<IMapping>();
            mappingMock.SetupGet(m => m.Guid).Returns(match.guid);

            var requestMatchResult = new RequestMatchResult();
            foreach (var score in match.scores)
            {
                requestMatchResult.AddScore(typeof(object), score, null);
            }

            mappingMock.SetupGet(m => m.Probability).Returns(match.probability);

            mappingMock.Setup(m => m.GetRequestMatchResult(It.IsAny<RequestMessage>(), It.IsAny<string>())).Returns(requestMatchResult);

            mappings.TryAdd(match.guid, mappingMock.Object);
        }

        return mappings;
    }
}