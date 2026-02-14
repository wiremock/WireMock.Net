// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using AwesomeAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithBodyFromFileTests
    {
        [Fact]
        public async Task Response_ProvideResponse_WithBodyFromFile_AbsolutePath()
        {
            // Arrange
            var server = WireMockServer.Start();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", "MyXmlResponse.xml");

            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(path)
                );

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");

            // Assert
            response.Should().Contain("<hello>world</hello>");

            server.Stop();
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBodyFromFile_InSubDirectory()
        {
            // Arrange
            var server = WireMockServer.Start();
            string path = @"subdirectory/MyXmlResponse.xml";

            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(path)
                );

            // Act
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");

            // Assert
            response.Should().Contain("<hello>world</hello>");

            server.Stop();
        }

        [Fact]
        public async Task Response_ProvideResponse_WithBodyFromFile_InAdminMappingFolder()
        {
            // Arrange
            var server = WireMockServer.Start();
            string path = @"MyXmlResponse.xml";

            server
                .Given(
                    Request
                        .Create()
                        .UsingGet()
                        .WithPath("/v1/content")
                )
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(path)
                );

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/v1/content");

            // Assert
            response.Should().Contain("<hello>world</hello>");

            server.Stop();
        }
    }
}