// Copyright © WireMock.Net

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
            if (options.X509CertificateFilePath!.EndsWith(ExtensionPem, StringComparison.OrdinalIgnoreCase))
            {
                // PEM logic based on: https://www.scottbrady91.com/c-sharp/pem-loading-in-dotnet-core-and-dotnet
#if NET8_0_OR_GREATER
                if (!string.IsNullOrEmpty(options.X509CertificatePassword))
                {
                    var certPem = File.ReadAllText(options.X509CertificateFilePath);
                    using var cert = X509Certificate2.CreateFromPem(certPem, options.X509CertificatePassword);
                    const string defaultPasswordPem = "WireMock.Net";

#if NET9_0_OR_GREATER
                    return X509CertificateLoader.LoadPkcs12(cert.Export(X509ContentType.Pfx, defaultPasswordPem), defaultPasswordPem);
#else
                    return new X509Certificate2(cert.Export(X509ContentType.Pfx, defaultPasswordPem), defaultPasswordPem);
#endif
                }
                return X509Certificate2.CreateFromPemFile(options.X509CertificateFilePath);
#else                
                throw new InvalidOperationException("Loading a PEM Certificate is only supported for .NET 8.0 and higher.");
#endif
            }

#if NET9_0_OR_GREATER
            var data = File.ReadAllBytes(options.X509CertificateFilePath);
            var contentType = X509Certificate2.GetCertContentType(data);
            if (contentType == X509ContentType.Pfx)
            {
                return X509CertificateLoader.LoadPkcs12(data, string.IsNullOrEmpty(options.X509CertificatePassword) ? null : options.X509CertificatePassword);
            }
            if (contentType == X509ContentType.Cert)
            {
                return X509CertificateLoader.LoadCertificate(data);
            }

            // The loader does not support legacy formats such as PKCS#7 and serialized certificates.
            #pragma warning disable SYSLIB0057
            return !string.IsNullOrEmpty(options.X509CertificatePassword) ?
                new X509Certificate2(data, options.X509CertificatePassword) :
                new X509Certificate2(data);
            #pragma warning restore SYSLIB0057
#else
            return !string.IsNullOrEmpty(options.X509CertificatePassword) ?
                new X509Certificate2(options.X509CertificateFilePath, options.X509CertificatePassword) :
                new X509Certificate2(options.X509CertificateFilePath);
#endif
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