// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using WireMock.Models.Mime;

namespace WireMock.Util;

/// <summary>
/// Defines the interface for MimeKitUtils.
/// </summary>
public interface IMimeKitUtils
{
    /// <summary>
    /// Loads the MimeKit.MimeMessage from the stream.
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <returns>MimeKit.MimeMessage</returns>
    IMimeMessageData LoadFromStream(Stream stream);

    /// <summary>
    /// Tries to get the MimeKit.MimeMessage from the request message.
    /// </summary>
    /// <param name="requestMessage">The request message.</param>
    /// <param name="mimeMessageData">A class MimeMessageDataWrapper which wraps a MimeKit.MimeMessage.</param>
    /// <returns><c>true</c> when parsed correctly, else <c>false</c></returns>
    bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out IMimeMessageData? mimeMessageData);

// Removed commented-out method signature and XML documentation for GetBodyParts.
}