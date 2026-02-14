// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests
{
	public partial class WireMockServerTests
	{
		[Theory]
		[InlineData(HttpStatusCode.Conflict)]
		[InlineData(409)]
		[InlineData("409")]
		public async Task WireMockServer_WithCallback_Should_Use_StatusCodeFromResponse(object statusCode)
		{
			// Arrange
			var server = WireMockServer.Start();
			server.Given(Request.Create().UsingPost().WithPath("/foo"))
				.RespondWith(Response.Create()
					.WithCallback(request => new ResponseMessage
					{
						StatusCode = statusCode
					}));

			// Act
			var httpClient = new HttpClient();
			var response = await httpClient.PostAsync("http://localhost:" + server.Ports[0] + "/foo", new StringContent("dummy"));

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            server.Stop();
		}
	}
}