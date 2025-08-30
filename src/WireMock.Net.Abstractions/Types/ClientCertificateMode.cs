// Copyright © WireMock.Net

namespace WireMock.Types;

//#if NETSTANDARD1_3_OR_GREATER || NET461
/// <summary>
/// Describes the client certificate requirements for a HTTPS connection.
/// This enum is the same as https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.server.kestrel.https.clientcertificatemode
/// </summary>
public enum ClientCertificateMode
{
    /// <summary>
    /// A client certificate is not required and will not be requested from clients.
    /// </summary>
    NoCertificate,

    /// <summary>
    /// A client certificate will be requested; however, authentication will not fail if a certificate is not provided by the client.
    /// </summary>
    AllowCertificate,

    /// <summary>
    /// A client certificate will be requested, and the client must provide a valid certificate for authentication to succeed.
    /// </summary>
    RequireCertificate,

    /// <summary>
    /// A client certificate is not required and will not be requested from clients at the start of the connection.
    /// It may be requested by the application later.
    /// </summary>
    DelayCertificate,
}
//#endif