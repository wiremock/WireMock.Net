// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Util;
using OrgMapping = WireMock.Org.Abstractions.Mapping;

namespace WireMock.Server;

public partial class WireMockServer
{
    /// <summary>
    /// Read WireMock.org mapping json file.
    /// </summary>
    /// <param name="path">The path to the WireMock.org mapping json file.</param>
    [PublicAPI]
    public void ReadStaticWireMockOrgMappingAndAddOrUpdate(string path)
    {
        Guard.NotNull(path);

        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out var value))
        {
            var mappings = DeserializeJsonToArray<OrgMapping>(value);
            foreach (var mapping in mappings)
            {
                if (mappings.Length == 1 && Guid.TryParse(filenameWithoutExtension, out var guidFromFilename))
                {
                    ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mapping, guidFromFilename, path);
                }
                else
                {
                    ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mapping, null, path);
                }
            }
        }
    }

    private IResponseMessage MappingsPostWireMockOrg(HttpContext _, IRequestMessage requestMessage)
    {
        try
        {
            var mappingModels = DeserializeRequestMessageToArray<OrgMapping>(requestMessage);
            if (mappingModels.Length == 1)
            {
                var guid = ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mappingModels[0]);
                return ResponseMessageBuilder.Create(201, "Mapping added", guid);
            }

            foreach (var mappingModel in mappingModels)
            {
                ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mappingModel);
            }

            return ResponseMessageBuilder.Create(201, "Mappings added");
        }
        catch (ArgumentException a)
        {
            _settings.Logger.Error("HttpStatusCode set to 400 {0}", a);
            return ResponseMessageBuilder.Create(400, a.Message);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to 500 {0}", e);
            return ResponseMessageBuilder.Create(500, e.ToString());
        }
    }

    private Guid? ConvertWireMockOrgMappingAndRegisterAsRespondProvider(OrgMapping mapping, Guid? guid = null, string? path = null)
    {
        var requestBuilder = Request.Create();

        var request = mapping.Request;
        if (request != null)
        {
            if (request.Url != null)
            {
                requestBuilder = requestBuilder.WithUrl(request.Url);
            }
            else if (request.UrlPattern != null)
            {
                requestBuilder = requestBuilder.WithUrl(new RegexMatcher(request.UrlPattern));
            }
            else if (request.UrlPath != null)
            {
                requestBuilder = requestBuilder.WithPath(request.UrlPath);
            }
            else if (request.UrlPathPattern != null)
            {
                requestBuilder = requestBuilder.WithPath(new RegexMatcher(request.UrlPathPattern));
            }

            if (request.Method != null)
            {
                requestBuilder = requestBuilder.UsingMethod(request.Method);
            }

            /*
            "headers" : {
              "Accept" : {
                "contains" : "xml"
              }
            }
            */
            if (request.Headers is JObject headers)
            {
                ProcessWireMockOrgJObjectAndUseStringMatcher(headers, (key, match) =>
                {
                    requestBuilder = requestBuilder.WithHeader(key, match);
                });
            }

            if (request.Cookies is JObject cookies)
            {
                ProcessWireMockOrgJObjectAndUseStringMatcher(cookies, (key, match) =>
                {
                    requestBuilder = requestBuilder.WithCookie(key, match);
                });
            }

            /*
            "queryParameters" : {
              "search_term" : {
                "equalTo" : "WireMock"
              }
            }
            */
            if (request.QueryParameters is JObject queryParameters)
            {
                ProcessWireMockOrgJObjectAndUseStringMatcher(queryParameters, (key, match) =>
                {
                    requestBuilder = requestBuilder.WithParam(key, match);
                });
            }

            /*
             "bodyPatterns" : [ {
              "equalToJson" : "{ "cityName": "São Paulo", "cityCode": 5001 },
              "ignoreArrayOrder" : true,
              "ignoreExtraElements" : true
            } ]
            */
            if (request.BodyPatterns?.Any() == true)
            {
                var jObjectArray = request.BodyPatterns.Cast<JObject>();
                var bodyPattern = jObjectArray.First();
                ProcessWireMockOrgJObjectAndUseIMatcher(bodyPattern, match =>
                {
                    requestBuilder = requestBuilder.WithBody(match);
                });
            }
        }

        var responseBuilder = Response.Create();

        var response = mapping.Response;
        if (response != null)
        {
            responseBuilder = responseBuilder.WithStatusCode(response.Status);

            if (response.Headers is JObject responseHeaders)
            {
                var rb = responseBuilder;
                ProcessWireMockOrgJObjectAndConvertToIDictionary(responseHeaders, headers =>
                {
                    rb = rb.WithHeaders(headers);
                });
            }

            if (response.Transformers != null)
            {
                responseBuilder = responseBuilder.WithTransformer();
            }

            if (response.Body != null)
            {
                responseBuilder = responseBuilder.WithBody(response.Body);
            }

            if (response.JsonBody != null)
            {
                responseBuilder = responseBuilder.WithBodyAsJson(response.JsonBody);
            }

            if (response.Base64Body != null)
            {
                responseBuilder = responseBuilder.WithBody(Encoding.UTF8.GetString(Convert.FromBase64String(response.Base64Body)));
            }

            if (response.BodyFileName != null)
            {
                responseBuilder = responseBuilder.WithBodyFromFile(response.BodyFileName);
            }
        }

        var respondProvider = Given(requestBuilder);
        if (guid != null)
        {
            respondProvider = respondProvider.WithGuid(guid.Value);
        }
        else if (!string.IsNullOrEmpty(mapping.Uuid))
        {
            respondProvider = respondProvider.WithGuid(new Guid(mapping.Uuid));
        }

        if (mapping.Name != null)
        {
            respondProvider = respondProvider.WithTitle(mapping.Name);
        }

        if (path != null)
        {
            respondProvider = respondProvider.WithPath(path);
        }

        respondProvider.RespondWith(responseBuilder);

        return respondProvider.Guid;
    }

    private void ProcessWireMockOrgJObjectAndConvertToIDictionary(JObject items, Action<IDictionary<string, string>> action)
    {
        var dict = new Dictionary<string, string>();
        foreach (var item in items)
        {
            var key = item.Key;
            var valueAsString = item.Value?.Value<string>();
            if (valueAsString == null)
            {
                // Skip if the item.Value is null or when the string value is null
                continue;
            }

            dict.Add(key, valueAsString);
        }

        action(dict);
    }

    private void ProcessWireMockOrgJObjectAndUseStringMatcher(JObject items, Action<string, IStringMatcher> action)
    {
        foreach (var item in items)
        {
            var key = item.Key;
            var match = item.Value?.First as JProperty;
            if (match == null)
            {
                continue;
            }

            var valueAsString = match.Value.Value<string>();
            if (string.IsNullOrEmpty(valueAsString))
            {
                continue;
            }

            var matcher = ProcessAsStringMatcher(match, valueAsString!);
            if (matcher != null)
            {
                action(key, matcher);
            }
        }
    }

    private static void ProcessWireMockOrgJObjectAndUseIMatcher(JObject items, Action<IMatcher> action)
    {
        if (items.First is not JProperty firstItem)
        {
            return;
        }

        IMatcher? matcher;
        if (firstItem.Name == "equalToJson")
        {
            matcher = new JsonMatcher(firstItem.Value);
        }
        else
        {
            if ((firstItem.Value as JValue)?.Value is not string valueAsString)
            {
                return;
            }

            matcher = ProcessAsStringMatcher(firstItem, valueAsString);
        }

        if (matcher != null)
        {
            action(matcher);
        }
    }

    private static IStringMatcher? ProcessAsStringMatcher(JProperty match, string valueAsString)
    {
        return match.Name switch
        {
            "contains" => new WildcardMatcher(valueAsString),
            "matches" => new RegexMatcher(valueAsString),
            "equalTo" => new ExactMatcher(valueAsString),
            _ => null,
        };
    }
}