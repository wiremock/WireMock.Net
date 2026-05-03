// Copyright © WireMock.Net

namespace WireMock.Matchers.Request;

/// <summary>
/// The scenario and state matcher.
/// </summary>
internal class RequestMessageScenarioAndStateMatcher : IRequestMatcher
{
    /// <summary>
    /// Execution state condition for the current mapping.
    /// </summary>
    private readonly string? _executionConditionState;

    /// <summary>
    /// The next state which will be signaled after the current mapping execution.
    /// In case the value is null state will not be changed.
    /// </summary>
    private readonly string? _nextState;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageScenarioAndStateMatcher"/> class.
    /// </summary>
    /// <param name="nextState">The next state.</param>
    /// <param name="executionConditionState">Execution state condition for the current mapping.</param>
    public RequestMessageScenarioAndStateMatcher(string? nextState, string? executionConditionState)
    {
        _nextState = nextState;
        _executionConditionState = executionConditionState;
    }

    /// <inheritdoc />
    public RequestMatcherType Type => RequestMatcherType.ScenarioAndState;

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        return requestMatchResult.AddScore(GetType(), GetScore(), null);
    }

    private double GetScore()
    {
        return Equals(_executionConditionState, _nextState) ? MatchScores.Perfect : MatchScores.Mismatch;
    }
}