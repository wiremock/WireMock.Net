// Copyright © WireMock.Net

using System.Threading.Tasks;
//using System.Xml;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
using Moq;
using WireMock.Handlers;
using FluentAssertions;
//#if !NETSTANDARD1_3
//using Wmhelp.XPath2;
//#endif

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsXPathTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsXPathTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_XPath_SelectNode_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/xml")
            .WithBody("<response>{{XPath.SelectNode request.body \"/todo-list/todo-item[1]/text()\"}}</response>")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be("<response>abc</response>");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_XPath_SelectNode_Text_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/xml")
            .WithBody("{{XPath.SelectNode request.body \"/todo-list/todo-item[1]/text()\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsEqualTo("abc");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_XPath_SelectNodes_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/xml")
            .WithBody("<response>{{XPath.SelectNodes request.body \"/todo-list/todo-item\"}}</response>")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be("<response>abc,def,xyz</response>");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_XPath_SelectNode_Request_SoapXML_BodyAsString()
    {
        // Assign
        string soap = @"
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://www.Test.nl/XMLHeader/10"" xmlns:req=""http://www.Test.nl/Betalen/COU/Services/RdplDbTknLystByOvkLyst/8/Req"">
   <soapenv:Header>
      <ns:TestHeader>
         <ns:HeaderVersion>10</ns:HeaderVersion>
         <ns:MessageId>MsgId10</ns:MessageId>
         <ns:ServiceRequestorDomain>Betalen</ns:ServiceRequestorDomain>
         <ns:ServiceRequestorId>CRM</ns:ServiceRequestorId>
         <ns:ServiceProviderDomain>COU</ns:ServiceProviderDomain>
         <ns:ServiceId>RdplDbTknLystByOvkLyst</ns:ServiceId>
         <ns:ServiceVersion>8</ns:ServiceVersion>
         <ns:FaultIndication>N</ns:FaultIndication>
         <ns:MessageTimestamp>?</ns:MessageTimestamp>
      </ns:TestHeader>
   </soapenv:Header>
   <soapenv:Body>
      <req:RdplDbTknLystByOvkLyst_REQ>
         <req:AanleveraarCode>CRM</req:AanleveraarCode>
         <!--Optional:-->
         <req:AanleveraarDetail>CRMi</req:AanleveraarDetail>
         <req:BerichtId>BerId</req:BerichtId>
         <req:BerichtType>RdplDbTknLystByOvkLyst</req:BerichtType>
         <!--Optional:-->
         <req:OpgenomenBedragenGewenstIndicatie>N</req:OpgenomenBedragenGewenstIndicatie>
         <req:TokenIdLijst>
            <!--1 to 10 repetitions:-->
            <req:TokenId>0000083256</req:TokenId>
            <req:TokenId>0000083259</req:TokenId>
         </req:TokenIdLijst>
      </req:RdplDbTknLystByOvkLyst_REQ>
   </soapenv:Body>
</soapenv:Envelope>";
        var body = new BodyData
        {
            BodyAsString = soap,
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/xml")
            .WithBody("<response>{{XPath.SelectNode request.body \"//*[local-name()='TokenId']/text()\"}}</response>")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be("<response>0000083256</response>");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_XPath_Evaluate_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/xml")
            .WithBody("{{XPath.Evaluate request.body \"boolean(/todo-list[count(todo-item) = 3])\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsEqualIgnoringCase("True");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_XPath_Evaluate_Attribute_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/xml")
            .WithBody("{{XPath.Evaluate request.body \"string(/todo-list/todo-item[1]/@id)\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsEqualTo("a1");
    }
}