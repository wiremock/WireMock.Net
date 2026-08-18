// Copyright © WireMock.Net

using AnyOfTypes;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Types;
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

    private readonly List<(WildcardMatcher Key, WildcardMatcher[]? Values)> KeyValueMatchers = [];

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
                    var keyMatcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, nameValue.Key, ignoreCase);
                    var valueMatchers = nameValue.Value.Select(value => new WildcardMatcher(MatchBehaviour.AcceptOnMatch, value, ignoreCase)).ToArray();

                    KeyValueMatchers.Add((keyMatcher, valueMatchers));
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
        return MatchResult.From(Name, score);
    }

    private List<double> GetMatches(IDictionary<string, WireMockList<string>> inputNameValueCollection)
    {
        var inputPairs = inputNameValueCollection.ToArray();
        var rowCount = inputPairs.Length;
        var columnCount = KeyValueMatchers.Count;

        if (rowCount == 0 && columnCount == 0)
        {
            return [];
        }

        var matrix = new double[rowCount][];

        for (var row = 0; row < rowCount; row++)
        {
            matrix[row] = new double[columnCount];

            var inputKeyValuePair = inputPairs[row];
            var inputKey = inputKeyValuePair.Key;
            var inputValues = inputKeyValuePair.Value;

            for (var column = 0; column < columnCount; column++)
            {
                var (keyMatcher, valuesMatchers) = KeyValueMatchers[column];

                var keyScore = keyMatcher.IsMatch(inputKey).Score;
                var valueScore = valuesMatchers != null ? MatchScores.ToScore(inputValues, valuesMatchers) : MatchScores.Perfect;

                matrix[row][column] = Math.Min(keyScore, valueScore);
            }
        }

        var rowScores = rowCount == 0 ? [] : matrix.Select(row => row.Length == 0 ? MatchScores.Mismatch : row.Max()).ToList();

        var columnScores = new List<double>();
        for (var column = 0; column < columnCount; column++)
        {
            columnScores.Add(rowCount == 0 ? MatchScores.Mismatch : matrix.Max(row => row[column]));
        }

        rowScores.AddRange(columnScores);
        return rowScores;
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