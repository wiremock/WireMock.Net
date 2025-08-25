// Copyright © WireMock.Net

using System.Security.Cryptography.X509Certificates;

namespace WireMock.Settings;

/// <summary>
/// HttpClientSettings
/// </summary>
public abstract class HttpClientSettings
{
    /// <summary>
    /// The clientCertificate thumbprint or subject name fragment to use.
    /// Example thumbprint : "D2DBF135A8D06ACCD0E1FAD9BFB28678DF7A9818". Example subject name: "www.google.com""
    /// </summary>
    public string? ClientX509Certificate2ThumbprintOrSubjectName { get; set; }

    /// <summary>
    /// Defines the WebProxySettings.
    /// </summary>
    public WebProxySettings? WebProxySettings { get; set; }

    /// <summary>
    /// Proxy requests should follow redirection (30x).
    /// </summary>
    public bool? AllowAutoRedirect { get; set; }

    /// <summary>
    /// The <see cref="X509Certificate2"/> to use.
    /// </summary>
    public X509Certificate2? Certificate { get; set; }
}