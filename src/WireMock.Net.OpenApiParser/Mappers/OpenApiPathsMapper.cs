// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Net.OpenApiParser.Utils;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace WireMock.Net.OpenApiParser.Mappers;

internal class OpenApiPathsMapper(WireMockOpenApiParserSettings settings)
{
    private const string HeaderContentType = "Content-Type";

    private readonly WireMockOpenApiParserSettings _settings = Guard.NotNull(settings);
    private readonly ExampleValueGenerator _exampleValueGenerator = new(settings);

    public IReadOnlyList<MappingModel> ToMappingModels(OpenApiPaths? paths, IList<OpenApiServer> servers)
    {
        return paths?
            .OrderBy(p => p.Key)
            .Select(p => MapPath(p.Key, p.Value, servers))
            .SelectMany(x => x)
            .ToArray() ?? [];
    }

    private MappingModel[] MapPath(string path, IOpenApiPathItem pathItem, IList<OpenApiServer> servers)
    {
        return pathItem.Operations?.Select(o => MapOperationToMappingModel(path, o.Key.ToString().ToUpperInvariant(), o.Value, servers)).ToArray() ?? [];
    }

    private MappingModel MapOperationToMappingModel(string path, string httpMethod, OpenApiOperation operation, IList<OpenApiServer> servers)
    {
        var queryParameters = operation.Parameters?.Where(p => p.In == ParameterLocation.Query) ?? [];
        var pathParameters = operation.Parameters?.Where(p => p.In == ParameterLocation.Path) ?? [];
        var requestHeaders = operation.Parameters?.Where(p => p.In == ParameterLocation.Header) ?? [];

        return new MappingModel
        {
            Guid = Guid.NewGuid(),
            Request = new RequestModel
            {
                Methods = [httpMethod],
                Path = PathUtils.Combine(MapBasePath(servers), MapPathWithParameters(path, pathParameters)),
                Params = MapQueryParameters(queryParameters),
                Headers = MapRequestHeaders(requestHeaders),
                Body = GetRequestBodyModel(operation.RequestBody)
            },
            Response = GetResponseModel(operation.Responses?.FirstOrDefault())
        };
    }

    private BodyModel GetRequestBodyModel(IOpenApiRequestBody? openApiRequestBody)
    {
        if (openApiRequestBody is not { Content: not null, Required: true })
        {
            return new BodyModel();
        }

        var content = openApiRequestBody.Content;

        TryGetContent(content, out var requestContent, out _);

        var requestExample = requestContent?.Example;
        var requestExamples = requestContent?.Examples;
        var requestSchemaExample = requestContent?.Schema?.Example;
        var requestSchemaExamples = requestContent?.Schema?.Examples;

        JsonNode? request;
        if (requestExample != null)
        {
            request = requestExample;
        }
        else if (requestSchemaExample != null)
        {
            request = requestSchemaExample;
        }
        else if (requestExamples != null)
        {
            request = requestExamples.FirstOrDefault().Value.Value;
        }
        else if (requestSchemaExamples != null)
        {
            request = requestSchemaExamples.FirstOrDefault();
        }
        else
        {
            var requestSchema = content?.FirstOrDefault().Value.Schema;
            request = MapSchemaToObject(requestSchema);
        }

        return MapRequestBody(request) ?? new BodyModel();
    }

