// Copyright © WireMock.Net

#if MIMEKIT
using System.Collections.Generic;
using AwesomeAssertions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithMultiPartTests
{
    [Fact]
    public void RequestBuilder_WithMultiPart_MimePartMatcher()
    {
        // Arrange
        var matcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, null, null, null, null);

        // Act
        var requestBuilder = (Request)Request.Create().WithMultiPart(matcher);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);
        ((RequestMessageMultiPartMatcher)matchers[0]).Matchers.Should().HaveCount(1).And.ContainItemsAssignableTo<MimePartMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithMultiPart_MimePartMatchers()
    {
        // Arrange
        var matcher1 = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, null, null, null, null);
        var matcher2 = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, null, null, null, null);

        // Act
        var requestBuilder = (Request)Request.Create().WithMultiPart(MatchBehaviour.RejectOnMatch, matcher1, matcher2);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);

        var x = ((RequestMessageMultiPartMatcher)matchers[0]);
        x.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
        x.Matchers.Should().HaveCount(2).And.ContainItemsAssignableTo<MimePartMatcher>();
    }
}
#endif