// Copyright © WireMock.Net


using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderUsingMethodTests
{
    [Fact]
    public void RequestBuilder_UsingConnect()
    {
        // Act
        var requestBuilder = (Request)Request.Create().UsingConnect();

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        ((matchers[0] as RequestMessageMethodMatcher).Methods).Should().ContainSingle("CONNECT");
    }

    [Fact]
    public void RequestBuilder_UsingOptions()
    {
        // Act
        var requestBuilder = (Request)Request.Create().UsingOptions();

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        ((matchers[0] as RequestMessageMethodMatcher).Methods).Should().ContainSingle("OPTIONS");
    }

    [Fact]
    public void RequestBuilder_UsingPatch()
    {
        // Act
        var requestBuilder = (Request)Request.Create().UsingPatch();

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        ((matchers[0] as RequestMessageMethodMatcher).Methods).Should().ContainSingle("PATCH");
    }

    [Fact]
    public void RequestBuilder_UsingTrace()
    {
        // Act
        var requestBuilder = (Request)Request.Create().UsingTrace();

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        ((matchers[0] as RequestMessageMethodMatcher).Methods).Should().ContainSingle("TRACE");
    }

    [Fact]
    public void RequestBuilder_UsingAnyMethod_ClearsAllOtherMatches()
    {
        // Assign
        var requestBuilder = (Request)Request.Create().UsingGet();

        // Assert 1
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(1);
        matchers[0].Should().BeOfType<RequestMessageMethodMatcher>();

        // Act
        requestBuilder.UsingAnyMethod();

        // Assert 2
        matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Count.Should().Be(0);
    }
}