    private ResponseModel GetResponseModel(KeyValuePair<string, IOpenApiResponse>? openApiResponse)
    {
        var content = openApiResponse?.Value.Content;

        TryGetContent(content, out var responseContent, out var contentType);

        var responseExample = responseContent?.Example;
        var responseExamples = responseContent?.Examples;
        var responseSchemaExample = responseContent?.Schema?.Example;
        var responseSchemaExamples = responseContent?.Schema?.Examples;

        JsonNode? response;
        if (responseExample != null)
        {
            response = responseExample;
        }
        else if (responseSchemaExample != null)
        {
            response = responseSchemaExample;
        }
        else if (responseExamples != null)
        {
            response = responseExamples.FirstOrDefault().Value.Value;
        }
        else if (responseSchemaExamples != null)
        {
            response = responseSchemaExamples.FirstOrDefault();
        }
        else
        {
            var responseSchema = content?.FirstOrDefault().Value?.Schema;
            response = MapSchemaToObject(responseSchema);
        }

        return new ResponseModel
        {
            StatusCode = int.TryParse(openApiResponse?.Key, out var httpStatusCode) ? httpStatusCode : 200,
            Headers = MapHeaders(contentType, openApiResponse?.Value.Headers),
            BodyAsJson = response != null ? JsonConvert.DeserializeObject(SystemTextJsonSerializer.Serialize(response)) : null
        };
    }

    private BodyModel? MapRequestBody(JsonNode? requestBody)
    {
        if (requestBody == null)
        {
            return null;
        }

        return new BodyModel
        {
            Matcher = new MatcherModel
            {
                Name = "JsonMatcher",
                Pattern = SystemTextJsonSerializer.Serialize(requestBody, new JsonSerializerOptions { WriteIndented = true }),
                IgnoreCase = _settings.RequestBodyIgnoreCase
            }
        };
    }

    private static bool TryGetContent(IDictionary<string, OpenApiMediaType>? contents, [NotNullWhen(true)] out OpenApiMediaType? openApiMediaType, [NotNullWhen(true)] out string? contentType)
    {
        openApiMediaType = null;
        contentType = null;

        if (contents == null || contents.Values.Count == 0)
        {
            return false;
        }

        if (contents.TryGetValue("application/json", out var content))
        {
            openApiMediaType = content;
            contentType = "application/json";
        }
        else
        {
            var first = contents.FirstOrDefault();
            openApiMediaType = first.Value;
            contentType = first.Key;
        }

        return true;
    }

    private JsonNode? MapSchemaToObject(IOpenApiSchema? schema)
    {
        if (schema == null)
        {
            return null;
        }

        switch (schema.GetSchemaType(out _))
        {
            case JsonSchemaType.Array:
                var array = new JsonArray();
                for (var i = 0; i < _settings.NumberOfArrayItems; i++)
                {
                    if (schema.Items?.Properties?.Count > 0)
                    {
                        var item = new JsonObject();
                        foreach (var property in schema.Items.Properties)
                        {
                            item[property.Key] = MapSchemaToObject(property.Value);
                        }

                        array.Add(item);
                    }
                    else
                    {
                        var arrayItem = MapSchemaToObject(schema.Items);
                        array.Add(arrayItem);
                    }
                }

                if (schema.AllOf?.Count > 0)
                {
                    array.Add(MapSchemaAllOfToObject(schema));
                }

                return array;

            case JsonSchemaType.Boolean:
            case JsonSchemaType.Integer:
            case JsonSchemaType.Number:
            case JsonSchemaType.String:
                return _exampleValueGenerator.GetExampleValue(schema);

            case JsonSchemaType.Object:
                var propertyAsJsonObject = new JsonObject();
                foreach (var schemaProperty in schema.Properties ?? new Dictionary<string, IOpenApiSchema>())
                {
                    propertyAsJsonObject[schemaProperty.Key] = MapPropertyAsJsonNode(schemaProperty.Value);
                }

                if (schema.AllOf?.Count > 0)
                {
                    foreach (var group in schema.AllOf.SelectMany(p => p.Properties ?? new Dictionary<string, IOpenApiSchema>()).GroupBy(x => x.Key))
                    {
                        propertyAsJsonObject[group.Key] = MapPropertyAsJsonNode(group.First().Value);
                    }
                }

                return propertyAsJsonObject;

            default:
                return null;
        }
    }

    private JsonObject MapSchemaAllOfToObject(IOpenApiSchema schema)
    {
        var arrayItem = new JsonObject();
        foreach (var property in schema.AllOf ?? [])
        {
            foreach (var item in property.Properties ?? new Dictionary<string, IOpenApiSchema>())
            {
                arrayItem[item.Key] = MapPropertyAsJsonNode(item.Value);
            }
        }
        return arrayItem;
    }

