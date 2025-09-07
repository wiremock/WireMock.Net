// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using WireMock.Types;
using WireMock.Util;

namespace WireMock;

/// <summary>
/// IRequestMessage
/// </summary>
public interface IRequestMessage
{
    /// <summary>
    /// Gets the Client IP Address.
    /// </summary>
    string ClientIP { get; }

    /// <summary>
    /// Gets the url (relative).
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets the AbsoluteUrl.
    /// </summary>
    string AbsoluteUrl { get; }

    /// <summary>
    /// The ProxyUrl (if a proxy is used).
    /// </summary>
    string? ProxyUrl { get; set; }

    /// <summary>
    /// Gets the DateTime.
    /// </summary>
    DateTime DateTime { get; }

    /// <summary>
    /// Gets the path (relative).
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the AbsolutePath.
    /// </summary>
    string AbsolutePath { get; }

    /// <summary>
    /// Gets the path segments.
    /// </summary>
    string[] PathSegments { get; }

    /// <summary>
    /// Gets the absolute path segments.
    /// </summary>
    string[] AbsolutePathSegments { get; }

    /// <summary>
    /// Gets the method.
    /// </summary>
    string Method { get; }

    /// <summary>
    /// Gets the HTTP Version.
    /// </summary>
    string HttpVersion { get; }

    /// <summary>
    /// Gets the headers.
    /// </summary>
    IDictionary<string, WireMockList<string>>? Headers { get; }

    /// <summary>
    /// Gets the cookies.
    /// </summary>
    IDictionary<string, string>? Cookies { get; }

    /// <summary>
    /// Gets the query.
    /// </summary>
    IDictionary<string, WireMockList<string>>? Query { get; }

    /// <summary>
    /// Gets the query.
    /// </summary>
    IDictionary<string, WireMockList<string>>? QueryIgnoreCase { get; }

    /// <summary>
    /// Gets the raw query.
    /// </summary>
    string RawQuery { get; }

    /// <summary>
    /// The body.
    /// </summary>
    IBodyData? BodyData { get; }

    /// <summary>
    /// The original body as string.
    /// Convenience getter for Handlebars and WireMockAssertions.
    /// </summary>
    string? Body { get; }

    /// <summary>
    /// The body (as JSON object).
    /// Convenience getter for Handlebars and WireMockAssertions.
    /// </summary>
    object? BodyAsJson { get; }

    /// <summary>
    /// The body (as bytearray).
    /// Convenience getter for Handlebars and WireMockAssertions.
    /// </summary>
    byte[]? BodyAsBytes { get; }

    /// <summary>
    /// The original body as MimeMessage.
    /// Convenience getter for Handlebars and WireMockAssertions.
    /// </summary>
    Models.Mime.IMimeMessageData? BodyAsMimeMessage { get; }

    /// <summary>
    /// The detected body type. Convenience getter for Handlebars.
    /// </summary>
    string? DetectedBodyType { get; }

    /// <summary>
    /// The detected body type from the Content-Type header. Convenience getter for Handlebars.
    /// </summary>
    string? DetectedBodyTypeFromContentType { get; }

    /// <summary>
    /// The detected compression from the Content-Encoding header. Convenience getter for Handlebars.
    /// </summary>
    string? DetectedCompression { get; }

    /// <summary>
    /// Gets the Host
    /// </summary>
    string Host { get; }

    /// <summary>
    /// Gets the protocol
    /// </summary>
    string Protocol { get; }

    /// <summary>
    /// Gets the port
    /// </summary>
    int Port { get; }

    /// <summary>
    /// Gets the origin
    /// </summary>
    string Origin { get; }

    /// <summary>
    /// Get a query parameter.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <returns>The query parameter value as WireMockList or null when not found.</returns>
    WireMockList<string>? GetParameter(string key, bool ignoreCase = false);

    /// <summary>
    /// Gets the connection's client certificate
    /// </summary>
    X509Certificate2? ClientCertificate { get; }
}