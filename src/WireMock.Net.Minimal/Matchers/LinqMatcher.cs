// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using AnyOfTypes;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Json;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// System.Linq.Dynamic.Core Expression Matcher
/// </summary>
/// <inheritdoc cref="IObjectMatcher"/>
/// <inheritdoc cref="IStringMatcher"/>
public class LinqMatcher : IObjectMatcher, IStringMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    public LinqMatcher(AnyOf<string, StringPattern> pattern) : this(new[] { pattern })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public LinqMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    public LinqMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> pattern) : this(matchBehaviour, MatchOperator.Or, pattern)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public LinqMatcher(
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
        Exception? error = null;

        // Convert a single input string to a Queryable string-list with 1 entry.
        IQueryable queryable = new[] { input }.AsQueryable();

        try
        {
            // Use the Any(...) method to check if the result matches
            score = MatchScores.ToScore(_patterns.Select(pattern => queryable.Any(pattern.GetPattern())).ToArray(), MatchOperator);
        }
        catch (Exception e)
        {
            error = e;
        }

        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), error);
    }

    /// <inheritdoc />
    public MatchResult IsMatch(object? input)
    {
        var score = MatchScores.Mismatch;
        Exception? error = null;

        JArray jArray;
        try
        {
            jArray = new JArray { input };
        }
        catch
        {
            jArray = input == null ? new JArray() : new JArray { JToken.FromObject(input) };
        }

        // Convert a single object to a Queryable JObject-list with 1 entry.
        var queryable = jArray.ToDynamicClassArray().AsQueryable();

        try
        {
            var patternsAsStringArray = _patterns.Select(p => p.GetPattern()).ToArray();
            var scores = patternsAsStringArray.Select(p => queryable.Any(p)).ToArray();

            score = MatchScores.ToScore(scores, MatchOperator);
        }
        catch (Exception e)
        {
            error = e;
        }

        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), error);
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(LinqMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {typeof(LinqMatcher).FullName}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}" +
               $")";
    }
}