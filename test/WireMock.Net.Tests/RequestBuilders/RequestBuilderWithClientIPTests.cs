// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithClientIPTests
{
    [Fact]
    public void Request_WithClientIP_Match_Ok()
    {
        // given
        var spec = Request.Create().WithClientIP("127.0.0.2", "1.1.1.1");

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.2");

        // then
        var requestMatchResult = new RequestMatchResult();
        spec.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
    }

    [Fact]
    public void Request_WithClientIP_Match_Fail()
    {
        // given
        var spec = Request.Create().WithClientIP("127.0.0.2");

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "192.1.1.1");

        // then
        var requestMatchResult = new RequestMatchResult();
        spec.GetMatchingScore(request, requestMatchResult).Should().Be(0.0);
    }

    [Fact]
    public void Request_WithClientIP_WildcardMatcher()
    {
        // given
        var spec = Request.Create().WithClientIP(new WildcardMatcher("127.0.0.2"));

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.2");

        // then
        var requestMatchResult = new RequestMatchResult();
        spec.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
    }

    [Fact]
    public void Request_WithClientIP_Func()
    {
        // given
        var spec = Request.Create().WithClientIP(c => c.Contains("."));

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.2");

        // then
        var requestMatchResult = new RequestMatchResult();
        spec.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
    }
}