    private JsonNode? MapPropertyAsJsonNode(IOpenApiSchema openApiSchema)
    {
        var schemaType = openApiSchema.GetSchemaType(out _);
        if (schemaType is JsonSchemaType.Object or JsonSchemaType.Array)
        {
            return MapSchemaToObject(openApiSchema);
        }

        return _exampleValueGenerator.GetExampleValue(openApiSchema);
    }

    private string MapPathWithParameters(string path, IEnumerable<IOpenApiParameter>? parameters)
    {
        if (parameters == null)
        {
            return path;
        }

        var newPath = path;
        foreach (var parameter in parameters)
        {
            var exampleMatcherModel = GetExampleMatcherModel(parameter.Schema, _settings.PathPatternToUse);
            newPath = newPath.Replace($"{{{parameter.Name}}}", exampleMatcherModel.Pattern as string);
        }

        return newPath;
    }

    private Dictionary<string, object>? MapHeaders(string? responseContentType, IDictionary<string, IOpenApiHeader>? headers)
    {
        var mappedHeaders = headers?
            .ToDictionary(item => item.Key, _ => GetExampleMatcherModel(null, _settings.HeaderPatternToUse).Pattern!) ?? [];

        if (!string.IsNullOrEmpty(responseContentType))
        {
            mappedHeaders.TryAdd(HeaderContentType, responseContentType);
        }

        return mappedHeaders.Count > 0 ? mappedHeaders : null;
    }

    private IList<ParamModel>? MapQueryParameters(IEnumerable<IOpenApiParameter> queryParameters)
    {
        var list = queryParameters
            .Where(req => req.Required)
            .Select(qp => new ParamModel
            {
                Name = qp.Name ?? string.Empty,
                IgnoreCase = _settings.QueryParameterPatternIgnoreCase,
                Matchers =
                [
                    GetExampleMatcherModel(qp.Schema, _settings.QueryParameterPatternToUse)
                ]
            })
            .ToList();

        return list.Any() ? list : null;
    }

    private IList<HeaderModel>? MapRequestHeaders(IEnumerable<IOpenApiParameter> headers)
    {
        var list = headers
            .Where(req => req.Required)
            .Select(qp => new HeaderModel
            {
                Name = qp.Name ?? string.Empty,
                IgnoreCase = _settings.HeaderPatternIgnoreCase,
                Matchers =
                [
                    GetExampleMatcherModel(qp.Schema, _settings.HeaderPatternToUse)
                ]
            })
            .ToList();

        return list.Any() ? list : null;
    }

    private MatcherModel GetExampleMatcherModel(IOpenApiSchema? schema, ExampleValueType type)
    {
        return type switch
        {
            ExampleValueType.Value => new MatcherModel
            {
                Name = "ExactMatcher",
                Pattern = GetExampleValueAsStringForSchemaType(schema),
                IgnoreCase = _settings.IgnoreCaseExampleValues
            },

            _ => new MatcherModel
            {
                Name = "WildcardMatcher",
                Pattern = "*"
            }
        };
    }

    private string GetExampleValueAsStringForSchemaType(IOpenApiSchema? schema)
    {
        var value = _exampleValueGenerator.GetExampleValue(schema);

        if (value.GetValueKind() == JsonValueKind.String)
        {
            return value.GetValue<string>();
        }

        return value.ToString();
    }

    private static string MapBasePath(IList<OpenApiServer>? servers)
    {
        var server = servers?.FirstOrDefault();
        if (server == null)
        {
            return string.Empty;
        }

        if (Uri.TryCreate(server.Url, UriKind.RelativeOrAbsolute, out var uriResult))
        {
            return uriResult.IsAbsoluteUri ? uriResult.AbsolutePath : uriResult.ToString();
        }

        return string.Empty;
    }
}