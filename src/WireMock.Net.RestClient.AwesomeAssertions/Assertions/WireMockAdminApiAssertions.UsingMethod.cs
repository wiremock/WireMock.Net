// Copyright © WireMock.Net

#pragma warning disable CS1591
using WireMock.Admin.Requests;
using WireMock.Constants;

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

public partial class WireMockAdminApiAssertions
{
    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingConnect(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.CONNECT, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingDelete(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.DELETE, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingGet(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.GET, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingHead(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.HEAD, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingOptions(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.OPTIONS, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingPost(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.POST, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingPatch(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.PATCH, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingPut(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.PUT, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingTrace(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.TRACE, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingAnyMethod(string because = "", params object[] becauseArgs)
        => UsingMethod(Any, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> UsingMethod(string method, string because = "", params object[] becauseArgs)
    {
        var any = method == Any;
        Func<LogRequestModel, bool> predicate = request => (any && !string.IsNullOrEmpty(request.Method)) ||
                                                           string.Equals(request.Method, method, StringComparison.OrdinalIgnoreCase);

        var (filter, condition) = BuildFilterAndCondition(predicate);

        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called using method {0}{reason}, but no calls were made.",
                method
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockadminapi} to have been called using method {0}{reason}, but didn't find it among the methods {1}.",
                _ => method,
                requests => requests.Select(request => request.Method)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAdminApiAssertions>(this);
    }
}
