// Copyright © WireMock.Net

using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// ExactMatcher
/// </summary>
/// <seealso cref="IStringMatcher" /> and <seealso cref="IIgnoreCaseMatcher" />
public class ExactMatcher : IStringMatcher, IIgnoreCaseMatcher
{
    private readonly AnyOf<string, StringPattern>[] _values;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param> 
    /// <param name="value">The string value.</param>
    public ExactMatcher(MatchBehaviour matchBehaviour, string value) : this(matchBehaviour, true, MatchOperator.Or, new AnyOf<string, StringPattern>(value))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ExactMatcher(params AnyOf<string, StringPattern>[] values) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, values)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
    /// </summary>
    /// <param name="ignoreCase">Ignore the case from the pattern(s).</param>
    /// <param name="values">The values.</param>
    public ExactMatcher(bool ignoreCase, params AnyOf<string, StringPattern>[] values) : this(MatchBehaviour.AcceptOnMatch, ignoreCase, MatchOperator.Or, values)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern(s).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="values">The values.</param>
    public ExactMatcher(
        MatchBehaviour matchBehaviour,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or,
        params AnyOf<string, StringPattern>[] values)
    {
        _values = Guard.NotNull(values);

        MatchBehaviour = matchBehaviour;
        IgnoreCase = ignoreCase;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        Func<string?, bool> equals = IgnoreCase
            ? pattern => string.Equals(pattern, input, StringComparison.OrdinalIgnoreCase)
            : pattern => pattern == input;

        var score = MatchScores.ToScore(_values.Select(v => equals(v)).ToArray(), MatchOperator);
        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score));
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _values;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(ExactMatcher);

    /// <inheritdoc />
    public bool IgnoreCase { get; }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_values)}" +
               $")";
    }
}