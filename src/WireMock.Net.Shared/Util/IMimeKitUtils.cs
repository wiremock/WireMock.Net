// Copyright © WireMock.Net

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
    /// Loads the <see cref="IMimeMessageData"/> from the stream.
    /// </summary>
    /// <param name="stream">The stream</param>
    IMimeMessageData LoadFromStream(Stream stream);

    /// <summary>
    /// Tries to get the <see cref="IMimeMessageData"/> from the request message.
    /// </summary>
    /// <param name="requestMessage">The request message.</param>
    /// <param name="mimeMessageData">A class MimeMessageDataWrapper which wraps a MimeKit.MimeMessage.</param>
    /// <returns><c>true</c> when parsed correctly, else <c>false</c></returns>
    bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out IMimeMessageData? mimeMessageData);
}