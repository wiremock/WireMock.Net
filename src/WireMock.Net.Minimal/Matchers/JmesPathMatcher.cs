// Copyright © WireMock.Net

using System;
using System.Linq;
using AnyOfTypes;
using DevLab.JmesPath;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// http://jmespath.org/
/// </summary>
public class JmesPathMatcher : IStringMatcher, IObjectMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public object Value { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns.ToAnyOfPatterns())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use.</param>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(MatchOperator matchOperator = MatchOperator.Or, params AnyOf<string, StringPattern>[] patterns) :
        this(MatchBehaviour.AcceptOnMatch, matchOperator, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JmesPathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use.</param>
    /// <param name="patterns">The patterns.</param>
    public JmesPathMatcher(
        MatchBehaviour matchBehaviour,
        MatchOperator matchOperator = MatchOperator.Or,
        params AnyOf<string, StringPattern>[] patterns)
    {
        _patterns = Guard.NotNull(patterns);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
        Value = patterns;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (!string.IsNullOrWhiteSpace(input))
        {
            try
            {
                var results = _patterns.Select(pattern => bool.Parse(new JmesPath().Transform(input, pattern.GetPattern()))).ToArray();
                score = MatchScores.ToScore(results, MatchOperator);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    /// <inheritdoc />
    public MatchResult IsMatch(object? input)
    {
        var score = MatchScores.Mismatch;

        // When input is null or byte[], return Mismatch.
        if (input != null && !(input is byte[]))
        {
            var inputAsString = JsonConvert.SerializeObject(input);
            return IsMatch(inputAsString);
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score));
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(JmesPathMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}" +
               $")";
    }
}