// Copyright © WireMock.Net

using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.FluentAssertions;

namespace WireMock.Net.Tests.FluentAssertions;

public static class WireMockAssertionsExtensions
{
    [CustomAssertion]
    public static AndWhichConstraint<WireMockAssertions, string> AtAbsoluteUrl2(this WireMockAssertions assertions,
        string absoluteUrl, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = assertions.BuildFilterAndCondition(request => string.Equals(request.AbsoluteUrl, absoluteUrl, StringComparison.OrdinalIgnoreCase));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => assertions.RequestMessages)
            .ForCondition(requests => assertions.CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but no calls were made.",
                absoluteUrl
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => absoluteUrl,
                requests => requests.Select(request => request.AbsoluteUrl)
            );

        assertions.FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, string>(assertions, absoluteUrl);
    }
}