// Copyright © WireMock.Net

using System.Linq;
using Stef.Validation;

namespace WireMock.Matchers;

/// <summary>
/// ExactObjectMatcher
/// </summary>
/// <seealso cref="IObjectMatcher" />
public class ExactObjectMatcher : IObjectMatcher
{
    /// <inheritdoc />
    public object Value { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(object value) : this(MatchBehaviour.AcceptOnMatch, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(MatchBehaviour matchBehaviour, object value)
    {
        Value = Guard.NotNull(value);
        MatchBehaviour = matchBehaviour;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(byte[] value) : this(MatchBehaviour.AcceptOnMatch, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(MatchBehaviour matchBehaviour, byte[] value)
    {
        Value = Guard.NotNull(value);
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(object? input)
    {
        bool equals;
        if (Value is byte[] valueAsBytes && input is byte[] inputAsBytes)
        {
            equals = valueAsBytes.SequenceEqual(inputAsBytes);
        }
        else
        {
            equals = Equals(Value, input);
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(equals)));
    }

    /// <inheritdoc />
    public string Name => nameof(ExactObjectMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return "NotImplemented";
    }
}