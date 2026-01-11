// Copyright © WireMock.Net

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AwesomeAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [Fact]
    public async Task WireMockServer_WithMultiPartBody_Using_MimePartMatchers()
    {
        // Arrange
        using var server = WireMockServer.Start();

        var textPlainContent = "This is some plain text";
        var textPlainContentType = "text/plain";
        var textPlainContentTypeMatcher = new ContentTypeMatcher(textPlainContentType);
        var textPlainContentMatcher = new ExactMatcher(textPlainContent);
        var textPlainMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textPlainContentTypeMatcher, null, null, textPlainContentMatcher);

        var textJson = "{ \"Key\" : \"Value\" }";
        var textJsonContentType = "applicatiom/json";
        var textJsonContentTypeMatcher = new ContentTypeMatcher(textJsonContentType);
        var textJsonContentMatcher = new JsonMatcher(new { Key = "Value" }, true);
        var jsonMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textJsonContentTypeMatcher, null, null, textJsonContentMatcher);

        var imagePngBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjlAAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC");
        var imagePngContentMatcher = new ExactObjectMatcher(imagePngBytes);
        var imagePngMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, null, null, null, imagePngContentMatcher);

        var matchers = new IMatcher[]
        {
            textPlainMatcher,
            jsonMatcher,
            imagePngMatcher
        };

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/multipart")
                .WithMultiPart(matchers)
            )
            .RespondWith(Response.Create()
                .WithBody("{{request.Method}};{{request.BodyAsMimeMessage.TextBody}}")
                .WithTransformer()
            );

        server
            .Given(Request.Create()
                .UsingPost()
                .WithPath("/multipart2")
                .WithMultiPart(matchers)
            )
            .RespondWith(Response.Create()
                .WithBody(request =>
                {
                    if (request.BodyAsMimeMessage == null)
                    {
                        throw new InvalidProgramException("Not expected");
                    }
                    return "OK";
                })
                .WithTransformer()
            );

        var formDataContent = new MultipartFormDataContent
        {
            { new StringContent(textPlainContent, Encoding.UTF8, textPlainContentType), "text" },
            { new StringContent(textJson, Encoding.UTF8, textJsonContentType), "json" }
        };

        var fileContent = new ByteArrayContent(imagePngBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        formDataContent.Add(fileContent, "somefile", "image.png");

        var client = server.CreateClient();

        // Act 1
        var response1 = await client.PostAsync("/multipart", formDataContent);

        // Assert 1
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var content1 = await response1.Content.ReadAsStringAsync();
        content1.Should().Be("POST;This is some plain text");

        // Act 2
        var response2 = await client.PostAsync("/multipart2", formDataContent);

        // Assert 1
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var content2 = await response2.Content.ReadAsStringAsync();
        content2.Should().Be("OK");
    }
}