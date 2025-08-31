// Copyright © WireMock.Net

using NFluent;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockServerProxy2Tests
    {
        [Fact]
        public async Task WireMockServer_ProxyAndRecordSettings_ShouldProxy()
        {
            // Assign
            var serverAsProxy = WireMockServer.Start();
            serverAsProxy.Given(Request.Create().UsingPost())
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));

            // Act
            var server = WireMockServer.Start();
            server.Given(Request.Create().UsingPost().WithHeader("prx", "1"))
                .RespondWith(Response.Create().WithProxy(serverAsProxy.Urls[0]));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{server.Urls[0]}/TST"),
                Content = new StringContent("test")
            };
            request.Headers.Add("prx", "1");

            // Assert
            var response = await new HttpClient().SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            Check.That(content).IsEqualTo("{\"p\":42}");
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
            Check.That(response.Content.Headers.GetValues("Content-Type").First()).IsEqualTo("application/json");

            server.Dispose();
            serverAsProxy.Dispose();
        }
    }
}