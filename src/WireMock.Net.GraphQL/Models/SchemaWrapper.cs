// Copyright © WireMock.Net

using GraphQL.Types;
using WireMock.Models.GraphQL;

namespace WireMock.Models;

public class SchemaWrapper(ISchema schema) : ISchemaData
{
    public ISchema Schema { get; } = schema;
}