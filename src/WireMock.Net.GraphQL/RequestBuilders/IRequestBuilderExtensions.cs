// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilderExtensions extensions for GraphQL.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IRequestBuilderExtensions
{
    /// <summary>
    /// WithGraphQLSchema: The GraphQL schema as a string.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, string schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(requestBuilder, schema, matchBehaviour);
    }

    /// <summary>
    /// WithGraphQLSchema: The GraphQL schema as a string.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, string schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(requestBuilder, schema, customScalars, matchBehaviour);
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a string.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsGraphQL(this IRequestBuilder requestBuilder, string schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a string.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsGraphQL(this IRequestBuilder requestBuilder, string schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
    }

    /// <summary>
    /// WithGraphQLSchema: The GraphQL schema as a ISchema.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, GraphQL.Types.ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(requestBuilder, schema, matchBehaviour);
    }

    /// <summary>
    /// WithGraphQLSchema: The GraphQL schema as a ISchema.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, GraphQL.Types.ISchema schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithBodyAsGraphQL(requestBuilder, schema, customScalars, matchBehaviour);
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a ISchema.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsGraphQL(this IRequestBuilder requestBuilder, GraphQL.Types.ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a ISchema.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsGraphQL(this IRequestBuilder requestBuilder, GraphQL.Types.ISchema schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
    }

    /// <summary>
    /// WithBodyAsGraphQLSchema: Body as GraphQL schema as a string.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="body">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsGraphQLSchema(this IRequestBuilder requestBuilder, string body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithGraphQLSchema(requestBuilder, body, matchBehaviour);
    }
}