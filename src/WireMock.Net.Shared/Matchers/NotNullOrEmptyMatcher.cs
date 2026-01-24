// Copyright © WireMock.Net

using System.Linq;
using AnyOfTypes;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// NotNullOrEmptyMatcher
/// </summary>
/// <seealso cref="IObjectMatcher" />
public class NotNullOrEmptyMatcher : IObjectMatcher, IStringMatcher
{
    /// <inheritdoc />
    public string Name => nameof(NotNullOrEmptyMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotNullOrEmptyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    public NotNullOrEmptyMatcher(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        MatchBehaviour = matchBehaviour;
        Value = string.Empty;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(object? input)
    {
        bool match;

        switch (input)
        {
            case string @string:
                match = !string.IsNullOrEmpty(@string);
                break;

            case byte[] bytes:
                match = bytes.Any();
                break;

            default:
                match = input != null;
                break;
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match)));
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        var match = !string.IsNullOrEmpty(input);

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match)));
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return [];
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator => MatchOperator.Or;

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}" +
               $")";
    }
}