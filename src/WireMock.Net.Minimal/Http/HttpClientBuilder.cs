// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using WireMock.HttpsCertificate;
using WireMock.Settings;

namespace WireMock.Http;

internal static class HttpClientBuilder
{
    public static HttpClient Build(HttpClientSettings settings)
    {
#if NETSTANDARD || NETCOREAPP3_1 || NET5_0_OR_GREATER
        var handler = new HttpClientHandler
        {
            CheckCertificateRevocationList = false,
#if NET5_0_OR_GREATER
            SslProtocols = System.Security.Authentication.SslProtocols.Tls13 | System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls,
#else
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls,
#endif
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,

            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
#elif NET46
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
#else
        var handler = new WebRequestHandler
        {
            ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
#endif

        if (!string.IsNullOrEmpty(settings.ClientX509Certificate2ThumbprintOrSubjectName))
        {
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;

            var x509Certificate2 = CertificateLoader.LoadCertificate(settings.ClientX509Certificate2ThumbprintOrSubjectName!);
            handler.ClientCertificates.Add(x509Certificate2);
        }
        else if (settings.Certificate != null)
        {
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(settings.Certificate);
        }

        handler.AllowAutoRedirect = settings.AllowAutoRedirect == true;

        // If UseCookies enabled, httpClient ignores Cookie header
        handler.UseCookies = false;

        if (settings.WebProxySettings != null)
        {
            handler.UseProxy = true;

            handler.Proxy = new WebProxy(settings.WebProxySettings.Address);
            if (settings.WebProxySettings.UserName != null && settings.WebProxySettings.Password != null)
            {
                handler.Proxy.Credentials = new NetworkCredential(settings.WebProxySettings.UserName, settings.WebProxySettings.Password);
            }
        }

#if NET5_0_OR_GREATER
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => true;
#elif !NETSTANDARD1_3
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => true;
#endif

        return HttpClientFactory2.Create(handler);
    }
}