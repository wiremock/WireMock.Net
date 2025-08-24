// Copyright © WireMock.Net

using System.Security.Cryptography.X509Certificates;
using WireMock.Settings;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The ProxyResponseBuilder interface.
/// </summary>
public interface IProxyResponseBuilder : IStatusCodeResponseBuilder
{
    /// <summary>
    /// WithProxy URL using Client X509Certificate2.
    /// </summary>
    /// <param name="proxyUrl">The proxy url.</param>
    /// <param name="clientX509Certificate2ThumbprintOrSubjectName">The X509Certificate2 file to use for client authentication.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithProxy(string proxyUrl, string? clientX509Certificate2ThumbprintOrSubjectName = null);

    /// <summary>
    /// WithProxy using <see cref="ProxyAndRecordSettings"/>.
    /// </summary>
    /// <param name="settings">The ProxyAndRecordSettings.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithProxy(ProxyAndRecordSettings settings);

    /// <summary>
    /// WithProxy using <see cref="X509Certificate2"/>.
    /// </summary>
    /// <param name="proxyUrl">The proxy url.</param>
    /// <param name="certificate">The X509Certificate2.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithProxy(string proxyUrl, X509Certificate2 certificate);
}