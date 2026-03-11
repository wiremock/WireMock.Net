// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithCookieTests
{
    [Fact]
    public void RequestBuilder_WithCookie_String_String_Bool_MatchBehaviour()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithCookie("c", "t", true, MatchBehaviour.AcceptOnMatch);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageCookieMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithCookie_String_IStringMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithCookie("c", new ExactMatcher("v"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageCookieMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithCookie_FuncIDictionary()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithCookie((IDictionary<string, string> x) => true);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageCookieMatcher>();
    }
}