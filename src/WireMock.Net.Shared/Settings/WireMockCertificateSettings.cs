// Copyright © WireMock.Net

using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;

namespace WireMock.Settings;

/// <summary>
/// If https is used, these settings can be used to configure the CertificateSettings in case a custom certificate instead the default .NET certificate should be used.
///
/// X509StoreName and X509StoreLocation should be defined
/// OR
/// X509CertificateFilePath should be defined
/// OR
/// X509Certificate should be defined
/// </summary>
public class WireMockCertificateSettings
{
    /// <summary>
    /// X.509 certificate StoreName (AddressBook, AuthRoot, CertificateAuthority, My, Root, TrustedPeople or TrustedPublisher)
    /// </summary>
    [PublicAPI]
    public string? X509StoreName { get; set; }

    /// <summary>
    /// X.509 certificate StoreLocation (CurrentUser or LocalMachine)
    /// </summary>
    [PublicAPI]
    public string? X509StoreLocation { get; set; }

    /// <summary>
    /// X.509 certificate Thumbprint or SubjectName (if not defined, the 'host' is used)
    /// </summary>
    [PublicAPI]
    public string? X509StoreThumbprintOrSubjectName { get; set; }

    /// <summary>
    /// X.509 certificate FilePath
    /// </summary>
    [PublicAPI]
    public string? X509CertificateFilePath { get; set; }

    /// <summary>
    /// A X.509 certificate instance.
    /// </summary>
    [PublicAPI]
    public X509Certificate2? X509Certificate { get; set; }

    /// <summary>
    /// X.509 certificate Password
    /// </summary>
    [PublicAPI]
    public string? X509CertificatePassword { get; set; }

    /// <summary>
    /// X509StoreName and X509StoreLocation should be defined
    /// OR
    /// X509CertificateFilePath should be defined
    /// OR
    /// X509Certificate should be defined
    /// </summary>
    [PublicAPI]
    public bool IsDefined =>
        !string.IsNullOrEmpty(X509StoreName) && !string.IsNullOrEmpty(X509StoreLocation) ||
        !string.IsNullOrEmpty(X509CertificateFilePath) ||
        X509Certificate != null;
}