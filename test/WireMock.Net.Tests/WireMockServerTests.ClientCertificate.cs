// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Net.Tests.Facts;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [IgnoreOnContinuousIntegrationFact]
    public async Task WireMockServer_WithRequiredClientCertificates_Should_Work_Correct()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            ClientCertificateMode = ClientCertificateMode.RequireCertificate,
            AcceptAnyClientCertificate = true,
            UseSSL = true,
        };

        using var server = WireMockServer.Start(settings);

        server.Given(Request.Create().WithPath("/*"))
            .RespondWith(Response.Create().WithCallback(message => new ResponseMessage
            {
                StatusCode = message.ClientCertificate?.Thumbprint == "2E32E3528C87046A95B8B0BA172A1597C3AF3A9D"
                    ? 200
                    : 403
            }));

        var certificates = new X509Certificate2Collection();
        certificates.Import("client_cert.pfx", "1234", X509KeyStorageFlags.Exportable);

        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };
        httpMessageHandler.ClientCertificates.AddRange(certificates);

        // Act
        var response = await new HttpClient(httpMessageHandler).GetAsync("https://localhost:" + server.Ports[0] + "/foo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}