// Copyright © WireMock.Net

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using MimeKit;
using Stef.Validation;
using WireMock.Http;
using WireMock.Models;
using WireMock.Models.Mime;
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
            byte[] bytes;

            switch (requestMessage.BodyData?.DetectedBodyType)
            {
                // If the body is bytes, use the BodyAsBytes to match on.
                case BodyType.Bytes:
                    bytes = requestMessage.BodyData.BodyAsBytes!;
                    break;

                // If the body is a String or MultiPart, use the BodyAsString to match on.
                case BodyType.String or BodyType.MultiPart:
                    bytes = Encoding.UTF8.GetBytes(requestMessage.BodyData.BodyAsString!);
                    break;

                // Else return false.
                default:
                    mimeMessageData = null;
                    return false;
            }

            var fixedBytes = PrependContentTypeHeader(bytes, contentTypeHeader[0]);

            mimeMessageData = LoadFromStream(new MemoryStream(fixedBytes));
            return true;
        }

        mimeMessageData = null;
        return false;
    }


    private static bool StartsWithMultiPart(WireMockList<string> contentTypeHeader)
    {
        return contentTypeHeader.Any(ct => ct.TrimStart().StartsWith("multipart/", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Prepends the Content-Type header to the byte array to make it a valid MIME message for MimeKit.
    /// </summary>
    private static byte[] PrependContentTypeHeader(byte[] bytes, WireMockList<string> contentType)
    {
        var contentTypeBytes = Encoding.UTF8.GetBytes($"{HttpKnownHeaderNames.ContentType}: {contentType}\r\n\r\n");

        var result = new byte[contentTypeBytes.Length + bytes.Length];

        Buffer.BlockCopy(contentTypeBytes, 0, result, 0, contentTypeBytes.Length);
        Buffer.BlockCopy(bytes, 0, result, contentTypeBytes.Length, bytes.Length);

        return result;
    }
}