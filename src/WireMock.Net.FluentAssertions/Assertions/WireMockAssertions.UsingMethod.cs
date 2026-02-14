// Copyright © WireMock.Net

#pragma warning disable CS1591
using WireMock.Constants;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingConnect(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.CONNECT, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingDelete(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.DELETE, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingGet(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.GET, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingHead(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.HEAD, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingOptions(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.OPTIONS, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingPost(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.POST, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingPatch(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.PATCH, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingPut(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.PUT, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingTrace(string because = "", params object[] becauseArgs)
        => UsingMethod(HttpRequestMethod.TRACE, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingAnyMethod(string because = "", params object[] becauseArgs)
        => UsingMethod(Any, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> UsingMethod(string method, string because = "", params object[] becauseArgs)
    {
        var any = method == Any;
        Func<IRequestMessage, bool> predicate = request => (any && !string.IsNullOrEmpty(request.Method)) ||
                                                           string.Equals(request.Method, method, StringComparison.OrdinalIgnoreCase);

        var (filter, condition) = BuildFilterAndCondition(predicate);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called using method {0}{reason}, but no calls were made.",
                method
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called using method {0}{reason}, but didn't find it among the methods {1}.",
                _ => method,
                requests => requests.Select(request => request.Method)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }
}