// Copyright © WireMock.Net

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Extensions;

internal static class OpenApiSchemaExtensions
{
    public static bool TryGetXNullable(this IOpenApiSchema schema, out bool value)
    {
        value = false;

#pragma warning disable S3011
        var extensionsProperty = schema.GetType().GetProperty("Extensions", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011
        if (extensionsProperty?.GetValue(schema) is Dictionary<string, IOpenApiExtension> extensions && extensions.TryGetValue("x-nullable", out var nullExtRawValue))
        {
            var nodeProperty = nullExtRawValue.GetType().GetProperty("Node", BindingFlags.Instance | BindingFlags.Public);
            if (nodeProperty?.GetValue(nullExtRawValue) is JsonNode jsonNode)
            {
                return jsonNode.GetValueKind() == JsonValueKind.True;
            }
        }

        return false;
    }

    public static JsonSchemaType? GetSchemaType(this IOpenApiSchema? schema, out bool isNullable)
    {
        isNullable = false;

        if (schema == null)
        {
            return null;
        }

        if (schema.Type == null)
        {
            if (schema.AllOf?.Any() == true || schema.AnyOf?.Any() == true)
            {
                return JsonSchemaType.Object;
            }
        }

        isNullable = (schema.Type | JsonSchemaType.Null) == JsonSchemaType.Null || (schema.TryGetXNullable(out var xNullable) && xNullable);

        // Removes the Null flag from the schema.Type, ensuring the returned value represents a non-nullable type.
        return schema.Type & ~JsonSchemaType.Null;
    }

    public static SchemaFormat GetSchemaFormat(this IOpenApiSchema? schema)
    {
        return (schema?.Format) switch
        {
            "float" => SchemaFormat.Float,
            "double" => SchemaFormat.Double,
            "int32" => SchemaFormat.Int32,
            "int64" => SchemaFormat.Int64,
            "date" => SchemaFormat.Date,
            "date-time" => SchemaFormat.DateTime,
            "password" => SchemaFormat.Password,
            "byte" => SchemaFormat.Byte,
            "binary" => SchemaFormat.Binary,
            _ => SchemaFormat.Undefined,
        };
    }

    internal static JsonNode? FindFirstExample(this IOpenApiSchema? schema)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return schema?.Examples?.FirstOrDefault() ?? schema?.Example;
#pragma warning restore CS0618 // Type or member is obsolete
    }
}