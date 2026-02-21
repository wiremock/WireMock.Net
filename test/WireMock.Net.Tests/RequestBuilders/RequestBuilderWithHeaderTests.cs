// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithHeaderTests
{
    [Fact]
    public void RequestBuilder_WithHeader_String_String_MatchBehaviour()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithHeader("h", "t", MatchBehaviour.AcceptOnMatch);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
    }

    [Fact]
    public void RequestBuilder_WithHeader_String_String_Bool_MatchBehaviour()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithHeader("h", "t", true, MatchBehaviour.AcceptOnMatch);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
    }

    [Fact]
    public void RequestBuilder_WithHeader_String_Strings_MatchBehaviour()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithHeader("h", new[] { "t1", "t2" }, MatchBehaviour.AcceptOnMatch);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
    }

    [Fact]
    public void RequestBuilder_WithHeader_String_Strings_Bool_MatchBehaviour()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithHeader("h", new[] { "t1", "t2" }, true, MatchBehaviour.AcceptOnMatch);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
    }

    [Fact]
    public void RequestBuilder_WithHeader_String_IStringMatcher()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithHeader("h", new ExactMatcher("v"));

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
    }

    [Fact]
    public void RequestBuilder_WithHeader_FuncIDictionary()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithHeader(x => true);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        Check.That(matchers.Count).IsEqualTo(1);
        Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
    }
}