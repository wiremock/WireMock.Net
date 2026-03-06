// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using WireMock.HttpsCertificate;
using WireMock.Settings;

namespace WireMock.Http;

internal static class HttpClientBuilder
{
    public static HttpClient Build(HttpClientSettings settings)
    {
#if NET8_0_OR_GREATER

        var handler = new HttpClientHandler
        {
            CheckCertificateRevocationList = false,
#pragma warning disable SYSLIB0039 // Type or member is obsolete
            SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
#pragma warning restore SYSLIB0039 // Type or member is obsolete
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
#else
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
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

#if NET8_0_OR_GREATER
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
#else
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
#endif
        ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => true;

        return HttpClientFactory2.Create(handler);
    }
}