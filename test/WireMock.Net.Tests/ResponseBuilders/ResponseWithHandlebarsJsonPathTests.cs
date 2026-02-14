// Copyright © WireMock.Net

using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsJsonPathTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsJsonPathTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Object_ResponseBodyAsJson()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"{
                  ""Stores"": [
                    ""Lambton Quay"",
                    ""Willis Street""
                  ],
                  ""Manufacturers"": [
                    {
                      ""Name"": ""Acme Co"",
                      ""Products"": [
                        {
                          ""Name"": ""Anvil"",
                          ""Price"": 50
                        }
                      ]
                    },
                    {
                      ""Name"": ""Contoso"",
                      ""Products"": [
                        {
                          ""Name"": ""Elbow Grease"",
                          ""Price"": 99.95
                        },
                        {
                          ""Name"": ""Headlight Fluid"",
                          ""Price"": 4
                        }
                      ]
                    }
                  ]
                }",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBodyAsJson(new { x = "{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}" })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        Check.That(j["x"]).IsNotNull();
        Check.That(j["x"]["Name"].ToString()).Equals("Acme Co");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Number_ResponseBodyAsJson()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "{ \"Price\": 99 }",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBodyAsJson(new { x = "{{JsonPath.SelectToken request.body \"..Price\"}}" })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson!);
        Check.That(j["x"].Value<long>()).Equals(99);
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"{
                  ""Stores"": [
                    ""Lambton Quay"",
                    ""Willis Street""
                  ],
                  ""Manufacturers"": [
                    {
                      ""Name"": ""Acme Co"",
                      ""Products"": [
                        {
                          ""Name"": ""Anvil"",
                          ""Price"": 50
                        }
                      ]
                    },
                    {
                      ""Name"": ""Contoso"",
                      ""Products"": [
                        {
                          ""Name"": ""Elbow Grease"",
                          ""Price"": 99.95
                        },
                        {
                          ""Name"": ""Headlight Fluid"",
                          ""Price"": 4
                        }
                      ]
                    }
                  ]
                }",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBody("{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).Equals($"{{{Environment.NewLine}  \"Name\": \"Acme Co\",{Environment.NewLine}  \"Products\": [{Environment.NewLine}    {{{Environment.NewLine}      \"Name\": \"Anvil\",{Environment.NewLine}      \"Price\": 50{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Request_BodyAsJObject()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ],
                  'Manufacturers': [
                    {
                      'Name': 'Acme Co',
                      'Products': [
                        {
                          'Name': 'Anvil',
                          'Price': 50
                        }
                      ]
                    },
                    {
                      'Name': 'Contoso',
                      'Products': [
                        {
                          'Name': 'Elbow Grease',
                          'Price': 99.95
                        },
                        {
                          'Name': 'Headlight Fluid',
                          'Price': 4
                        }
                      ]
                    }
                  ]
                }"),
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBody("{{JsonPath.SelectToken request.bodyAsJson \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).Equals($"{{{Environment.NewLine}  \"Name\": \"Acme Co\",{Environment.NewLine}  \"Products\": [{Environment.NewLine}    {{{Environment.NewLine}      \"Name\": \"Anvil\",{Environment.NewLine}      \"Price\": 50{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectTokens_Request_BodyAsString()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = @"{
                  ""Stores"": [
                    ""Lambton Quay"",
                    ""Willis Street""
                  ],
                  ""Manufacturers"": [
                    {
                      ""Name"": ""Acme Co"",
                      ""Products"": [
                        {
                          ""Name"": ""Anvil"",
                          ""Price"": 50
                        }
                      ]
                    },
                    {
                      ""Name"": ""Contoso"",
                      ""Products"": [
                        {
                          ""Name"": ""Elbow Grease"",
                          ""Price"": 99.95
                        },
                        {
                          ""Name"": ""Headlight Fluid"",
                          ""Price"": 4
                        }
                      ]
                    }
                  ]
                }",
            DetectedBodyType = BodyType.String
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBody("{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}}{{#each this}}%{{@index}}:{{this}}%{{/each}}{{/JsonPath.SelectTokens}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).Equals("%0:Anvil%%1:Elbow Grease%");
    }

    [Fact]
    public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectTokens_Request_BodyAsJObject()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ],
                  'Manufacturers': [
                    {
                      'Name': 'Acme Co',
                      'Products': [
                        {
                          'Name': 'Anvil',
                          'Price': 50
                        }
                      ]
                    },
                    {
                      'Name': 'Contoso',
                      'Products': [
                        {
                          'Name': 'Elbow Grease',
                          'Price': 99.95
                        },
                        {
                          'Name': 'Headlight Fluid',
                          'Price': 4
                        }
                      ]
                    }
                  ]
                }"),
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBody("{{#JsonPath.SelectTokens request.bodyAsJson \"$..Products[?(@.Price >= 50)].Name\"}}{{#each this}}%{{@index}}:{{this}}%{{/each}}{{/JsonPath.SelectTokens}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).Equals("%0:Anvil%%1:Elbow Grease%");
    }

    [Fact]
    public void Response_ProvideResponse_Handlebars_JsonPath_SelectTokens_Throws()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ]
                }"),
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBody("{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}}{{id}} {{value}},{{/JsonPath.SelectTokens}}")
            .WithTransformer();

        // Act
        Check.ThatCode(() => responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings)).Throws<ArgumentNullException>();
    }

    [Fact]
    public async Task Response_ProvideResponse_Transformer_WithBodyAsFile_JsonPath()
    {
        // Assign
        const string jsonString = "{ \"MyUniqueNumber\": \"1\" }";
        var bodyData = new BodyData
        {
            BodyAsString = jsonString,
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            DetectedBodyType = BodyType.Json,
            DetectedBodyTypeFromContentType = BodyType.Json,
            Encoding = Encoding.UTF8
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, bodyData);

        var responseBuilder = Response.Create()
            .WithTransformer()

            // We need to use `c:\\\\` here because when just using `c:\\\`, the `\\` it will be interpreted as an escape character to skip / exclude / escape the whole {{}} expression.
            // See https://handlebarsjs.com/guide/expressions.html#escaping-handlebars-expressions
            .WithBodyFromFile("c:\\\\{{JsonPath.SelectToken request.body \"$.MyUniqueNumber\" }}\\test.json");

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.BodyData?.BodyAsFile.Should().Be(@"c:\1\test.json");
    }
}