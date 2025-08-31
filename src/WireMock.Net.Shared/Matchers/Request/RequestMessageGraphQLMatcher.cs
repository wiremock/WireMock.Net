// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Models;
using WireMock.Models.GraphQL;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body GraphQL matcher.
/// </summary>
public class RequestMessageGraphQLMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. [optional]</param>
    public RequestMessageGraphQLMatcher(MatchBehaviour matchBehaviour, string schema, IDictionary<string, Type>? customScalars = null) :
        this(CreateMatcherArray(matchBehaviour, schema, customScalars))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. [optional]</param>
    public RequestMessageGraphQLMatcher(MatchBehaviour matchBehaviour, ISchemaData schema, IDictionary<string, Type>? customScalars = null) :
        this(CreateMatcherArray(matchBehaviour, new AnyOf<string, StringPattern, ISchemaData>(schema), customScalars))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageGraphQLMatcher(params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageGraphQLMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    public RequestMessageGraphQLMatcher(MatchOperator matchOperator, params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var results = CalculateMatchResults(requestMessage);
        var (score, exception) = MatchResult.From(results, MatchOperator).Expand();

        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private static MatchResult CalculateMatchResult(IRequestMessage requestMessage, IMatcher matcher)
    {
        // In case the matcher is a IStringMatcher and the body is a Json or a String, use the BodyAsString to match on.
        if (matcher is IStringMatcher stringMatcher && requestMessage.BodyData?.DetectedBodyType is BodyType.Json or BodyType.String or BodyType.FormUrlEncoded)
        {
            return stringMatcher.IsMatch(requestMessage.BodyData.BodyAsString);
        }

        return default;
    }

    private IReadOnlyList<MatchResult> CalculateMatchResults(IRequestMessage requestMessage)
    {
        return Matchers == null ? [new MatchResult()] : Matchers.Select(matcher => CalculateMatchResult(requestMessage, matcher)).ToArray();
    }

    private static IMatcher[] CreateMatcherArray(
        MatchBehaviour matchBehaviour,
        AnyOf<string, StringPattern, ISchemaData> schema,
        IDictionary<string, Type>? customScalars
    )
    {
        if (TypeLoader.TryLoadNewInstance<IGraphQLMatcher>(out var graphQLMatcher, schema, customScalars, matchBehaviour, MatchOperator.Or))
        {
            return [graphQLMatcher];
        }
        return [];
    }
}