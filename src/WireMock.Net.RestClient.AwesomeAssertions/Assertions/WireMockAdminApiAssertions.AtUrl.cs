// Copyright © WireMock.Net

using WireMock.Extensions;
using WireMock.Matchers;

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

#pragma warning disable CS1591
public partial class WireMockAdminApiAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAdminApiAssertions, string> AtAbsoluteUrl(string absoluteUrl, string because = "", params object[] becauseArgs)
    {
        _ = AtAbsoluteUrl(new ExactMatcher(true, absoluteUrl), because, becauseArgs);

        return new AndWhichConstraint<WireMockAdminApiAssertions, string>(this, absoluteUrl);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAdminApiAssertions, IStringMatcher> AtAbsoluteUrl(IStringMatcher absoluteUrlMatcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => absoluteUrlMatcher.IsPerfectMatch(request.AbsoluteUrl));

        var absoluteUrl = absoluteUrlMatcher.GetPatterns().FirstOrDefault().GetPattern();

        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called at address matching the absolute url {0}{reason}, but no calls were made.",
                absoluteUrl
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called at address matching the absolute url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => absoluteUrl,
                requests => requests.Select(request => request.AbsoluteUrl)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAdminApiAssertions, IStringMatcher>(this, absoluteUrlMatcher);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAdminApiAssertions, string> AtUrl(string url, string because = "", params object[] becauseArgs)
    {
        _ = AtUrl(new ExactMatcher(true, url), because, becauseArgs);

        return new AndWhichConstraint<WireMockAdminApiAssertions, string>(this, url);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAdminApiAssertions, IStringMatcher> AtUrl(IStringMatcher urlMatcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => urlMatcher.IsPerfectMatch(request.Url));

        var url = urlMatcher.GetPatterns().FirstOrDefault().GetPattern();

        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called at address matching the url {0}{reason}, but no calls were made.",
                url
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called at address matching the url {0}{reason}, but didn't find it among the calls to {1}.",
                _ => url,
                requests => requests.Select(request => request.Url)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAdminApiAssertions, IStringMatcher>(this, urlMatcher);
    }
}
