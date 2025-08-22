// Copyright © WireMock.Net

#pragma warning disable CS1591
using System;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> FromClientIP(string clientIP, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => string.Equals(request.ClientIP, clientIP, StringComparison.OrdinalIgnoreCase));

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called from client IP {0}{reason}, but no calls were made.",
                clientIP
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called from client IP {0}{reason}, but didn't find it among the calls from IP(s) {1}.",
                _ => clientIP, requests => requests.Select(request => request.ClientIP)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, string>(this, clientIP);
    }
}