// Copyright © WireMock.Net

#if MIMEKIT
using System;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class MimePartMatcherTests
{
    private static readonly IMimeKitUtils MimeKitUtils = new MimeKitUtils();

    private const string TestMultiPart =
        """
        From:
        Date: Sun, 23 Jul 2023 16:13:13 +0200
        Subject:
        Message-Id: <HZ3K1HEAJKU4.IO57XCVO4BWV@desktop-6dd5qi2>
        MIME-Version: 1.0
        Content-Type: multipart/mixed; boundary="=-5XgmpXt0XOfzdtcgNJc2ZQ=="

        --=-5XgmpXt0XOfzdtcgNJc2ZQ==
        Content-Type: text/plain; charset=utf-8

        This is some plain text
        --=-5XgmpXt0XOfzdtcgNJc2ZQ==
        Content-Type: text/json; charset=utf-8

        {
            "Key": "Value"
        }
        --=-5XgmpXt0XOfzdtcgNJc2ZQ==
        Content-Type: image/png; name=image.png
        Content-Disposition: attachment; filename=image.png
        Content-Transfer-Encoding: base64

        iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjl
        AAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC

        --=-5XgmpXt0XOfzdtcgNJc2ZQ==-- 
        """;

    [Fact]
    public void MimePartMatcher_IsMatch_Part_TextPlain()
    {
        // Arrange
        var message = MimeKitUtils.LoadFromStream(StreamUtils.CreateStream(TestMultiPart));
        var part = message.BodyParts[0];

        // Act
        var contentTypeMatcher = new ContentTypeMatcher("text/plain");
        var contentMatcher = new ExactMatcher("This is some plain text");

        var matcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, contentTypeMatcher, null, null, contentMatcher);
        var result = matcher.IsMatch(part);

        // Assert
        matcher.Name.Should().Be("MimePartMatcher");
        result.Score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void MimePartMatcher_IsMatch_Part_TextJson()
    {
        // Arrange
        var message = MimeKitUtils.LoadFromStream(StreamUtils.CreateStream(TestMultiPart));
        var part = message.BodyParts[1];

        // Act
        var contentTypeMatcher = new ContentTypeMatcher("text/json");
        var contentMatcher = new JsonMatcher(new { Key = "Value" }, true);

        var matcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, contentTypeMatcher, null, null, contentMatcher);
        var result = matcher.IsMatch(part);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void MimePartMatcher_IsMatch_Part_ImagePng()
    {
        // Arrange
        var message = MimeKitUtils.LoadFromStream(StreamUtils.CreateStream(TestMultiPart));
        var part = message.BodyParts[2];

        // Act
        var contentTypeMatcher = new ContentTypeMatcher("image/png");
        var contentDispositionMatcher = new ExactMatcher("attachment; filename=\"image.png\"");
        var contentTransferEncodingMatcher = new ExactMatcher("base64");
        var contentMatcher = new ExactObjectMatcher(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjlAAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC"));

        var matcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, contentTypeMatcher, contentDispositionMatcher, contentTransferEncodingMatcher, contentMatcher);
        var result = matcher.IsMatch(part);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
    }
}
#endif