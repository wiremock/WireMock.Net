// Copyright © WireMock.Net

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using WireMock.Owin;

namespace WireMock.HttpsCertificate;

internal static class CertificateLoader
{
    private const string ExtensionPem = ".PEM";

    /// <summary>
    /// Used by the WireMock.Net server
    /// </summary>
    public static X509Certificate2 LoadCertificate(IWireMockMiddlewareOptions options, string host)
    {
        if (!string.IsNullOrEmpty(options.X509StoreName) && !string.IsNullOrEmpty(options.X509StoreLocation))
        {
            var thumbprintOrSubjectNameOrHost = options.X509ThumbprintOrSubjectName ?? host;

            var certStore = new X509Store((StoreName)Enum.Parse(typeof(StoreName), options.X509StoreName!), (StoreLocation)Enum.Parse(typeof(StoreLocation), options.X509StoreLocation!));
            try
            {
                certStore.Open(OpenFlags.ReadOnly);

                // Attempt to find by Thumbprint first
                var matchingCertificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprintOrSubjectNameOrHost, false);
                if (matchingCertificates.Count == 0)
                {
                    // Fallback to SubjectName
                    matchingCertificates = certStore.Certificates.Find(X509FindType.FindBySubjectName, thumbprintOrSubjectNameOrHost, false);
                    if (matchingCertificates.Count == 0)
                    {
                        // No certificates matched the search criteria.
                        throw new FileNotFoundException($"No Certificate found with in store '{options.X509StoreName}', location '{options.X509StoreLocation}' for Thumbprint or SubjectName '{thumbprintOrSubjectNameOrHost}'.");
                    }
                }

                // Use the first matching certificate.
                return matchingCertificates[0];
            }
            finally
            {
                certStore.Dispose();
                certStore.Close();
            }
        }

        if (!string.IsNullOrEmpty(options.X509CertificateFilePath))
        {
            if (options.X509CertificateFilePath.EndsWith(ExtensionPem, StringComparison.OrdinalIgnoreCase))
            {
                // PEM logic based on: https://www.scottbrady91.com/c-sharp/pem-loading-in-dotnet-core-and-dotnet
#if NET8_0_OR_GREATER
                if (!string.IsNullOrEmpty(options.X509CertificatePassword))
                {
                    var certPem = File.ReadAllText(options.X509CertificateFilePath);
                    var cert = X509Certificate2.CreateFromPem(certPem, options.X509CertificatePassword);
                    const string defaultPasswordPem = "WireMock.Net";

                    return new X509Certificate2(cert.Export(X509ContentType.Pfx, defaultPasswordPem), defaultPasswordPem);
                }
                return X509Certificate2.CreateFromPemFile(options.X509CertificateFilePath);
#else                
                throw new InvalidOperationException("Loading a PEM Certificate is only supported for .NET 8.0 and higher.");
#endif
            }

            return !string.IsNullOrEmpty(options.X509CertificatePassword) ?
                new X509Certificate2(options.X509CertificateFilePath, options.X509CertificatePassword) :
                new X509Certificate2(options.X509CertificateFilePath);
        }

        if (options.X509Certificate != null)
        {
            return options.X509Certificate;
        }

        throw new InvalidOperationException("X509StoreName and X509StoreLocation OR X509CertificateFilePath OR X509Certificate are mandatory. Note that X509CertificatePassword is optional.");
    }

    /// <summary>
    /// Used for Proxy
    /// </summary>
    public static X509Certificate2 LoadCertificate(string thumbprintOrSubjectName)
    {
        var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        try
        {
            // Certificate must be in the local machine store
            certStore.Open(OpenFlags.ReadOnly);

            // Attempt to find by Thumbprint first
            var matchingCertificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprintOrSubjectName, false);
            if (matchingCertificates.Count == 0)
            {
                // Fallback to SubjectName
                matchingCertificates = certStore.Certificates.Find(X509FindType.FindBySubjectName, thumbprintOrSubjectName, false);
                if (matchingCertificates.Count == 0)
                {
                    // No certificates matched the search criteria.
                    throw new FileNotFoundException("No certificate found with specified Thumbprint or SubjectName.", thumbprintOrSubjectName);
                }
            }

            // Use the first matching certificate.
            return matchingCertificates[0];
        }
        finally
        {
            certStore.Dispose();
            certStore.Close();
        }
    }
}