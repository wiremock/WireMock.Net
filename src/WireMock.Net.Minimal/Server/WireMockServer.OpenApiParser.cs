// Copyright © WireMock.Net

using System.Net;
//#if OPENAPIPARSER
using System;
using System.Linq;
using WireMock.Net.OpenApiParser;
//#endif

namespace WireMock.Server;

public partial class WireMockServer
{
    private IResponseMessage OpenApiConvertToMappings(IRequestMessage requestMessage)
    {
//#if OPENAPIPARSER
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
//#else
//        return ResponseMessageBuilder.Create(HttpStatusCode.BadRequest, "Not supported for .NETStandard 1.3 and .NET 4.6.x or lower.");
//#endif
    }

    private IResponseMessage OpenApiSaveToMappings(IRequestMessage requestMessage)
    {
//#if OPENAPIPARSER
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
//#else
//        return ResponseMessageBuilder.Create(HttpStatusCode.BadRequest, "Not supported for .NETStandard 1.3 and .NET 4.6.x or lower.");
//#endif
    }
}