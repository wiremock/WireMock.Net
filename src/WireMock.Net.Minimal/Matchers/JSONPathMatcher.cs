// Copyright © WireMock.Net

using System;
using System.Linq;
using AnyOfTypes;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// JsonPathMatcher
/// </summary>
/// <seealso cref="IStringMatcher" />
/// <seealso cref="IObjectMatcher" />
public class JsonPathMatcher : IStringMatcher, IObjectMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JsonPathMatcher(params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or,
        patterns.ToAnyOfPatterns())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public JsonPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch,
        MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public JsonPathMatcher(
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
                var jToken = JToken.Parse(input);
                score = IsMatch(jToken);
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
        Exception? exception = null;

        // When input is null or byte[], return Mismatch.
        if (input != null && input is not byte[])
        {
            try
            {
                // Check if JToken or object
                JToken jToken = input as JToken ?? JObject.FromObject(input);
                score = IsMatch(jToken);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(JsonPathMatcher);

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

    private double IsMatch(JToken jToken)
    {
        var array = ConvertJTokenToJArrayIfNeeded(jToken);

        // The SelectToken method can accept a string path to a child token ( i.e. "Manufacturers[0].Products[0].Price").
        // In that case it will return a JValue (some type) which does not implement the IEnumerable interface.
        var values = _patterns.Select(pattern => array.SelectToken(pattern.GetPattern()) != null).ToArray();

        return MatchScores.ToScore(values, MatchOperator);
    }

    // https://github.com/wiremock/WireMock.Net/issues/965
    // https://stackoverflow.com/questions/66922188/newtonsoft-jsonpath-with-c-sharp-syntax
    // Filtering using SelectToken() isn't guaranteed to work for objects inside objects -- only objects inside arrays.
    // So this code checks if it's an JArray, if it's not an array, construct a new JArray.
    private static JToken ConvertJTokenToJArrayIfNeeded(JToken jToken)
    {
        if (jToken.Count() == 1)
        {
            var property = jToken.First();
            var item = property.First();
            if (item is JArray)
            {
                return jToken;
            }

            return new JObject
            {
                [property.Path] = new JArray(item)
            };
        }

        return jToken;
    }
}