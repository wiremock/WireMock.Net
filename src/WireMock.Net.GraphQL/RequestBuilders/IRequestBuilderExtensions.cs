// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using GraphQL.Types;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Models.GraphQL;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilderExtensions extensions for GraphQL.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IRequestBuilderExtensions
{
    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL body as a string.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, string schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
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
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, string schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a <see cref="ISchema"/>.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, ISchema schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, new SchemaDataWrapper(schema)));
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a <see cref="ISchema"/>.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, ISchema schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, new SchemaDataWrapper(schema), customScalars));
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a <see cref="ISchemaData"/>.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, ISchemaData schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema));
    }

    /// <summary>
    /// WithBodyAsGraphQL: The GraphQL schema as a <see cref="ISchemaData"/>.
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="schema">The GraphQL schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithGraphQLSchema(this IRequestBuilder requestBuilder, ISchemaData schema, IDictionary<string, Type>? customScalars, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return Guard.NotNull(requestBuilder).Add(new RequestMessageGraphQLMatcher(matchBehaviour, schema, customScalars));
    }
}