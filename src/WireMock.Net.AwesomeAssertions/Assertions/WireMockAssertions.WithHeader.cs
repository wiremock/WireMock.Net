// Copyright © WireMock.Net

#pragma warning disable CS1591

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> WitHeaderKey(string expectedKey, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            return request.Headers?.Any(h => h.Key == expectedKey) == true;
        });

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0}{reason}.",
                expectedKey
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0}{reason}, but didn't find it among the calls with Header(s) {1}.",
                _ => expectedKey,
                requests => requests.Select(request => request.Headers)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, string>(this, expectedKey);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string value, string because = "", params object[] becauseArgs)
        => WithHeader(expectedKey, new[] { value }, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithHeader(string expectedKey, string[] expectedValues, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            var headers = request.Headers?.ToArray() ?? [];

            var matchingHeaderValues = headers.Where(h => h.Key == expectedKey).SelectMany(h => h.Value.ToArray()).ToArray();

            if (expectedValues.Length == 1 && matchingHeaderValues.Length == 1)
            {
                return matchingHeaderValues[0] == expectedValues[0];
            }

            var trimmedHeaderValues = string.Join(",", matchingHeaderValues.Select(x => x)).Split(',').Select(x => x.Trim()).ToArray();
            return expectedValues.Any(trimmedHeaderValues.Contains);
        });

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0} and Values {1}{reason}.",
                expectedKey,
                expectedValues
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called with Header {0} and Values {1}{reason}, but didn't find it among the calls with Header(s) {2}.",
                _ => expectedKey,
                _ => expectedValues,
                requests => requests.Select(request => request.Headers)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithoutHeaderKey(string unexpectedKey, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            return request.Headers?.Any(h => h.Key == unexpectedKey) != true;
        });

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0}{reason}.",
                unexpectedKey
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0}{reason}, but found it among the calls with Header(s) {1}.",
                _ => unexpectedKey,
                requests => requests.Select(request => request.Headers)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithoutHeader(string unexpectedKey, string value, string because = "", params object[] becauseArgs)
        => WithoutHeader(unexpectedKey, new[] { value }, because, becauseArgs);

    [CustomAssertion]
    public AndConstraint<WireMockAssertions> WithoutHeader(string unexpectedKey, string[] expectedValues, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request =>
        {
            var headers = request.Headers?.ToArray() ?? [];

            var matchingHeaderValues = headers.Where(h => h.Key == unexpectedKey).SelectMany(h => h.Value.ToArray()).ToArray();

            if (expectedValues.Length == 1 && matchingHeaderValues.Length == 1)
            {
                return matchingHeaderValues[0] != expectedValues[0];
            }

            var trimmedHeaderValues = string.Join(",", matchingHeaderValues.Select(x => x)).Split(',').Select(x => x.Trim()).ToArray();
            return !expectedValues.Any(trimmedHeaderValues.Contains);
        });

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0} and Values {1}{reason}.",
                unexpectedKey,
                expectedValues
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} not to have been called with Header {0} and Values {1}{reason}, but found it among the calls with Header(s) {2}.",
                _ => unexpectedKey,
                _ => expectedValues,
                requests => requests.Select(request => request.Headers)
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAssertions>(this);
    }
}