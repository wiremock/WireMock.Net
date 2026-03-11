// Copyright © WireMock.Net

#pragma warning disable CS1591
// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

public partial class WireMockAdminApiAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAdminApiAssertions, string> FromClientIP(string clientIP, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.ClientIP, clientIP, StringComparison.OrdinalIgnoreCase));

        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called from client IP {0}{reason}, but no calls were made.",
                clientIP
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called from client IP {0}{reason}, but didn't find it among the calls from IP(s) {1}.",
                _ => clientIP, requests => requests.Select(request => request.ClientIP)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAdminApiAssertions, string>(this, clientIP);
    }
}
