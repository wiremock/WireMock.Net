// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using AnyOfTypes;
using Newtonsoft.Json;
using WireMock.Models.GraphQL;

namespace WireMock.Models;

/// <summary>
/// GraphQLSchemaDetails
/// </summary>
public class GraphQLSchemaDetails
{
    /// <summary>
    /// The GraphQL schema as a string.
    /// </summary>
    public string? SchemaAsString { get; set; }

    /// <summary>
    /// The GraphQL schema as a StringPattern.
    /// </summary>
    public StringPattern? SchemaAsStringPattern { get; set; }

    /// <summary>
    /// The GraphQL schema as a <seealso cref="ISchemaData"/>.
    /// </summary>
    public ISchemaData? SchemaAsISchemaData { get; set; }

    /// <summary>
    /// The GraphQL Schema.
    /// </summary>
    [JsonIgnore]
    public AnyOf<string, StringPattern, ISchemaData>? Schema
    {
        get
        {
            if (SchemaAsString != null)
            {
                return SchemaAsString;
            }

            if (SchemaAsStringPattern != null)
            {
                return SchemaAsStringPattern;
            }

            if (SchemaAsISchemaData != null)
            {
                return new AnyOf<string, StringPattern, ISchemaData>(SchemaAsISchemaData);
            }

            return null;
        }
    }

    /// <summary>
    /// The custom Scalars to define for this schema.
    /// </summary>
    public IDictionary<string, Type>? CustomScalars { get; set; }
}