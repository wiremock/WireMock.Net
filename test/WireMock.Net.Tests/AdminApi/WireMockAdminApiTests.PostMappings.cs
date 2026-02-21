// Copyright © WireMock.Net

using System.Net.Http;
using System.Text;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NFluent;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Constants;
using WireMock.Models;
using WireMock.Server;

namespace WireMock.Net.Tests.AdminApi;

public partial class WireMockAdminApiTests
{
    public static string RemoveLineContainingUpdatedAt(string text)
    {
        var lines = text.Split([Environment.NewLine], StringSplitOptions.None);
        var filteredLines = lines.Where(line => !line.Contains("\"UpdatedAt\": "));
        return string.Join(Environment.NewLine, filteredLines);
    }

    [Theory]
    [InlineData("protobuf-mapping-1.json", "351f0240-bba0-4bcb-93c6-1feba0fe0001")]
    [InlineData("protobuf-mapping-2.json", "351f0240-bba0-4bcb-93c6-1feba0fe0002")]
    [InlineData("protobuf-mapping-3.json", "351f0240-bba0-4bcb-93c6-1feba0fe0003")]
    [InlineData("protobuf-mapping-4.json", "351f0240-bba0-4bcb-93c6-1feba0fe0004")]
    public async Task HttpClient_PostMappingsAsync_ForProtoBufMapping(string mappingFile, string guid)
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var mappingsJson = ReadMappingFile(mappingFile);

        using var server = WireMockServer.StartWithAdminInterface();
        var httpClient = server.CreateClient();

        // Act
        var result = await httpClient.PostAsync("/__admin/mappings", new StringContent(mappingsJson, Encoding.UTF8, WireMockConstants.ContentTypeJson), cancelationToken);
        result.EnsureSuccessStatusCode();

        // Assert
        var mapping = await httpClient.GetStringAsync($"/__admin/mappings/{guid}", cancelationToken);
        mapping = RemoveLineContainingUpdatedAt(mapping);
        mapping.Should().Be(mappingsJson);
    }

    [Fact]
    public async Task IWireMockAdminApi_PostMappingsAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var model1 = new MappingModel
        {
            Request = new RequestModel { Path = "/1" },
            Response = new ResponseModel { Body = "txt 1" },
            Title = "test 1",
            Description = "description 1"
        };
        var model2 = new MappingModel
        {
            Request = new RequestModel { Path = "/2" },
            Response = new ResponseModel { Body = "txt 2" },
            Title = "test 2",
            Description = "description 2"
        };
        var result = await api.PostMappingsAsync([model1, model2], TestContext.Current.CancellationToken);

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result.Status).IsNotNull();
        Check.That(result.Guid).IsNull();
        Check.That(server.Mappings.Where(m => !m.IsAdminInterface)).HasSize(2);
        Check.That(server.Mappings.Single(x => x.Title == "test 1").Description).IsEqualTo("description 1");
        Check.That(server.Mappings.Single(x => x.Title == "test 2").Description).IsEqualTo("description 2");

        server.Stop();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(200, 200)]
    [InlineData("200", "200")]
    public async Task IWireMockAdminApi_PostMappingAsync_WithStatusCode(object? statusCode, object? expectedStatusCode)
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var model = new MappingModel
        {
            Request = new RequestModel { Path = "/1" },
            Response = new ResponseModel { Body = "txt", StatusCode = statusCode },
            Priority = 500,
            Title = "test"
        };
        var result = await api.PostMappingAsync(model, TestContext.Current.CancellationToken);

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result.Status).IsNotNull();
        Check.That(result.Guid).IsNotNull();

        var mapping = server.Mappings.Single(m => m.Priority == 500);
        Check.That(mapping).IsNotNull();
        Check.That(mapping.Title).Equals("test");

        var response = await mapping.ProvideResponseAsync(Mock.Of<HttpContext>(), new RequestMessage(new UrlDetails("http://localhost/1"), "GET", ""));
        Check.That(response.Message.StatusCode).Equals(expectedStatusCode);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_PostMappingsAsync_WithDuplicateGuids_Should_Return_400()
    {
        // Arrange
        var guid = Guid.Parse("1b731398-4a5b-457f-a6e3-d65e541c428f");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var model1WithGuid = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel { Path = "/1g" },
            Response = new ResponseModel { Body = "txt 1g" },
            Title = "test 1g"
        };
        var model2WithGuid = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel { Path = "/2g" },
            Response = new ResponseModel { Body = "txt 2g" },
            Title = "test 2g"
        };
        var model1 = new MappingModel
        {
            Request = new RequestModel { Path = "/1" },
            Response = new ResponseModel { Body = "txt 1" },
            Title = "test 1"
        };
        var model2 = new MappingModel
        {
            Request = new RequestModel { Path = "/2" },
            Response = new ResponseModel { Body = "txt 2" },
            Title = "test 2"
        };

        var models = new[]
        {
                model1WithGuid,
                model2WithGuid,
                model1,
                model2
            };

        var sutMethod = async () => await api.PostMappingsAsync(models);
        var exceptionAssertions = await sutMethod.Should().ThrowAsync<ApiException>();
        exceptionAssertions.Which.Content.Should().Be(@"{""Status"":""The following Guids are duplicate : '1b731398-4a5b-457f-a6e3-d65e541c428f' (Parameter 'mappingModels')""}");

        server.Stop();
    }
}