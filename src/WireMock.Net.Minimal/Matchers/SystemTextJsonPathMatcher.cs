// Copyright © WireMock.Net

using System.Text.Json.Nodes;
using AnyOfTypes;
using Json.Path;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// SystemTextJsonPathMatcher - behaves the same as <see cref="JsonPathMatcher"/> but uses System.Text.Json instead of Newtonsoft.Json.
/// </summary>
/// <seealso cref="IStringMatcher" />
/// <seealso cref="IObjectMatcher" />
public class SystemTextJsonPathMatcher : IStringMatcher, IObjectMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextJsonPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public SystemTextJsonPathMatcher(params string[] patterns)
        : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns.ToAnyOfPatterns())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextJsonPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public SystemTextJsonPathMatcher(params AnyOf<string, StringPattern>[] patterns)
        : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextJsonPathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public SystemTextJsonPathMatcher(
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
                var node = JsonNode.Parse(input!);
                score = IsMatchInternal(node);
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
                JsonNode? node = input switch
                {
                    JsonNode jsonNode => jsonNode,
                    string str => JsonNode.Parse(str),
                    _ => JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(input))
                };

                score = IsMatchInternal(node);
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
    public string Name => nameof(SystemTextJsonPathMatcher);

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

    private double IsMatchInternal(JsonNode? node)
    {
        // JsonPath.Net requires the node to be inside an object or array for filter expressions.
        // Similar to JsonPathMatcher's ConvertJTokenToJArrayIfNeeded, wrap a plain object in an array
        // when it's an object with a single non-array child property.
        var evaluationNode = WrapIfNeeded(node);

        var values = _patterns
            .Select(pattern =>
            {
                var path = JsonPath.Parse(pattern.GetPattern());
                var result = path.Evaluate(evaluationNode);
                return result.Matches is { Count: > 0 };
            })
            .ToArray();

        return MatchScores.ToScore(values, MatchOperator);
    }

    // Mirrors JsonPathMatcher.ConvertJTokenToJArrayIfNeeded:
    // If the node is an object with exactly one property whose value is not already an array,
    // wrap that value in an array so that filter expressions (e.g. [?(@.x == y)]) can match.
    private static JsonNode? WrapIfNeeded(JsonNode? node)
    {
        if (node is not JsonObject obj)
        {
            return node;
        }

        var properties = obj.ToList();
        if (properties.Count != 1)
        {
            return node;
        }

        var single = properties[0];
        if (single.Value is JsonArray)
        {
            return node;
        }

        var clonedValue = JsonNode.Parse(single.Value?.ToJsonString() ?? "null");
        return new JsonObject
        {
            [single.Key] = new JsonArray(clonedValue)
        };
    }
}
