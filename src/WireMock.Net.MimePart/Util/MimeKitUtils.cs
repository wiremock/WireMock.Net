// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using MimeKit;
using Stef.Validation;
using WireMock.Http;
using WireMock.Models;
using WireMock.Types;

namespace WireMock.Util;

internal class MimeKitUtils : IMimeKitUtils
{
    /// <inheritdoc />
    public IMimeMessageData LoadFromStream(Stream stream)
    {
        return new MimeMessageDataWrapper(MimeMessage.Load(Guard.NotNull(stream)));
    }

    /// <inheritdoc />
    public bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out IMimeMessageData? mimeMessageData)
    {
        Guard.NotNull(requestMessage);

        if (requestMessage.BodyData != null &&
            requestMessage.Headers?.TryGetValue(HttpKnownHeaderNames.ContentType, out var contentTypeHeader) == true &&
            StartsWithMultiPart(contentTypeHeader)
        )
        {
            var bytes = requestMessage.BodyData?.DetectedBodyType switch
            {
                // If the body is bytes, use the BodyAsBytes to match on.
                BodyType.Bytes => requestMessage.BodyData.BodyAsBytes!,

                // If the body is a String or MultiPart, use the BodyAsString to match on.
                BodyType.String or BodyType.MultiPart => Encoding.UTF8.GetBytes(requestMessage.BodyData.BodyAsString!),

                _ => throw new NotSupportedException()
            };

            var fixedBytes = FixBytes(bytes, contentTypeHeader[0]);

            mimeMessageData = LoadFromStream(new MemoryStream(fixedBytes));
            return true;
        }

        mimeMessageData = null;
        return false;
    }

    /// <inheritdoc />
    public IReadOnlyList<object> GetBodyParts(IMimeMessageData mimeMessageData)
    {
        if (mimeMessageData is not MimeMessageDataWrapper wrapper)
        {
            throw new ArgumentException($"The mimeMessageData must be of type {nameof(MimeMessageDataWrapper)}", nameof(mimeMessageData));
        }

        return wrapper.Message.BodyParts
            .OfType<MimePart>()
            .ToArray();
    }

    private static bool StartsWithMultiPart(WireMockList<string> contentTypeHeader)
    {
        return contentTypeHeader.Any(ct => ct.TrimStart().StartsWith("multipart/", StringComparison.OrdinalIgnoreCase));
    }

    private static byte[] FixBytes(byte[] bytes, WireMockList<string> contentType)
    {
        var contentTypeBytes = Encoding.UTF8.GetBytes($"{HttpKnownHeaderNames.ContentType}: {contentType}\r\n\r\n");

        var result = new byte[contentTypeBytes.Length + bytes.Length];

        Buffer.BlockCopy(contentTypeBytes, 0, result, 0, contentTypeBytes.Length);
        Buffer.BlockCopy(bytes, 0, result, contentTypeBytes.Length, bytes.Length);

        return result;
    }
}