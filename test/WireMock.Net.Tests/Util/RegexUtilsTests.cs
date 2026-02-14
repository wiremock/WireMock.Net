// Copyright © WireMock.Net

using System.Text.RegularExpressions;
using AwesomeAssertions;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class RegexUtilsTests
{
    [Fact]
    public void GetNamedGroups_ValidRegexWithNamedGroups_ReturnsNamedGroupsDictionary()
    {
        // Arrange
        var pattern = @"^(?<street>\w+)\s(?<number>\d+)$";
        var input = "MainStreet 123";
        var regex = new Regex(pattern);

        // Act
        var namedGroupsDictionary = RegexUtils.GetNamedGroups(regex, input);

        // Assert
        namedGroupsDictionary.Should().NotBeEmpty()
            .And.Contain(new KeyValuePair<string, string>("street", "MainStreet"))
            .And.Contain(new KeyValuePair<string, string>("number", "123"));
    }

    [Theory]
    [InlineData("", "test", false, false)]
    [InlineData(null, "test", false, false)]
    [InlineData(".*", "test", true, true)]
    [InlineData("invalid[", "test", false, false)]
    public void MatchRegex_WithVariousPatterns_ReturnsExpectedResults(string? pattern, string input, bool expectedIsValid, bool expectedResult)
    {
        // Act
        var (isValidResult, matchResult) = RegexUtils.MatchRegex(pattern, input);

        // Assert
        isValidResult.Should().Be(expectedIsValid);
        matchResult.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "test", false, false)]
    [InlineData(null, "test", false, false)]
    [InlineData(".*", "test", true, true)]
    [InlineData("invalid[", "test", false, false)]
    public void MatchRegex_WithVariousPatternsAndExtendedRegex_ReturnsExpectedResults(string? pattern, string input, bool expectedIsValid, bool expectedResult)
    {
        // Act
        var (isValidResult, matchResult) = RegexUtils.MatchRegex(pattern, input, useRegexExtended: true);

        // Assert
        isValidResult.Should().Be(expectedIsValid);
        matchResult.Should().Be(expectedResult);
    }
}