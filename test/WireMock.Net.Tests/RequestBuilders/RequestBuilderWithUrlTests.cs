// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithUrlTests
{
    [Fact]
    public void RequestBuilder_WithUrl_Strings()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithUrl("http://a", "http://b");

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageUrlMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithUrl_MatchBehaviour_Strings()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithUrl(MatchOperator.Or, "http://a", "http://b");

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageUrlMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithUrl_Funcs()
    {
        // Act
        var requestBuilder = (Request) Request.Create().WithUrl(url => true, url => false);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageUrlMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithUrl_IStringMatchers()
    {
        // Act
        var requestBuilder = (Request) Request.Create().WithUrl(new ExactMatcher("http://a"), new ExactMatcher("http://b"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageUrlMatcher>();
    }
}