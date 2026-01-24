// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Text.RegularExpressions;
using AnyOfTypes;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.RegularExpressions;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// Regular Expression Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
/// <inheritdoc cref="IIgnoreCaseMatcher"/>
public class RegexMatcher : IStringMatcher, IIgnoreCaseMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;
    private readonly Regex[] _expressions;
    private readonly bool _useRegexExtended;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="useRegexExtended">Use RegexExtended (default = true).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public RegexMatcher(
        [RegexPattern] AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        bool useRegexExtended = true,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(MatchBehaviour.AcceptOnMatch, [pattern], ignoreCase, useRegexExtended, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="useRegexExtended">Use RegexExtended (default = true).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public RegexMatcher(
        MatchBehaviour matchBehaviour,
        [RegexPattern] AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        bool useRegexExtended = true,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(matchBehaviour, [pattern], ignoreCase, useRegexExtended, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="useRegexExtended">Use RegexExtended (default = true).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public RegexMatcher(
        MatchBehaviour matchBehaviour,
        [RegexPattern] AnyOf<string, StringPattern>[] patterns,
        bool ignoreCase = false,
        bool useRegexExtended = true,
        MatchOperator matchOperator = MatchOperator.Or)
    {
        _patterns = Guard.NotNull(patterns);
        IgnoreCase = ignoreCase;
        _useRegexExtended = useRegexExtended;
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;

        var options = RegexOptions.Compiled | RegexOptions.Multiline;

        if (ignoreCase)
        {
            options |= RegexOptions.IgnoreCase;
        }

        _expressions = patterns.Select(p => useRegexExtended ? new RegexExtended(p.GetPattern(), options) : new Regex(p.GetPattern(), options, RegexConstants.DefaultTimeout)).ToArray();
    }

    /// <inheritdoc />
    public virtual MatchResult IsMatch(string? input)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (input != null)
        {
            try
            {
                score = MatchScores.ToScore(_expressions.Select(e => e.IsMatch(input)).ToArray(), MatchOperator);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    /// <inheritdoc />
    public virtual AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public virtual string Name => nameof(RegexMatcher);

    /// <inheritdoc />
    public bool IgnoreCase { get; }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public virtual string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(_useRegexExtended)}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}" +
               $")";
    }
}