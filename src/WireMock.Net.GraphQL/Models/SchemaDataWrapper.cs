// Copyright © WireMock.Net

using GraphQL.Types;
using WireMock.Models.GraphQL;

namespace WireMock.Models;

/// <summary>
/// Represents a wrapper for schema data, providing access to the associated schema.
/// </summary>
/// <param name="schema"></param>
public class SchemaDataWrapper(ISchema schema) : ISchemaData
{
    /// <summary>
    /// Gets the schema associated with the current instance.
    /// </summary>
    public ISchema Schema { get; } = schema;
}