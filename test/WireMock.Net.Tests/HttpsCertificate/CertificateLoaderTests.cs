// Copyright © WireMock.Net

#if NET8_0_OR_GREATER
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using WireMock.HttpsCertificate;
using WireMock.Owin;

namespace WireMock.Net.Tests.HttpsCertificate;

public class CertificateLoaderTests
{
    [Theory]
    [InlineData("pfx", null, true)]
    [InlineData("pfx", "password", true)]
    [InlineData("cer", null, false)]
    [InlineData("cer", "password", false)]
    [InlineData("pem", null, true)]
    [InlineData("pem", "private-key", true)]
    public void LoadCertificate_LoadsSupportedFormats(string format, string? password, bool hasPrivateKey)
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var original = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.{format}");

        try
        {
            if (format == "pem")
            {
                var pem = original.ExportCertificatePem();
                File.WriteAllText(path, password == null ? pem + "\n" + key.ExportPkcs8PrivateKeyPem() : pem);
            }
            else
            {
                var contentType = format == "pfx" ? X509ContentType.Pfx : X509ContentType.Cert;
                File.WriteAllBytes(path, original.Export(contentType, format == "pfx" ? password : null));
            }

            var options = new WireMockMiddlewareOptions
            {
                X509CertificateFilePath = path,
                X509CertificatePassword = format == "pem" && password != null ? key.ExportPkcs8PrivateKeyPem() : password
            };
            using var loaded = CertificateLoader.LoadCertificate(options, "localhost");

            loaded.Thumbprint.Should().Be(original.Thumbprint);
            loaded.HasPrivateKey.Should().Be(hasPrivateKey);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadCertificate_InvalidPfxPassword_Throws()
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var original = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pfx");

        try
        {
            File.WriteAllBytes(path, original.Export(X509ContentType.Pfx, "password"));
            var options = new WireMockMiddlewareOptions { X509CertificateFilePath = path, X509CertificatePassword = "wrong" };

            Action load = () => CertificateLoader.LoadCertificate(options, "localhost");
            load.Should().Throw<CryptographicException>();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void GetX509Certificate2_LoadsEmbeddedCertificate()
    {
        using var certificate = PublicCertificateHelper.GetX509Certificate2();

        certificate.HasPrivateKey.Should().BeTrue();
        certificate.Thumbprint.Should().NotBeNullOrEmpty();
    }
}
#endif