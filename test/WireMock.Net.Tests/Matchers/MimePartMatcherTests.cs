// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Util;

namespace WireMock.Net.Tests.Matchers;

public class MimePartMatcherTests
{
    private static readonly IMimeKitUtils MimeKitUtils = new MimeKitUtils();

    private const string TestMultiPart =
        """
        Content-Type: multipart/mixed; boundary=----MyBoundary123

        ------MyBoundary123
        Content-Type: text/plain
        Content-Disposition: form-data; name="textPart"

        This is some plain text.
        ------MyBoundary123
        Content-Type: application/json
        Content-Disposition: form-data; name="jsonPart"

        {
          "id": 42,
          "message": "Hello from JSON"
        }

        ------MyBoundary123
        Content-Type: image/png
        Content-Disposition: form-data; name="imagePart"; filename="example.png"
        Content-Transfer-Encoding: base64

        iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4
        //8wEzIABCMDgAEMwEAAAwAA//8DAKkCBf4AAAAASUVORK5CYII=

        ------MyBoundary123--
        """;

    [Fact]
    public void MimePartMatcher_IsMatch_Part_TextPlain()
    {
        // Arrange
        var message = MimeKitUtils.LoadFromStream(StreamUtils.CreateStream(TestMultiPart));
        var part = message.BodyParts[0];

        // Act
        var contentTypeMatcher = new ContentTypeMatcher("text/plain");
        var contentMatcher = new ExactMatcher("This is some plain text.");

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
        var contentTypeMatcher = new ContentTypeMatcher("application/json");
        var contentMatcher = new JsonPartialMatcher(new { id = 42 }, true);

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
        var contentDispositionMatcher = new WildcardMatcher("*filename=\"example.png\"");
        var contentTransferEncodingMatcher = new ExactMatcher("base64");
        var contentMatcher = new ExactObjectMatcher(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4\r\n//8wEzIABCMDgAEMwEAAAwAA//8DAKkCBf4AAAAASUVORK5CYII="));

        var matcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, contentTypeMatcher, contentDispositionMatcher, contentTransferEncodingMatcher, contentMatcher);
        var result = matcher.IsMatch(part);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
    }
}