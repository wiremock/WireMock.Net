// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Extensions;
using WireMock.Pact.Models.V2;
using WireMock.Server;
using WireMock.Util;

namespace WireMock.Serialization;

internal static class PactMapper
{
    private const string DefaultMethod = "GET";
    private const int DefaultStatusCode = 200;
    private const string DefaultConsumer = "Default Consumer";
    private const string DefaultProvider = "Default Provider";

    public static (string FileName, byte[] Bytes) ToPact(WireMockServer server, string? filename = null)
    {
        var consumer = server.Consumer ?? DefaultConsumer;
        var provider = server.Provider ?? DefaultProvider;

        filename ??= $"{consumer} - {provider}.json";

        var pact = new Pact.Models.V2.Pact
        {
            Consumer = new Pacticipant { Name = consumer },
            Provider = new Pacticipant { Name = provider }
        };

        foreach (var mapping in server.MappingModels.OrderBy(m => m.Guid))
        {
            var path = mapping.Request.GetPathAsString();
            if (path == null)
            {
                // Path is null (probably a Func<>), skip this.
                continue;
            }

            var interaction = new Interaction
            {
                Description = !string.IsNullOrEmpty(mapping.Description) ? mapping.Description : mapping.Title ?? string.Empty,
                Request = MapRequest(mapping.Request, path),
                Response = MapResponse(mapping.Response)
            };

            pact.Interactions.Add(interaction);
        }

        return (filename, JsonUtils.SerializeAsPactFile(pact));
    }

    private static PactRequest MapRequest(RequestModel request, string path)
    {
        return new PactRequest
        {
            Method = request.Methods?.FirstOrDefault() ?? DefaultMethod,
            Path = path,
            Query = MapQueryParameters(request.Params),
            Headers = MapRequestHeaders(request.Headers),
            Body = MapBody(request.Body)
        };
    }

    private static PactResponse MapResponse(ResponseModel? response)
    {
        if (response == null)
        {
            return new PactResponse();
        }

        return new PactResponse
        {
            Status = MapStatusCode(response.StatusCode),
            Headers = MapResponseHeaders(response.Headers),
            Body = MapBody(response)
        };
    }

    private static object? MapBody(ResponseModel? response)
    {
        if (response?.BodyAsJson != null)
        {
            return response.BodyAsJson;
        }

        if (response?.Body != null)
        {
            return TryDeserializeJsonStringAsObject(response.Body);
        }

        return null;
    }

    private static int MapStatusCode(object? statusCode)
    {
        if (statusCode is string statusCodeAsString)
        {
            return int.TryParse(statusCodeAsString, out var statusCodeAsInt) ? statusCodeAsInt : DefaultStatusCode;
        }

        if (statusCode != null)
        {
            // Convert to Int32 because Newtonsoft deserializes an 'object' with a number value to a long.
            return Convert.ToInt32(statusCode);
        }

        return DefaultStatusCode;
    }

    private static string? MapQueryParameters(IList<ParamModel>? queryParameters)
    {
        if (queryParameters == null)
        {
            return null;
        }

        var values = queryParameters
            .Where(qp => qp.Matchers != null && qp.Matchers.Any() && qp.Matchers[0].Pattern is string)
            .Select(param => $"{Uri.EscapeDataString(param.Name)}={Uri.EscapeDataString((string)param.Matchers![0].Pattern!)}");

        return string.Join("&", values);
    }

    private static IDictionary<string, string>? MapRequestHeaders(IList<HeaderModel>? headers)
    {
        var validHeaders = headers?.Where(h => h.Matchers != null && h.Matchers.Any() && h.Matchers[0].Pattern is string);
        return validHeaders?.ToDictionary(x => x.Name, y => (string)y.Matchers![0].Pattern!);
    }

    private static IDictionary<string, string>? MapResponseHeaders(IDictionary<string, object>? headers)
    {
        var validHeaders = headers?.Where(h => h.Value is string);
        return validHeaders?.ToDictionary(x => x.Key, y => (string)y.Value);
    }

    private static object? MapBody(BodyModel? body)
    {
        return MapMatcherPattern(body?.Matcher ?? body?.Matchers?.FirstOrDefault());
    }

    private static object? MapMatcherPattern(MatcherModel? matcher)
    {
        var pattern = matcher?.Pattern ?? matcher?.Patterns?.FirstOrDefault();
        if (pattern is string patternAsString)
        {
            return TryDeserializeJsonStringAsObject(patternAsString);
        }

        return pattern;
    }

    /// <summary>
    /// In case it's a string, try to deserialize into object, else just return the string
    /// </summary>
    private static object? TryDeserializeJsonStringAsObject(string? value)
    {
        return value != null ? JsonUtils.TryDeserializeObject<object?>(value) ?? value : null;
    }

    //private static string GetPatternAsStringFromMatchers(MatcherModel[]? matchers, string defaultValue)
    //{
    //    if (matchers != null && matchers.Any() && matchers[0].Pattern is string patternAsString)
    //    {
    //        return patternAsString;
    //    }

    //    return defaultValue;
    //}
}