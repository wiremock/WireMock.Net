// Copyright © WireMock.Net

using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.MimePart;

// Test this CURL:
// curl -X POST http://localhost:9091/multipart -F "plainText=This is some plain text;type=text/plain" -F "jsonData={ `"Key`": `"Value`" };type=application/json" -F "image=@image.png;type=image/png"
//
// curl -X POST http://localhost:9091/multipart2 -F "plainText=This is some plain text;type=text/plain" -F "jsonData={ `"Key`": `"Value`" };type=application/json" -F "image=@image.png;type=image/png"

public static class MainApp
{
    public static async Task RunAsync()
    {
        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091,
            StartAdminInterface = true,

            ReadStaticMappings = true,
            //WatchStaticMappings = true,
            //WatchStaticMappingsInSubdirectories = true,

            Logger = new WireMockConsoleLogger()
        });
        System.Console.WriteLine("WireMockServer listening at {0}", string.Join(",", server.Urls));

        var textPlainContentTypeMatcher = new ContentTypeMatcher("text/plain");
        var textPlainContentMatcher = new ExactMatcher("This is some plain text");
        var textPlainMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textPlainContentTypeMatcher, null, null, textPlainContentMatcher);

        var textJsonContentTypeMatcher = new ContentTypeMatcher("application/json");
        var textJsonContentMatcher = new JsonMatcher(new { Key = "Value" }, true);
        var textJsonMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textJsonContentTypeMatcher, null, null, textJsonContentMatcher);

        var imagePngContentTypeMatcher = new ContentTypeMatcher("image/png");
        var imagePngContentDispositionMatcher = new ExactMatcher("form-data; name=\"image\"; filename=\"image.png\"");
        var imagePngContentTransferEncodingMatcher = new ExactMatcher("default");
        var imagePngContentMatcher = new ExactObjectMatcher(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjlAAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC"));
        var imagePngMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, imagePngContentTypeMatcher, imagePngContentDispositionMatcher, imagePngContentTransferEncodingMatcher, imagePngContentMatcher);

        var matchers = new IMatcher[]
        {
            textPlainMatcher,
            textJsonMatcher,
            imagePngMatcher
        };

        server
            .Given(Request.Create()
                .WithPath("/multipart")
                .UsingPost()
                .WithMultiPart(matchers)
            )
            .WithGuid("b9c82182-e469-41da-bcaf-b6e3157fefdb")
            .RespondWith(Response.Create()
                .WithBody("MultiPart is ok")
            );

        // server.SaveStaticMappings();

        System.Console.WriteLine(JsonConvert.SerializeObject(server.MappingModels, Formatting.Indented));

        System.Console.WriteLine("Press any key to stop the server");
        System.Console.ReadKey();
        server.Stop();

        System.Console.WriteLine("Displaying all requests");
        var allRequests = server.LogEntries;
        System.Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

        System.Console.WriteLine("Press any key to quit");
        System.Console.ReadKey();        
    }
}