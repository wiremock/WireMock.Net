// Copyright © WireMock.Net

#pragma warning disable CS1591
// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

public partial class WireMockAdminApiAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAdminApiAssertions, string> WithProxyUrl(string proxyUrl, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.ProxyUrl, proxyUrl, StringComparison.OrdinalIgnoreCase));

        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called with proxy url {0}{reason}, but no calls were made.",
                proxyUrl
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called with proxy url {0}{reason}, but didn't find it among the calls with {1}.",
                _ => proxyUrl,
                requests => requests.Select(request => request.ProxyUrl)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAdminApiAssertions, string>(this, proxyUrl);
    }
}
