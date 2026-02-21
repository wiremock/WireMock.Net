// Copyright © WireMock.Net

using NFluent;
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
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
    }

    [Fact]
    public void RequestBuilder_WithParam_String_Strings()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", new[] { "v1", "v2" });

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
    }

    [Fact]
    public void RequestBuilder_WithParam_String_IStringMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", new RegexMatcher("[012]"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
    }

    [Fact]
    public void RequestBuilder_WithParam_String_MatchBehaviour_IExactMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithParam("p", MatchBehaviour.AcceptOnMatch, new ExactMatcher("v"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
    }
}