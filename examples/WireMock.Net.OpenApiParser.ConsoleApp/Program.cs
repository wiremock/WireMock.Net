// Copyright © WireMock.Net

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace WireMock.Net.OpenApiParser.ConsoleApp;

class Program
{
    private const string Folder = "OpenApiFiles";
    static void Main(string[] args)
    {
        RunOthersOpenApiParserExample();

        //RunMockServerWithDynamicExampleGeneration();
    }

    private static void RunMockServerWithDynamicExampleGeneration()
    {
        // Run your mocking framework specifying your Example Values generator class.
        var serverCustomer_V2_json = Run.RunServer(Path.Combine(Folder, "Swagger_Customer_V2.0.json"), "http://localhost:8090/", true, new DynamicDataGeneration(), Types.ExampleValueType.Value, Types.ExampleValueType.Value);
        Console.WriteLine("Press any key to stop the servers");

        Console.ReadKey();
        serverCustomer_V2_json.Stop();
    }

    private static void RunOthersOpenApiParserExample()
    {
        var serverOpenAPIExamples = Run.RunServer(Path.Combine(Folder, "openAPIExamples.yaml"), "http://localhost:9091/");
        var serverPetstore_V2_json = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V2.0.json"), "http://localhost:9092/");
        var serverPetstore_V2_yaml = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V2.0.yaml"), "http://localhost:9093/");
        var serverPetstore_V300_yaml = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V3.0.0.yaml"), "http://localhost:9094/");
        var serverPetstore_V302_json = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V3.0.2.json"), "http://localhost:9095/");
        var testopenapifile_json = Run.RunServer(Path.Combine(Folder, "testopenapifile.json"), "http://localhost:9096/");
        var file_errorYaml = Run.RunServer(Path.Combine(Folder, "file_error.yaml"), "http://localhost:9097/");
        var file_petJson = Run.RunServer(Path.Combine(Folder, "pet.json"), "http://localhost:9098/");
        var refsYaml = Run.RunServer(Path.Combine(Folder, "refs.yaml"), "http://localhost:9099/");

        testopenapifile_json
            .Given(Request.Create().WithPath("/x").UsingGet())
            .WithTitle("t")
            .WithDescription("d")
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new
                {
                    result = "ok"
                })
            );

        testopenapifile_json
            .Given(Request.Create().WithPath("/y").UsingGet())
            .WithTitle("t2")
            .WithDescription("d2")
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new[] { "string-value"})
            );

        Console.WriteLine("Press any key to stop the servers");
        Console.ReadKey();

        serverOpenAPIExamples.Stop();
        serverPetstore_V2_json.Stop();
        serverPetstore_V2_yaml.Stop();
        serverPetstore_V300_yaml.Stop();
        serverPetstore_V302_json.Stop();
        testopenapifile_json.Stop();
        file_errorYaml.Stop();
        file_petJson.Stop();
        refsYaml.Stop();

        //IWireMockOpenApiParser parser = new WireMockOpenApiParser();

        //var petStoreModels = parser.FromStream(File.OpenRead("petstore-openapi3.json"), out OpenApiDiagnostic diagnostic1);
        //string petStoreJson = JsonConvert.SerializeObject(petStoreModels, Settings);
        // File.WriteAllText("../../../wiremock-petstore-openapi3.json", petStoreJson);

        //Run.RunServer(petStoreModels);

        //var mappingModels2 = parser.FromStream(File.OpenRead("infura.yaml"), out OpenApiDiagnostic diagnostic2);
        //Console.WriteLine(JsonConvert.SerializeObject(diagnostic2, Settings));

        //string json2 = JsonConvert.SerializeObject(mappingModels2, Settings);
        //Console.WriteLine(json2);
    }
}