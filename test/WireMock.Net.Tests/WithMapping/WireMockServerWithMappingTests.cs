// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Admin.Mappings;
using WireMock.Server;

namespace WireMock.Net.Tests.WithMapping;

public class WireMockServerWithMappingTests
{
    [Fact]
    public void WireMockServer_WithMappingAsModel_Should_Add_Mapping()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var pattern = "hello wiremock";
        var path = "/foo";
        var response = "OK";
        var mapping = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel
            {
                Path = path,
                Body = new BodyModel
                {
                    Matcher = new MatcherModel
                    {
                        Name = "ExactMatcher",
                        Pattern = pattern
                    }
                }
            },
            Response = new ResponseModel
            {
                Body = response,
                StatusCode = 201
            }
        };

        var server = WireMockServer.Start();

        // Act
        server.WithMapping(mapping);

        // Assert
        server.MappingModels.Should().HaveCount(1).And.Contain(m =>
            m.Guid == guid &&
            //((PathModel)m.Request.Path).Matchers.OfType<WildcardMatcher>().First().GetPatterns().First() == "/foo*"
            // m.Request.Body.Matchers.OfType<ExactMatcher>().First().GetPatterns().First() == pattern &&
            m.Response.Body == response &&
            (int?)m.Response.StatusCode == 201
        );

        server.Stop();
    }

    [Fact]
    public void WireMockServer_WithMappingAsJson_Should_Add_Mapping()
    {
        // Arrange
        var mapping = @"{
                ""Guid"": ""532889c2-f84d-4dc8-b847-9ea2c6aca7d5"",
                ""Request"": {
                    ""Path"": ""/pet"",
                    ""Methods"": [
                        ""PUT""
                    ]
                },
                ""Response"": {
                    ""StatusCode"": 201,
                    ""BodyAsJson"": {
                        ""id"": 42,
                    },
                    ""Headers"": {
                        ""Content-Type"": ""application/json""
                    }
                }
            }";

        var server = WireMockServer.Start();

        // Act
        server.WithMapping(mapping);

        // Assert
        server.MappingModels.Should().HaveCount(1).And.Contain(m =>
            m.Guid == Guid.Parse("532889c2-f84d-4dc8-b847-9ea2c6aca7d5") &&
            (int?)m.Response.StatusCode == 201
        );

        server.Stop();
    }

    [Fact]
    public void WireMockServer_WithMappingsAsJson_Should_Add_Mapping()
    {
        // Arrange
        var mapping = @"[
            {
                ""Guid"": ""532889c2-f84d-4dc8-b847-9ea2c6aca7d1"",
                ""Request"": {
                    ""Path"": ""/pet1""
                },
                ""Response"": {
                    ""StatusCode"": 201
                }
            },
            {
                ""Guid"": ""532889c2-f84d-4dc8-b847-9ea2c6aca7d2"",
                ""Request"": {
                    ""Path"": ""/pet2""
                },
                ""Response"": {
                    ""StatusCode"": 202
                }
            }
            ]";

        var server = WireMockServer.Start();

        // Act
        server.WithMapping(mapping);

        // Assert
        server.MappingModels.Should().HaveCount(2);

        server.Stop();
    }
}