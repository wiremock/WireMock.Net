// Copyright © WireMock.Net

using System;

namespace WireMock.Matchers;

/// <summary>
/// Represents a matcher that combines multiple matching strategies into a single composite operation.
/// </summary>
public class CompositeMatcher : IMatcher
{
    /// <inheritdoc />
    public string Name => nameof(CompositeMatcher);

    /// <summary>
    /// The logical operator used to combine the results of the matchers.
    /// </summary>
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// All matchers.
    /// </summary>
    public IMatcher[] Matchers { get; }

    /// <summary>
    /// Initializes a new instance of the CompositeMatcher class with the specified matchers, operator, and match behaviour.
    /// </summary>
    /// <param name="matchers">An array of matchers to be combined. Cannot be null or contain null elements.</param>
    /// <param name="matchOperator">The logical operator used to combine the results of the matchers.</param>
    /// <param name="matchBehaviour">The behaviour that determines how the composite matcher interprets the combined results.</param>
    public CompositeMatcher(IMatcher[] matchers, MatchOperator matchOperator, MatchBehaviour matchBehaviour)
    {
        Matchers = matchers;
        MatchOperator = matchOperator;
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        throw new NotImplementedException();
    }
}
