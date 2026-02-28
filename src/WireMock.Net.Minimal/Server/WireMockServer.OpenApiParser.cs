// Copyright © WireMock.Net

using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using WireMock.Net.OpenApiParser;

namespace WireMock.Server;

public partial class WireMockServer
{
    private IResponseMessage OpenApiConvertToMappings(HttpContext _, IRequestMessage requestMessage)
    {
        try
        {
            var mappingModels = new WireMockOpenApiParser().FromText(requestMessage.Body!, out var diagnostic);
            return diagnostic.Errors.Any() ? ToJson(diagnostic, false, HttpStatusCode.BadRequest) : ToJson(mappingModels);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to {0} {1}", HttpStatusCode.BadRequest, e);
            return ResponseMessageBuilder.Create(HttpStatusCode.BadRequest, e.Message);
        }
    }

    private IResponseMessage OpenApiSaveToMappings(HttpContext _, IRequestMessage requestMessage)
    {
        try
        {
            var mappingModels = new WireMockOpenApiParser().FromText(requestMessage.Body!, out var diagnostic);
            if (diagnostic.Errors.Any())
            {
                return ToJson(diagnostic, false, HttpStatusCode.BadRequest);
            }

            ConvertMappingsAndRegisterAsRespondProvider(mappingModels);

            return ResponseMessageBuilder.Create(HttpStatusCode.Created, "OpenApi document converted to Mappings");
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to {0} {1}", HttpStatusCode.BadRequest, e);
            return ResponseMessageBuilder.Create(HttpStatusCode.BadRequest, e.Message);
        }
    }
}