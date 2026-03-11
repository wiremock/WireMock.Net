// Copyright © WireMock.Net

using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// FormUrl Encoded fields Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
/// <inheritdoc cref="IIgnoreCaseMatcher"/>
public class FormUrlEncodedMatcher : IStringMatcher, IIgnoreCaseMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    private readonly List<(WildcardMatcher Key, WildcardMatcher? Value)> _pairs = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(MatchBehaviour.AcceptOnMatch, [pattern], ignoreCase, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        MatchBehaviour matchBehaviour,
        AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(matchBehaviour, [pattern], ignoreCase, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        AnyOf<string, StringPattern>[] patterns,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        MatchBehaviour matchBehaviour,
        AnyOf<string, StringPattern>[] patterns,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or)
    {
        _patterns = Guard.NotNull(patterns);
        IgnoreCase = ignoreCase;
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;

        foreach (var pattern in _patterns)
        {
            if (QueryStringParser.TryParse(pattern, IgnoreCase, out var nameValueCollection))
            {
                foreach (var nameValue in nameValueCollection)
                {
                    var keyMatcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, [nameValue.Key], ignoreCase, MatchOperator);
                    var valueMatcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, [nameValue.Value], ignoreCase, MatchOperator);
                    _pairs.Add((keyMatcher, valueMatcher));
                }
            }
        }
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        // Input is null or empty and if no patterns defined, return Perfect match.
        if (string.IsNullOrEmpty(input) && _patterns.Length == 0)
        {
            return MatchResult.From(Name, MatchScores.Perfect);
        }

        if (!QueryStringParser.TryParse(input, IgnoreCase, out var inputNameValueCollection))
        {
            return MatchResult.From(Name, MatchScores.Mismatch);
        }

        var matches = GetMatches(inputNameValueCollection);

        var score = MatchScores.ToScore(matches, MatchOperator);
        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score));
    }

    private bool[] GetMatches(IDictionary<string, string> inputNameValueCollection)
    {
        var matches = new List<bool>();
        if (_pairs.Count > inputNameValueCollection.Count)
        {
            matches.AddRange(Enumerable.Repeat(false, _pairs.Count - inputNameValueCollection.Count));
        }

        foreach (var inputKeyValuePair in inputNameValueCollection)
        {
            var match = false;
            foreach (var pair in _pairs)
            {
                var keyMatchResult = pair.Key.IsMatch(inputKeyValuePair.Key).IsPerfect();
                if (keyMatchResult)
                {
                    match = pair.Value?.IsMatch(inputKeyValuePair.Value).IsPerfect() ?? false;
                    if (match)
                    {
                        break;
                    }
                }
            }

            matches.Add(match);
        }

        return matches.ToArray();
    }

    /// <inheritdoc />
    public virtual AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public virtual string Name => nameof(FormUrlEncodedMatcher);

    /// <inheritdoc />
    public bool IgnoreCase { get; }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}" +
               $")";
    }
}