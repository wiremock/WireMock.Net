// Copyright © WireMock.Net

using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests.Pact;

public class PactTests
{
    [Fact]
    public async Task SavePact_Get_Request_And_Response_WithBodyAsJson()
    {
        var server = WireMockServer.Start();
        server
            .WithConsumer("Something API Consumer Get")
            .WithProvider("Something API")

            .Given(Request.Create()
                .UsingGet()
                .WithPath("/tester")
                .WithParam("q1", "test")
                .WithParam("q2", "ok")
                .WithHeader("Accept", "application/json")
            )
            .WithTitle("A GET request to retrieve the something")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBodyAsJson(new
                    {
                        Id = "tester",
                        FirstName = "Totally",
                        LastName = "Awesome"
                    })
            );

        var folder = Path.Combine("../../../", "Pact", "files");
        var file = "pact-get.json";
        var path = Path.Combine(folder, file);

        // Act
        server.SavePact(folder, file);

        // Assert
        await VerifyFile(path);
    }

    [Fact]
    public async Task SavePact_Post_Request_WithDescription()
    {
        var server = WireMockServer.Start();
        server
            .Given(Request.Create().UsingPost().WithPath("/tester"))
            .WithTitle("POST something")
            .WithDescription("A POST request to change something")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        var folder = Path.Combine("../../../", "Pact", "files");
        var file = "pact-post.json";
        var path = Path.Combine(folder, file);

        // Act
        server.SavePact(folder, file);

        // Assert
        await VerifyFile(path);
    }

    [Fact]
    public void SavePact_Get_Request_And_Response_WithBody_StringIsJson()
    {
        // Act
        var server = WireMockServer.Start();
        server
            .Given(Request.Create()
                .UsingGet()
                .WithPath("/tester")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody(@"{ foo: ""bar"" }")
            );

        var memoryStream = new MemoryStream();
        server.SavePact(memoryStream);

        var json = Encoding.UTF8.GetString(memoryStream.ToArray());
        var pact = JsonConvert.DeserializeObject<WireMock.Pact.Models.V2.Pact>(json)!;

        // Assert
        pact.Interactions.Should().HaveCount(1);

        var expectedBody = new JObject { { "foo", "bar" } };
        pact.Interactions[0].Response.Body.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public void SavePact_Get_Request_And_Response_WithBody_StringIsString()
    {
        // Act
        var server = WireMockServer.Start();
        server
            .Given(Request.Create()
                .UsingGet()
                .WithPath("/tester")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody("test")
            );

        var memoryStream = new MemoryStream();
        server.SavePact(memoryStream);

        var json = Encoding.UTF8.GetString(memoryStream.ToArray());
        var pact = JsonConvert.DeserializeObject<WireMock.Pact.Models.V2.Pact>(json)!;

        // Assert
        pact.Interactions.Should().HaveCount(1);
        pact.Interactions[0].Response.Body.Should().Be("test");
    }

    [Fact]
    public void SavePact_Get_Request_And_Response_WithNullBody()
    {
        // Act
        var server = WireMockServer.Start();
        server
            .Given(Request.Create()
                .UsingGet()
                .WithPath("/tester")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
            );

        var memoryStream = new MemoryStream();
        server.SavePact(memoryStream);

        var json = Encoding.UTF8.GetString(memoryStream.ToArray());
        var pact = JsonConvert.DeserializeObject<WireMock.Pact.Models.V2.Pact>(json)!;

        // Assert
        pact.Interactions.Should().HaveCount(1);
        pact.Interactions[0].Response.Body.Should().BeNull();
    }

    [Fact]
    public void SavePact_Post_Request_WithBody_JsonPartialMatcher()
    {
        // Act
        var server = WireMockServer.Start();
        server
            .Given(Request.Create()
                .UsingPost()
                .WithBody(new JsonPartialMatcher(@"{ ""name"": ""stef"" }"))
                .WithPath("/tester")
            )
            .RespondWith(Response.Create());

        var memoryStream = new MemoryStream();
        server.SavePact(memoryStream);

        var json = Encoding.UTF8.GetString(memoryStream.ToArray());
        var pact = JsonConvert.DeserializeObject<WireMock.Pact.Models.V2.Pact>(json)!;

        // Assert
        pact.Interactions.Should().HaveCount(1);

        var expectedBody = new JObject { { "name", "stef" } };
        pact.Interactions[0].Request.Body.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public void SavePact_Multiple_Requests()
    {
        var server = WireMockServer.Start();
        server
            .WithConsumer("Something API Consumer Multiple")
            .WithProvider("Something API")

            .Given(Request.Create()
                .UsingPost()
                .WithPath("/tester")
                .WithParam("q1", "test")
                .WithParam("q2", "ok")
                .WithHeader("Accept", "application/json")
            )
            .WithTitle("A GET request to retrieve the something")
            .WithGuid("23e2aedb-166c-467b-b9f6-9b0817cb1636")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBodyAsJson(new
                    {
                        Id = "tester",
                        FirstName = "Totally",
                        LastName = "Awesome"
                    })
            );

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/add")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher("{ \"Id\" : \"1\", \"FirstName\" : \"Totally\" }"))
            )
            .WithTitle("A Post request to add the something")
            .WithGuid("f3f8abe7-7d1e-4518-afa1-d295ce7dadfd")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.RedirectMethod)
                    .WithBodyAsJson(new
                    {
                        Id = "1",
                        FirstName = "Totally"
                    })
            );

        var folder = Path.Combine("../../../", "Pact", "files");
        var file = "pact-multiple.json";

        // Act
        server.SavePact(folder, file);

        // Assert
        File.ReadAllBytes(Path.Combine(folder, file)).Length.Should().BeGreaterThan(1);
    }
}