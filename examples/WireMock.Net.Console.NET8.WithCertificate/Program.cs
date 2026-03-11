// Copyright © WireMock.Net

using System.IO;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NET6.WithCertificate;

class Program
{
    static void Main(string[] args)
    {
        var serverEC = WireMockServer.Start(new WireMockServerSettings
        {
            Urls = new[] { "https://localhost:8433/" },
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
            CertificateSettings = new WireMockCertificateSettings
            {
                // https://www.scottbrady91.com/c-sharp/pem-loading-in-dotnet-core-and-dotnet
                // https://www.scottbrady91.com/openssl/creating-elliptical-curve-keys-using-openssl
                X509CertificateFilePath = "cert.pem",
                X509CertificatePassword = @"
-----BEGIN EC PRIVATE KEY-----
MHcCAQEEIJZTv6ujGrEwxW+ab1+CtZouRd8PK7PsklVMvJwm1uDmoAoGCCqGSM49
AwEHoUQDQgAE39VoI268uDuIeKmRzr9e9jgMSGeuJTvTG7+cSXmeDymrVgIGXQgm
qKA8TDXpJNrRhWMd/fpsnWu1JwJUjBmspQ==
-----END EC PRIVATE KEY-----"
            }
        });
        System.Console.WriteLine("WireMockServer listening at {0}", serverEC.Url);

        var serverRSA = WireMockServer.Start(new WireMockServerSettings
        {
            Urls = new[] { "https://localhost:8434/" },
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
            CertificateSettings = new WireMockCertificateSettings
            {
                // https://www.scottbrady91.com/c-sharp/pem-loading-in-dotnet-core-and-dotnet
                // https://www.scottbrady91.com/openssl/creating-rsa-keys-using-openssl
                X509CertificateFilePath = "cert-rsa.pem",
                X509CertificatePassword = File.ReadAllText("private-key-rsa.pem")
            }
        });
        System.Console.WriteLine("WireMockServer listening at {0}", serverRSA.Url);

        System.Console.WriteLine("Press any key to stop the server(s)");
        System.Console.ReadKey();
        serverEC.Stop();
        serverRSA.Stop();
    }
}