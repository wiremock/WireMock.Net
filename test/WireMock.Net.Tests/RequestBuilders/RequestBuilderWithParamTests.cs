// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithParamTests
{
    [Fact]
    public void RequestBuilder_WithParam_String_MatchBehaviour()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", MatchBehaviour.AcceptOnMatch);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageParamMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithParam_String_Strings()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", new[] { "v1", "v2" });

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageParamMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithParam_String_IStringMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", new RegexMatcher("[012]"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageParamMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithParam_String_MatchBehaviour_IExactMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", MatchBehaviour.AcceptOnMatch, new ExactMatcher("v"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageParamMatcher>();
    }
}