// Copyright © WireMock.Net

using AwesomeAssertions;
using System.Net;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

/// <summary>
/// Based on https://raw.githubusercontent.com/tmenier/Flurl/129565361e135e639f1d44a35a78aea4302ac6ca/Test/Flurl.Test/Http/HttpStatusRangeParserTests.cs 
/// </summary>
public class HttpStatusRangeParserTests
{
    [Theory]
    [InlineData("4**", 399, false)]
    [InlineData("4**", 400, true)]
    [InlineData("4**", 499, true)]
    [InlineData("4**", 500, false)]

    [InlineData("4xx", 399, false)]
    [InlineData("4xx", 400, true)]
    [InlineData("4xx", 499, true)]
    [InlineData("4xx", 500, false)]

    [InlineData("4XX", 399, false)]
    [InlineData("4XX", 400, true)]
    [InlineData("4XX", 499, true)]
    [InlineData("4XX", 500, false)]

    [InlineData("400-499", 399, false)]
    [InlineData("400-499", 400, true)]
    [InlineData("400-499", 499, true)]
    [InlineData("400-499", 500, false)]

    [InlineData("100,3xx,600", 100, true)]
    [InlineData("100,3xx,600", 101, false)]
    [InlineData("100,3xx,600", 300, true)]
    [InlineData("100,3xx,600", 399, true)]
    [InlineData("100,3xx,600", 400, false)]
    [InlineData("100,3xx,600", 600, true)]

    [InlineData("400-409,490-499", 399, false)]
    [InlineData("400-409,490-499", 405, true)]
    [InlineData("400-409,490-499", 450, false)]
    [InlineData("400-409,490-499", 495, true)]
    [InlineData("400-409,490-499", 500, false)]

    [InlineData("*", 0, true)]
    [InlineData(",,,*", 9999, true)]

    [InlineData("", 0, false)]
    [InlineData(",,,", 9999, false)]

    [InlineData(null, 399, true)]
    public void HttpStatusRangeParser_ValidPattern_IsMatch(string pattern, int value, bool expectedResult)
    {
        HttpStatusRangeParser.IsMatch(pattern, value).Should().Be(expectedResult);
    }

    [Fact]
    public void HttpStatusRangeParser_ValidPattern_HttpStatusCode_IsMatch()
    {
        HttpStatusRangeParser.IsMatch("4xx", HttpStatusCode.BadRequest).Should().BeTrue();
    }

    [Theory]
    [InlineData("-100")]
    [InlineData("100-")]
    [InlineData("1yy")]
    public void HttpStatusRangeParser_InvalidPattern_ThrowsException(string pattern)
    {
        Assert.Throws<ArgumentException>(() => HttpStatusRangeParser.IsMatch(pattern, 100));
    }
}