// Copyright © WireMock.Net

namespace WireMock.Matchers.Request;

/// <summary>
/// Return the mismatch if the matching score of matchers is not perfect.
/// </summary>
internal sealed class RequestMessageEarlyMatcher(
    RequestMatcherType? earlyMatcherType,
    IEnumerable<IRequestMatcher> requestMatchers) : IRequestMatcher
{
    /// <inheritdoc />
    public RequestMatcherType Type => RequestMatcherType.Composite;

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        if (earlyMatcherType is null)
        {
            return MatchScores.Perfect;
        }

        var earlyMatchers = requestMatchers
            .Where(m => m.Type == earlyMatcherType)
            .ToList();

        if (earlyMatchers.Count is 0)
        {
            return MatchScores.Perfect;
        }

        var compositeMatcher = new RequestBuilders.Request(earlyMatchers);
        return compositeMatcher.GetMatchingScore(requestMessage, requestMatchResult);
    }
}