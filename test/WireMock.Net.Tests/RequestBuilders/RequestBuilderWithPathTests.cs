// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithPathTests
{
    private const string ClientIp = "::1";

    [Fact]
    public void Request_WithPath_Spaces()
    {
        // Assign
        var spec = Request.Create().WithPath("/path/a b").UsingAnyMethod();

        // when
        var body = new BodyData();
        var request = new RequestMessage(new UrlDetails("http://localhost/path/a b"), "GET", ClientIp, body);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_WithHeader_Match()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingAnyMethod().WithHeader("X-toto", "tata");

        // when
        var body = new BodyData
        {
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, new Dictionary<string, string[]> { { "X-toto", new[] { "tata" } } });

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath()
    {
        // given
        var spec = Request.Create().WithPath("/foo");

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPaths()
    {
        var requestBuilder = Request.Create().WithPath("/x1", "/x2");

        var request1 = new RequestMessage(new UrlDetails("http://localhost/x1"), "blabla", ClientIp);
        var request2 = new RequestMessage(new UrlDetails("http://localhost/x2"), "blabla", ClientIp);

        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request1, requestMatchResult)).IsEqualTo(1.0);
        Check.That(requestBuilder.GetMatchingScore(request2, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPathFunc()
    {
        // given
        var spec = Request.Create().WithPath(url => url.EndsWith("/foo"));

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_RegexMatcher_HasMatch()
    {
        // given
        var spec = Request.Create().WithPath(new RegexMatcher("^/foo"));

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo/bar"), "blabla", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPathRegexMatcher_HasNoMatch()
    {
        // given
        var spec = Request.Create().WithPath("/foo");

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/bar"), "blabla", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_RegexMatcher_WithPatternAsFile_HasMatch()
    {
        // Arrange
        var pattern = new StringPattern
        {
            Pattern = "^/foo",
            PatternAsFile = "c:\\x.txt"
        };
        var spec = Request.Create().WithPath(new RegexMatcher(pattern));

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo/bar"), "blabla", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_specify_requests_matching_given_path_and_method_delete()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingDelete();

        // when
        var body = new BodyData
        {
            BodyAsString = "whatever"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "Delete", ClientIp, body);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_specify_requests_matching_given_path_and_method_get()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingGet();

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_specify_requests_matching_given_path_and_method_head()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingHead();

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "HEAD", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_specify_requests_matching_given_path_and_method_post()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingPost();

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_specify_requests_matching_given_path_and_method_put()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingPut();

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_specify_requests_matching_given_path_and_method_patch()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingPatch();

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PATCH", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithPath_Should_exclude_requests_matching_given_path_but_not_http_method()
    {
        // given
        var spec = Request.Create().WithPath("/foo").UsingPut();

        // when
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "HEAD", ClientIp);

        // then
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
    }
}