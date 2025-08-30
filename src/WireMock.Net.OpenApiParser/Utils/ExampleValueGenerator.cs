// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Stef.Validation;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Utils;

internal class ExampleValueGenerator
{
    private readonly IWireMockOpenApiParserExampleValues _exampleValues;

    public ExampleValueGenerator(WireMockOpenApiParserSettings settings)
    {
        Guard.NotNull(settings);

        // Check if user provided an own implementation
        if (settings.ExampleValues is null)
        {
            if (settings.DynamicExamples)
            {
                _exampleValues = new WireMockOpenApiParserDynamicExampleValues();
            }
            else
            {
                _exampleValues = new WireMockOpenApiParserExampleValues();
            }
        }
        else
        {
            _exampleValues = settings.ExampleValues;
        }
    }

    public JsonNode GetExampleValue(IOpenApiSchema? schema)
    {
        var schemaExample = schema?.Example;
        var schemaEnum = schema?.Enum?.FirstOrDefault();

        _exampleValues.Schema = schema;

        switch (schema?.GetSchemaType(out _))
        {
            case JsonSchemaType.Boolean:
                var exampleBoolean = schemaExample?.GetValue<bool>();
                return exampleBoolean ?? _exampleValues.Boolean;

            case JsonSchemaType.Integer:
                var exampleInteger = schemaExample?.GetValue<decimal>();
                var enumInteger = schemaEnum?.GetValue<decimal>();
                var valueIntegerEnumOrExample = enumInteger ?? exampleInteger;
                return valueIntegerEnumOrExample ?? _exampleValues.Integer;

            case JsonSchemaType.Number:
                switch (schema.GetSchemaFormat())
                {
                    case SchemaFormat.Float:
                        var exampleFloat = schemaExample?.GetValue<float>();
                        var enumFloat = schemaEnum?.GetValue<float>();
                        var valueFloatEnumOrExample = enumFloat ?? exampleFloat;
                        return valueFloatEnumOrExample ?? _exampleValues.Float;

                    default:
                        var exampleDecimal = schemaExample?.GetValue<decimal>();
                        var enumDecimal = schemaEnum?.GetValue<decimal>();
                        var valueDecimalEnumOrExample = enumDecimal ?? exampleDecimal;
                        return valueDecimalEnumOrExample ?? _exampleValues.Decimal;
                }

            default:
                switch (schema?.GetSchemaFormat())
                {
                    case SchemaFormat.Date:
                        var exampleDate = schemaExample?.GetValue<string>();
                        var enumDate = schemaEnum?.GetValue<string>();
                        var valueDateEnumOrExample = enumDate ?? exampleDate;
                        return valueDateEnumOrExample ?? DateTimeUtils.ToRfc3339Date(_exampleValues.Date());

                    case SchemaFormat.DateTime:
                        var exampleDateTime = schemaExample?.GetValue<string>();
                        var enumDateTime = schemaEnum?.GetValue<string>();
                        var valueDateTimeEnumOrExample = enumDateTime ?? exampleDateTime;
                        return valueDateTimeEnumOrExample ?? DateTimeUtils.ToRfc3339DateTime(_exampleValues.DateTime());

                    case SchemaFormat.Byte:
                        var exampleByte = schemaExample?.GetValue<byte[]>();
                        var enumByte = schemaEnum?.GetValue<byte[]>();
                        var valueByteEnumOrExample = enumByte ?? exampleByte;
                        return Convert.ToBase64String(valueByteEnumOrExample ?? _exampleValues.Bytes);

                    default:
                        var exampleString = schemaExample?.GetValue<string>();
                        var enumString = schemaEnum?.GetValue<string>();
                        var valueStringEnumOrExample = enumString ?? exampleString;
                        return valueStringEnumOrExample ?? _exampleValues.String;
                }
        }
    }
}