// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnyOfTypes;
using GraphQL;
using GraphQL.Types;
using GraphQLParser;
using GraphQLParser.AST;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Exceptions;
using WireMock.Extensions;
using WireMock.GraphQL.Models;
using WireMock.Models;
using WireMock.Models.GraphQL;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// GrapQLMatcher Schema Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
public class GraphQLMatcher : IGraphQLMatcher
{
    private sealed class GraphQLRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? Query { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public Dictionary<string, object?>? Variables { get; set; }
    }

    private readonly AnyOf<string, StringPattern>[] _patterns;

    private readonly ISchema _schema;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// An optional dictionary defining the custom Scalar and the type.
    /// </summary>
    public IDictionary<string, Type>? CustomScalars { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphQLMatcher"/> class.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public GraphQLMatcher(
        AnyOf<string, StringPattern, ISchemaData> schema,
        MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch,
        MatchOperator matchOperator = MatchOperator.Or
    ) : this(schema, null, matchBehaviour, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphQLMatcher"/> class.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. (optional)</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public GraphQLMatcher(
        AnyOf<string, StringPattern, ISchemaData> schema,
        IDictionary<string, Type>? customScalars,
        MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch,
        MatchOperator matchOperator = MatchOperator.Or
    )
    {
        Guard.NotNull(schema);
        CustomScalars = customScalars;
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;

        var patterns = new List<AnyOf<string, StringPattern>>();
        switch (schema.CurrentType)
        {
            case AnyOfType.First:
                patterns.Add(schema.First);
                _schema = BuildSchema(schema);
                break;

            case AnyOfType.Second:
                patterns.Add(schema.Second);
                _schema = BuildSchema(schema.Second.Pattern);
                break;

            case AnyOfType.Third:
                _schema = ((SchemaWrapper)schema.Third).Schema;
                break;

            default:
                throw new NotSupportedException();
        }
        _patterns = patterns.ToArray();
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (input != null && TryGetGraphQLRequest(input, out var graphQLRequest))
        {
            try
            {
                var executionResult = new DocumentExecuter().ExecuteAsync(eo =>
                {
                    eo.ThrowOnUnhandledException = true;

                    eo.Schema = _schema;
                    eo.Query = graphQLRequest.Query;

                    if (graphQLRequest.Variables != null)
                    {
                        eo.Variables = new Inputs(graphQLRequest.Variables);
                    }
                }).GetAwaiter().GetResult();

                if (executionResult.Errors == null || executionResult.Errors.Count == 0)
                {
                    score = MatchScores.Perfect;
                }
                else
                {
                    exception = executionResult.Errors.OfType<Exception>().ToArray().ToException();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(GraphQLMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return "NotImplemented";
    }

    private static bool TryGetGraphQLRequest(string input, [NotNullWhen(true)] out GraphQLRequest? graphQLRequest)
    {
        try
        {
            graphQLRequest = JsonConvert.DeserializeObject<GraphQLRequest>(input);
            return graphQLRequest != null;
        }
        catch
        {
            graphQLRequest = default;
            return false;
        }
    }

    /// <param name="typeDefinitions">A textual description of the schema in SDL (Schema Definition Language) format.</param>
    private ISchema BuildSchema(string typeDefinitions)
    {
        var schema = Schema.For(typeDefinitions);

        // #984
        var graphTypes = schema.BuiltInTypeMappings.Select(tm => tm.graphType).ToArray();
        schema.RegisterTypes(graphTypes);

        var doc = Parser.Parse(typeDefinitions);
        var scalarTypeDefinitions = doc.Definitions
            .Where(d => d.Kind == ASTNodeKind.ScalarTypeDefinition)
            .OfType<GraphQLTypeDefinition>();

        foreach (var scalarTypeDefinitionName in scalarTypeDefinitions.Select(s => s.Name.StringValue))
        {
            var customScalarGraphTypeName = $"{scalarTypeDefinitionName}GraphType";
            if (graphTypes.All(t => t.Name != customScalarGraphTypeName)) // Only process when not built-in.
            {
                // Check if this custom Scalar is defined in the dictionary
                if (CustomScalars == null || !CustomScalars.TryGetValue(scalarTypeDefinitionName, out var clrType))
                {
                    throw new WireMockException($"The GraphQL Scalar type '{scalarTypeDefinitionName}' is not defined in the CustomScalars dictionary.");
                }

                // Create a custom Scalar GraphType (extending the WireMockCustomScalarGraphType<{clrType}> class)
                var customScalarGraphType = ReflectionUtils.CreateGenericType(customScalarGraphTypeName, typeof(WireMockCustomScalarGraphType<>), clrType);
                schema.RegisterType(customScalarGraphType);
            }
        }

        return schema;
    }
}