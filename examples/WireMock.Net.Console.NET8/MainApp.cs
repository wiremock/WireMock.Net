// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.ConsoleApplication
{
    public interface IHandleBarTransformer
    {
        string Name { get; }

        void Render(TextWriter textWriter, dynamic context, object[] arguments);
    }

    public class CustomNameTransformer : IHandleBarTransformer
    {
        public string Name => "CustomName";

        public void Render(TextWriter writer, dynamic context, object[] parameters)
        {
            /* Handlebar logic to render */
        }
    }

    public class Todo
    {
        public int Id { get; set; }
    }

    public static class MainApp
    {
        private const string ProtoDefinitionGreeter =
            """
            syntax = "proto3";

            package greet;

            service Greeter {
              rpc SayHello (HelloRequest) returns (HelloReply);
            }

            message HelloRequest {
              string name = 1;
            }

            message HelloReply {
              string message = 1;
            }

            """;

        private const string ProtoDefinitionPolicy =
            """
            syntax = "proto3";

            import "google/protobuf/timestamp.proto";

            // option csharp_namespace = "NarrowIntegrationTest.Lookup";

            package Policy2;

            service PolicyService2 {	
            	rpc GetCancellationDetail (GetCancellationDetailRequest) returns (GetCancellationDetailResponse);
            }

            message GetCancellationDetailRequest {
            	Client Client = 1;
            	LegacyPolicyKey LegacyPolicyKey = 2;
            }

            message GetCancellationDetailResponse {
            	ResponseStatus Status = 1;
            	string CancellationCode = 2;
            	string CancellationName = 3;
            	string CancellationDescription = 4;
            	google.protobuf.Timestamp CancellationEffDate = 5;
            	string NonRenewalCode = 6;
            	string NonRenewalName = 7;
            	string NonRenewalDescription = 8;
            	google.protobuf.Timestamp NonRenewalEffDate = 9;
            	google.protobuf.Timestamp LastReinstatementDate = 10; // Always send the last reinstatement date if present on the policy term.
            }

            message LegacyPolicyKey {
            	string Group = 1;
            	int32 UnitNumber = 2;
            	int32 Year = 3;
            	string Suffix = 4;
            }


            message ResponseStatus {
            	bool HasErrors = 1;
            	bool HasWarnings = 2;
            	repeated string Errors = 3;
            	repeated string Warnings = 4;
            	string CorrelationId = 5;
            }

            message Client {
            	string CorrelationId = 1;
            	enum Clients {
            		Unknown = 0;
                    QMS = 1;
                    BillingCenter = 2;
                    PAS = 3;
                    Payroll = 4;
                    Portal = 5;
                    SFO = 6;
                    QuoteAndBind = 7;
                    LegacyConversion = 8;
                    BindNow = 9;
            		PaymentPortal = 10 ;
            		PricingEngine = 11;
            	}
            	Clients ClientName = 2;
            }
            """;

        private const string TestSchemaQueryStudents =
            """
            type Query {
                students:[Student]
            }

            type Student {
                id:ID!
                firstName:String
                lastName:String
                fullName:String 
            }
            """;

        private const string TestSchemaQueryStudentById =
            """
            type Query {
                studentById(id:ID!):Student
            }

            type Student {
                id:ID!
                firstName:String
                lastName:String
                fullName:String 
            }
            """;

        private const string TestSchemaQueryGreeting =
            """
            type Query {
                greeting:String
            }
            """;

        private const string TestSchemaMutationMessage =
            """
            scalar DateTime
            scalar MyCustomScalar

            input MessageInput {
                content: String
                author: String
            }

            type Message {
                id: ID!
                content: String
                author: String
            }

            type Mutation {
                createMessage(input: MessageInput): Message
                createAnotherMessage(x: MyCustomScalar, dt: DateTime): Message
                updateMessage(id: ID!, input: MessageInput): Message
            }
            """;

        private static void RunSse()
        {
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = 9091,
                StartAdminInterface = true,
                Logger = new WireMockConsoleLogger()
            });
            server
                .WhenRequest(r => r
                    .UsingGet()
                    .WithPath("/sse")
                )
                .ThenRespondWith(r => r
                    .WithHeader("Content-Type", "text/event-stream")
                    .WithHeader("Cache-Control", "no-cache")
                    .WithHeader("Connection", "keep-alive")
                    .WithSseBody(async (_, q) =>
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            q.Write("test " + i + "\r\n");
                            await Task.Delay(5000);
                        }

                        q.Close();
                    })
                );

            server
                .WhenRequest(r => r
                    .UsingGet()
                )
                .ThenRespondWith(r => r
                    .WithBody("normal")
                );

            System.Console.ReadKey();
        }

        private static void RunOnLocal()
        {
            try
            {
                var server = WireMockServer.Start(new WireMockServerSettings
                {
                    Port = 9091,
                    StartAdminInterface = true,
                    Logger = new WireMockConsoleLogger()
                });
                System.Console.WriteLine(string.Join(", ", server.Urls));

                var requestJson = new { PricingContext = new { Market = "USA" } };
                var responseJson = new { Market = "{{JsonPath.SelectToken request.body \"$.PricingContext.Market\"}}" };
                server
                    .Given(Request.Create()
                        //.WithBody(new JsonMatcher(requestJson))
                        .WithBodyAsJson(requestJson)
                        .WithPath("/pricing")
                        .UsingPost()
                    )
                    .RespondWith(Response.Create()
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(responseJson)
                        .WithTransformer(true)
                    );

                System.Console.WriteLine("Press any key to stop...");
                System.Console.ReadKey();
                server.Stop();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        public static async Task RunAsync()
        {
            //RunSse();
            //RunOnLocal();

            var mappingBuilder = new MappingBuilder();
            mappingBuilder
                .Given(Request
                    .Create()
                    .WithPath(new WildcardMatcher("/param2", true))
                    .WithParam("key", "test")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "param2" }));

            var json = mappingBuilder.ToJson();
            System.Console.WriteLine("mappingBuilder : Json = {0}", json);

            var todos = new Dictionary<int, Todo>();

            var server = WireMockServer.Start();

            //server
            //    .Given(Request.Create()
            //        .WithPath("todos")
            //        .UsingGet()
            //    )
            //    .RespondWith(Response.Create()
            //        .WithBodyAsJson(todos.Values)
            //    );

            //server
            //    .Given(Request.Create()
            //        .UsingGet()
            //        .WithPath("todos")
            //        .WithParam("id")
            //    )
            //    .RespondWith(Response.Create()
            //        .WithBodyAsJson(rm => todos[int.Parse(rm.Query!["id"].ToString())])
            //    );

            var pX = 0.40;
            server
                .Given(Request.Create().UsingGet().WithPath("/p"))
                .WithProbability(pX)
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("X"));

            server
                .Given(Request.Create().UsingGet().WithPath("/p"))
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("default"));

            // Act
            var requestUri = new Uri($"http://localhost:{server.Port}/p");
            var c = server.CreateClient();
            var xCount = 0;
            var defaultCount = 0;
            var tot = 1000;
            for (var i = 0; i < tot; i++)
            {
                var response = await c.GetAsync(requestUri);
                var value = await response.Content.ReadAsStringAsync();
                if (value == "X")
                {
                    xCount++;
                }
                else if (value == "default")
                {
                    defaultCount++;
                }
            }
            System.Console.WriteLine("X = {0} ; default = {1} ; pX = {2} ; valueX = {3:0.00}", xCount, defaultCount, pX, 1.0 * xCount / tot);
            return;

            using var httpAndHttpsWithPort = WireMockServer.Start(new WireMockServerSettings
            {
                HostingScheme = HostingScheme.HttpAndHttps,
                Port = 12399
            });
            httpAndHttpsWithPort.Stop();

            using var httpAndHttpsFree = WireMockServer.Start(new WireMockServerSettings
            {
                HostingScheme = HostingScheme.HttpAndHttps
            });
            httpAndHttpsFree.Stop();

            string url1 = "http://localhost:9091/";
            string url2 = "http://localhost:9092/";
            string url3 = "https://localhost:9443/";
            string urlGrpc = "grpc://localhost:9093/";
            string urlGrpcSSL = "grpcs://localhost:9094/";

            server = WireMockServer.Start(new WireMockServerSettings
            {
                // CorsPolicyOptions = CorsPolicyOptions.AllowAll,
                AllowCSharpCodeMatcher = true,
                Urls = new[] { url1, url2, url3, urlGrpc, urlGrpcSSL },
                StartAdminInterface = true,
                ReadStaticMappings = true,
                SaveUnmatchedRequests = true,
                WatchStaticMappings = true,
                WatchStaticMappingsInSubdirectories = true,
                //ProxyAndRecordSettings = new ProxyAndRecordSettings
                //{
                //    SaveMapping = true
                //},
                PreWireMockMiddlewareInit = app => { System.Console.WriteLine($"PreWireMockMiddlewareInit : {app.GetType()}"); },
                PostWireMockMiddlewareInit = app => { System.Console.WriteLine($"PostWireMockMiddlewareInit : {app.GetType()}"); },

#if USE_ASPNETCORE
                AdditionalServiceRegistration = services => { System.Console.WriteLine($"AdditionalServiceRegistration : {services.GetType()}"); },
#endif
                Logger = new WireMockConsoleLogger(),

                HandlebarsRegistrationCallback = (handlebarsContext, fileSystemHandler) =>
                {
                    var transformer = new CustomNameTransformer();
                    // handlebarsContext.RegisterHelper(transformer.Name, transformer.Render); TODO
                },

                // Uncomment below if you want to use the CustomFileSystemFileHandler
                // FileSystemHandler = new CustomFileSystemFileHandler()
            });
            System.Console.WriteLine("WireMockServer listening at {0}", string.Join(",", server.Urls));

            server.SetBasicAuthentication("a", "b");
            //server.SetAzureADAuthentication(Environment.GetEnvironmentVariable("WIREMOCK_AAD_TENANT")!, "api://e083d51a-01a6-446c-8ad5-0c5c7f002208");

            //var http = new HttpClient();
            //var response = await http.GetAsync($"{_wireMockServer.Url}/pricing");
            //var value = await response.Content.ReadAsStringAsync();

#if PROTOBUF
            var protoBufJsonMatcher = new JsonPartialWildcardMatcher(new { name = "*" });
            server
                .Given(Request.Create()
                    .UsingPost()
                    .WithHttpVersion("2")
                    .WithPath("/grpc/greet.Greeter/SayHello")
                    .WithBodyAsProtoBuf(ProtoDefinitionGreeter, "greet.HelloRequest", protoBufJsonMatcher)
                )
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/grpc")
                    .WithBodyAsProtoBuf(ProtoDefinitionGreeter, "greet.HelloReply",
                        new
                        {
                            message = "hello {{request.BodyAsJson.name}}"
                        }
                    )
                    .WithTrailingHeader("grpc-status", "0")
                    .WithTransformer()
                );

            server
                .Given(Request.Create()
                    .UsingPost()
                    .WithHttpVersion("2")
                    .WithPath("/grpc2/greet.Greeter/SayHello")
                    .WithBodyAsProtoBuf("greet.HelloRequest", protoBufJsonMatcher)
                )
                .WithProtoDefinition(ProtoDefinitionGreeter)
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/grpc")
                    .WithBodyAsProtoBuf("greet.HelloReply",
                        new
                        {
                            message = "hello {{request.BodyAsJson.name}}"
                        }
                    )
                    .WithTrailingHeader("grpc-status", "0")
                    .WithTransformer()
                );

            server
                .AddProtoDefinition("my-greeter", ProtoDefinitionGreeter)
                .Given(Request.Create()
                    .UsingPost()
                    .WithPath("/grpc3/greet.Greeter/SayHello")
                    .WithBodyAsProtoBuf("greet.HelloRequest", protoBufJsonMatcher)
                )
                .WithProtoDefinition("my-greeter")
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/grpc")
                    .WithBodyAsProtoBuf("greet.HelloReply",
                        new
                        {
                            message = "hello {{request.BodyAsJson.name}}"
                        }
                    )
                    .WithTrailingHeader("grpc-status", "0")
                    .WithTransformer()
                );

            var protoBufJsonMatcherForGetCancellationDetailRequest = new JsonPartialWildcardMatcher("{\"Client\":{\"CorrelationId\":\"*\"}}", false, true);
            var getCancellationDetailResponseAsJsonObject = JsonConvert.DeserializeObject(
                """{"Status":{"HasErrors":false,"HasWarnings":false,"Errors":[],"Warnings":[],"CorrelationId":"b8ad0d04-ed2f-42e1-ac85-339d91dc9855"},"CancellationCode":"cc123","CancellationName":"cn123","CancellationDescription":"","CancellationEffDate":null,"NonRenewalCode":"","NonRenewalName":"","NonRenewalDescription":"","NonRenewalEffDate":null,"LastReinstatementDate":null}"""
            )!;
            server
                .AddProtoDefinition("grpc-policy", ProtoDefinitionPolicy)
                .Given(Request.Create()
                    .UsingPost()
                    .WithPath("/Policy2.PolicyService2/GetCancellationDetail")
                    .WithBodyAsProtoBuf("Policy2.GetCancellationDetailRequest", protoBufJsonMatcherForGetCancellationDetailRequest)
                )
                .WithProtoDefinition("grpc-policy")
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/grpc")
                    .WithBodyAsProtoBuf("Policy2.GetCancellationDetailResponse", getCancellationDetailResponseAsJsonObject)
                    .WithTrailingHeader("grpc-status", "0")
                    .WithTransformer()
                );
#endif

#if GRAPHQL
            var customScalars = new Dictionary<string, Type> { { "MyCustomScalar", typeof(int) } };
            server
                .Given(Request.Create()
                    .WithPath("/graphql")
                    .UsingPost()
                    .WithGraphQLSchema(TestSchemaQueryStudents)
                )
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(
                        """
                        {
                          "data": {
                            "students": [
                              {
                                "id": "1",
                                "firstName": "Alice",
                                "lastName": "Johnson",
                                "fullName": "Alice Johnson"
                              },
                              {
                                "id": "2",
                                "firstName": "Bob",
                                "lastName": "Smith",
                                "fullName": "Bob Smith"
                              }
                            ]
                          }
                        }
                        """)
                );

            server
                .Given(Request.Create()
                    .WithPath("/graphql")
                    .UsingPost()
                    .WithGraphQLSchema(TestSchemaQueryStudentById)
                    .WithBody(new JsonPartialWildcardMatcher("{ \"variables\": { \"sid\": \"1\" } }"))
                )
                .WithTitle("Student found")
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(
                        """
                        {
                          "data": {
                            "studentById": {
                              "id": "123",
                              "firstName": "John",
                              "lastName": "Doe",
                              "fullName": "John Doe"
                            }
                          }
                        }
                        """)
                );

            server
                .Given(Request.Create()
                    .WithPath("/graphql")
                    .UsingPost()
                    .WithGraphQLSchema(TestSchemaQueryStudentById)
                )
                .WithTitle("Student not found")
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(
                        """
                        {
                          "data": null
                        }
                        """)
                );
#endif

#if MIMEKIT
            var textPlainContentTypeMatcher = new ContentTypeMatcher("text/plain");
            var textPlainContentMatcher = new ExactMatcher("This is some plain text");
            var textPlainMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textPlainContentTypeMatcher, null, null, textPlainContentMatcher);

            var textJsonContentTypeMatcher = new ContentTypeMatcher("text/json");
            var textJsonContentMatcher = new JsonMatcher(new { Key = "Value" }, true);
            var textJsonMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textJsonContentTypeMatcher, null, null, textJsonContentMatcher);

            var imagePngContentTypeMatcher = new ContentTypeMatcher("image/png");
            var imagePngContentDispositionMatcher = new ExactMatcher("attachment; filename=\"image.png\"");
            var imagePngContentTransferEncodingMatcher = new ExactMatcher("base64");
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
#endif
            // 400 ms
            server
                .Given(Request.Create()
                    .WithPath("/slow/400")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(400)
                        .WithBody("return 400")
                        .WithHeader("Content-Type", "text/plain")
                );
            // 4 sec
            server
                .Given(Request.Create()
                    .WithPath("/slow/500")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(500)
                        .WithBody("return 500")
                        .WithHeader("Content-Type", "text/plain")
                );

            server
                .Given(Request.Create()
                    .UsingHead()
                    .WithPath("/cl")
                )
                .RespondWith(Response.Create()
                    .WithHeader("Content-Length", "42")
                );

            server
                .Given(Request.Create()
                    .UsingMethod("GET")
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                )
                .WithGuid("90356dba-b36c-469a-a17e-669cd84f1f05")
                .RespondWith(Response.Create()
                    .WithBody("Hello World")
                );

            server.Given(Request.Create().WithPath(MatchOperator.Or, "/mypath", "/mypath1", "/mypath2").UsingPost())
                .WithGuid("86984b0e-2516-4935-a2ef-b45bf4820d7d")
                .RespondWith(Response.Create()
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson("{{JsonPath.SelectToken request.body \"..name\"}}")
                        .WithTransformer()
            );

            server
                .Given(Request.Create().WithPath(p => p.Contains("x")).UsingGet())
                .AtPriority(4)
                .WithTitle("t")
                .WithDescription("d")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""Contains x with FUNC 200""}"));

            server
                .Given(Request.Create()
                    .UsingGet()
                    .WithPath("/proxy-test-keep-alive")
                )
                .RespondWith(Response.Create()
                    .WithHeader("Keep-Alive", "timeout=1, max=1")
                );

            server
                .Given(Request.Create()
                    .UsingPost()
                    .WithHeader("postmanecho", "post")
                )
                .RespondWith(Response.Create()
                    .WithProxy(new ProxyAndRecordSettings { Url = "http://postman-echo.com" })
                );

            server
                .Given(Request.Create()
                    .UsingGet()
                    .WithHeader("postmanecho", "get")
                )
                .RespondWith(Response.Create()
                    .WithProxy(new ProxyAndRecordSettings { Url = "http://postman-echo.com/get" })
                );

            server
                .Given(Request.Create()
                    .UsingGet()
                    .WithHeader("postmanecho", "get2")
                )
                .RespondWith(Response.Create()
                    .WithProxy(new ProxyAndRecordSettings
                    {
                        Url = "http://postman-echo.com/get",
                        WebProxySettings = new WebProxySettings
                        {
                            Address = "http://company",
                            UserName = "test",
                            Password = "pwd"
                        }
                    })
                );

            server
                .Given(Request.Create()
                    .UsingGet()
                    .WithPath("/proxy-execute-keep-alive")
                )
                .RespondWith(Response.Create()
                    .WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999", ExcludedHeaders = new[] { "Keep-Alive" } })
                    .WithHeader("Keep-Alive-Test", "stef")
                );

            server
                .Given(Request.Create()
                    .UsingGet()
                    .WithPath("/proxy-replace")
                )
                .RespondWith(Response.Create()
                    .WithProxy(new ProxyAndRecordSettings
                    {
                        Url = "http://localhost:9999",
                        ReplaceSettings = new ProxyUrlReplaceSettings
                        {
                            OldValue = "old",
                            NewValue = "new"
                        }
                    })
                );

            server
                .Given(Request.Create()
                    .WithPath("/xpath").UsingPost()
                    .WithBody(new XPathMatcher("/todo-list[count(todo-item) = 3]"))
                )
                .RespondWith(Response.Create().WithBody("XPathMatcher!"));

            server
                .Given(Request.Create()
                    .WithPath("/xpaths").UsingPost()
                    .WithBody(new[] { new XPathMatcher("/todo-list[count(todo-item) = 3]"), new XPathMatcher("/todo-list[count(todo-item) = 4]") })
                )
                .RespondWith(Response.Create().WithBody("xpaths!"));

            server
                .Given(Request
                    .Create()
                    .WithPath("/jsonthings")
                    .WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"))
                    .UsingPut())
                .RespondWith(Response.Create()
                    .WithBody(@"{ ""result"": ""JsonPathMatcher !!!""}"));

            server
                .Given(Request
                    .Create()
                    .WithPath("/jsonbodytest1")
                    .WithBody(new JsonMatcher("{ \"x\": 42, \"s\": \"s\" }"))
                    .UsingPost())
                .WithGuid("debaf408-3b23-4c04-9d18-ef1c020e79f2")
                .RespondWith(Response.Create()
                    .WithBody(@"{ ""result"": ""jsonbodytest1"" }"));

            server
                .Given(Request
                    .Create()
                    .WithPath("/jsonbodytest2")
                    .WithBody(new JsonMatcher(new { x = 42, s = "s" }))
                    .UsingPost())
                .WithGuid("debaf408-3b23-4c04-9d18-ef1c020e79f3")
                .RespondWith(Response.Create()
                    .WithBody(@"{ ""result"": ""jsonbodytest2"" }"));

            server
                .Given(Request
                    .Create()
                    .WithPath(new WildcardMatcher("/navision/OData/Company('My Company')/School*", true))
                    .WithParam("$filter", "(substringof(Code, 'WA')")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""odata""}"));

            server
                .Given(Request
                    .Create()
                    .WithPath(new WildcardMatcher("/param2", true))
                    .WithParam("key", "test")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "param2" }));

            server
                .Given(Request
                    .Create()
                    .WithPath(new WildcardMatcher("/param3", true))
                    .WithParam("key", new WildcardMatcher("t*"))
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "param3" }));

            server
                .Given(Request.Create().WithPath("/headers", "/headers_test").UsingPost().WithHeader("Content-Type", "application/json*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "data:headers posted with 201" }));

            if (!File.Exists(@"c:\temp\x.json"))
            {
                File.WriteAllText(@"c:\temp\x.json", "{ \"hello\": \"world\", \"answer\": 42 }");
            }

            server
                .Given(Request.Create().WithPath("/file").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyFromFile(@"c:\temp\x.json", false)
                );

            server
                .Given(Request.Create().WithPath("/filecache").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyFromFile(@"c:\temp\x.json")
                );

            server
                .Given(Request.Create().WithPath("/file_rel").UsingGet())
                .WithGuid("0000aaaa-fcf4-4256-a0d3-1c76e4862947")
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/xml")
                    .WithBodyFromFile("WireMock.Net.xml", false)
                );

            server
                .Given(Request.Create().WithHeader("ProxyThis", "true")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithProxy("http://www.google.com")
            );

            server
                .Given(Request.Create().WithHeader("ProxyThisHttps", "true")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithProxy("https://www.google.com")
            );

            server
                .Given(Request.Create().WithPath("/bodyasbytes.png")
                .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "image/png")
                    .WithBody(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAIAAAACUFjqAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTczbp9jAAAAJ0lEQVQoU2NgUPuPD6Hz0RCEAtJoiAxpCCBXGgmRIo0TofORkdp/AMiMdRVnV6O0AAAAAElFTkSuQmCC"))
                );

            server
                .Given(Request.Create().WithPath("/oauth2/access").UsingPost().WithBody("grant_type=password;username=u;password=p"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { access_token = "AT", refresh_token = "RT" }));

            server
                .Given(Request.Create().WithPath("/helloworld").UsingGet().WithHeader("Authorization", new RegexMatcher("^(?i)Bearer AT$")))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("hi"));

            server
                .Given(Request.Create().WithPath("/data").UsingPost().WithBody(b => b != null && b.Contains("e")))
                .AtPriority(999)
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "data posted with FUNC 201" }));

            server
                .Given(Request.Create().WithPath("/json").UsingPost().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]")))
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""json posted with 201""}"));

            server
                .Given(Request.Create().WithPath("/json2").UsingPost().WithBody("x"))
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""json posted with x - 201""}"));

            server
                .Given(Request.Create().WithPath("/data").UsingDelete())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""data deleted with 200""}"));

            server
                .Given(Request.Create()
                    .WithPath("/needs-a-key")
                    .UsingGet()
                    .WithHeader("api-key", "*", MatchBehaviour.AcceptOnMatch)
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody(@"{ ""result"": ""api-key found""}"));

            server
                .Given(Request.Create()
                    .WithPath("/needs-a-key")
                    .UsingGet()
                    .WithHeader("api-key", "*", MatchBehaviour.RejectOnMatch)
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.Unauthorized)
                    .WithBody(@"{ ""result"": ""api-key missing""}"));

            server
                .Given(Request.Create().WithPath("/nobody").UsingGet())
                .RespondWith(Response.Create().WithDelay(TimeSpan.FromSeconds(1))
                    .WithStatusCode(200));

            server
                .Given(Request.Create().WithPath("/partial").UsingPost().WithBody(new SimMetricsMatcher(new[] { "cat", "dog" })))
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("partial = 200"));

            // http://localhost:9091/trans?start=1000&stop=1&stop=2
            server
                .Given(Request.Create().WithPath("/trans").UsingGet())
                .WithGuid("90356dba-b36c-469a-a17e-669cd84f1f06")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Transformed-Postman-Token", "token is {{request.headers.Postman-Token}}")
                    .WithHeader("xyz_{{request.headers.Postman-Token}}", "token is {{request.headers.Postman-Token}}")
                    .WithBody(@"{""msg"": ""Hello world CATCH-ALL on /*, {{request.path}}, add={{Math.Add request.query.start.[0] 42}} bykey={{request.query.start}}, bykey={{request.query.stop}}, byidx0={{request.query.stop.[0]}}, byidx1={{request.query.stop.[1]}}"" }")
                    .WithTransformer(TransformerType.Handlebars, true, ReplaceNodeOptions.EvaluateAndTryToConvert)
                    .WithDelay(TimeSpan.FromMilliseconds(100))
                );

            server
                .Given(Request.Create().WithPath("/jsonpathtestToken").UsingPost())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
                    .WithTransformer()
                );

            server
                .Given(Request.Create().WithPath("/zubinix").UsingPost())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{ \"result\": \"{{JsonPath.SelectToken request.bodyAsJson \"username\"}}\" }")
                    .WithTransformer()
                );

            server
                .Given(Request.Create().WithPath("/zubinix2").UsingPost())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { path = "{{request.path}}", result = "{{JsonPath.SelectToken request.bodyAsJson \"username\"}}" })
                    .WithTransformer()
                );

            server
                .Given(Request.Create().WithPath("/jsonpathtestTokenJson").UsingPost())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { status = "OK", url = "{{request.url}}", transformed = "{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}" })
                    .WithTransformer()
                );

            server
                .Given(Request.Create().WithPath("/jsonpathtestTokens").UsingPost())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("[{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}} { \"idx\":{{id}}, \"value\":\"{{value}}\" }, {{/JsonPath.SelectTokens}} {} ]")
                    .WithTransformer()
                );

            server
                .Given(Request.Create()
                    .WithPath("/state1")
                    .UsingGet())
                .InScenario("s1")
                .WillSetStateTo("Test state 1")
                .RespondWith(Response.Create()
                    .WithBody("No state msg 1"));

            server
                .Given(Request.Create()
                    .WithPath("/foostate1")
                    .UsingGet())
                .InScenario("s1")
                .WhenStateIs("Test state 1")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg 1"));

            server
                .Given(Request.Create()
                    .WithPath("/state2")
                    .UsingGet())
                .InScenario("s2")
                .WillSetStateTo("Test state 2")
                .RespondWith(Response.Create()
                    .WithBody("No state msg 2"));

            server
                .Given(Request.Create()
                    .WithPath("/foostate2")
                    .UsingGet())
                .InScenario("s2")
                .WhenStateIs("Test state 2")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg 2"));

            server
                .Given(Request.Create().WithPath("/encoded-test/a%20b"))
                .RespondWith(Response.Create()
                    .WithBody("EncodedTest 1 : Path={{request.path}}, Url={{request.url}}")
                    .WithTransformer()
                );

            server
                .Given(Request.Create().WithPath("/encoded-test/a b"))
                .RespondWith(Response.Create()
                    .WithBody("EncodedTest 2 : Path={{request.path}}, Url={{request.url}}")
                    .WithTransformer()
                );

            // https://stackoverflow.com/questions/51985089/wiremock-request-matching-with-comparison-between-two-query-parameters
            //server
            //    .Given(Request.Create().WithPath("/linq")
            //        .WithParam("from", new LinqMatcher("DateTime.Parse(it) > \"2018-03-01 00:00:00\"")))
            //    .RespondWith(Response.Create()
            //        .WithBody("linq match !!!")
            //    );

            //server
            //    .Given(Request.Create().WithPath("/linq2")
            //        .WithBody(new LinqMatcher("it.applicationId != null"))
            //        .UsingPost()
            //    )
            //    .RespondWith(Response.Create()
            //        .WithBody("linq2 match !!!")
            //    );

            server
                .Given(Request.Create().WithPath("/myendpoint").UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithStatusCode(500)
                    .WithBody(requestMessage =>
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            Message = "Test error"
                        });
                    })
                );

            server
                .Given(Request.Create().WithPath("/random"))
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new
                    {
                        Xeger1 = "{{Xeger \"\\w{4}\\d{5}\"}}",
                        Xeger2 = "{{Xeger \"\\d{5}\"}}",
                        TextRegexPostcode = "{{Random Type=\"TextRegex\" Pattern=\"[1-9][0-9]{3}[A-Z]{2}\"}}",
                        Text = "{{Random Type=\"Text\" Min=8 Max=20}}",
                        TextLipsum = "{{Random Type=\"TextLipsum\"}}",
                        IBAN = "{{Random Type=\"IBAN\" CountryCode=\"NL\"}}",
                        TimeSpan1 = "{{Random Type=\"TimeSpan\" Format=\"c\" IncludeMilliseconds=false}}",
                        TimeSpan2 = "{{Random Type=\"TimeSpan\"}}",
                        DateTime1 = "{{Random Type=\"DateTime\"}}",
                        DateTimeNow = DateTime.Now,
                        DateTimeNowToString = DateTime.Now.ToString("s", CultureInfo.InvariantCulture),
                        Guid1 = "{{Random Type=\"Guid\" Uppercase=false}}",
                        Guid2 = "{{Random Type=\"Guid\"}}",
                        Guid3 = "{{Random Type=\"Guid\" Format=\"X\"}}",
                        Boolean = "{{Random Type=\"Boolean\"}}",
                        Integer = "{{Random Type=\"Integer\" Min=1000 Max=9999}}",
                        Long = "{{#Random Type=\"Long\" Min=10000000 Max=99999999}}{{this}}{{/Random}}",
                        Double = "{{Random Type=\"Double\" Min=10 Max=99}}",
                        Float = "{{Random Type=\"Float\" Min=100 Max=999}}",
                        IP4Address = "{{Random Type=\"IPv4Address\" Min=\"10.2.3.4\"}}",
                        IP6Address = "{{Random Type=\"IPv6Address\"}}",
                        MACAddress = "{{Random Type=\"MACAddress\" Separator=\"-\"}}",
                        StringListValue = "{{Random Type=\"StringList\" Values=[\"a\", \"b\", \"c\"]}}"
                    })
                    .WithTransformer()
                );

            server
                .Given(Request.Create()
                    .UsingPost()
                    .WithPath("/xpathsoap")
                    .WithBody(new XPathMatcher("//*[local-name() = 'getMyData']"))
                )
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/xml")
                    .WithBody("<xml>ok</xml>")
                );

            server
                .Given(Request.Create()
                    .UsingPost()
                    .WithPath("/post_with_query")
                    .WithHeader("PRIVATE-TOKEN", "t")
                    .WithParam("name", "stef")
                    .WithParam("path", "p")
                    .WithParam("visibility", "Private")
                    .WithParam("parent_id", "1")
                )
                .RespondWith(Response.Create()
                    .WithBody("OK : post_with_query")
                );

            server.Given(Request.Create()
                    .WithPath("/services/query/")
                    .WithParam("q", "SELECT Id from User where username='user@gmail.com'")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { Id = "5bdf076c-5654-4b3e-842c-7caf1fabf8c9" }));

            server
                .Given(Request.Create().WithPath("/random200or505").UsingGet())
                .RespondWith(Response.Create().WithCallback(request =>
                {
                    int code = new Random().Next(1, 2) == 1 ? 505 : 200;
                    return new ResponseMessage
                    {
                        BodyData = new BodyData
                        {
                            BodyAsString = "random200or505:" + code + ", HeadersFromRequest = " + string.Join(",", request.Headers),
                            DetectedBodyType = BodyType.String,
                        },
                        StatusCode = code
                    };
                }));

            server
                .Given(Request.Create().WithPath("/random200or505async").UsingGet())
                .RespondWith(Response.Create().WithCallback(async request =>
                {
                    await Task.Delay(1).ConfigureAwait(false);

                    int code = new Random().Next(1, 2) == 1 ? 505 : 200;

                    return new ResponseMessage
                    {
                        BodyData = new BodyData { BodyAsString = "random200or505async:" + code, DetectedBodyType = BodyType.String },
                        StatusCode = code
                    };
                }));

            server.Given(Request.Create().WithPath(new WildcardMatcher("/multi-webhook", true)).UsingPost())
                .WithWebhook
                (
                    new Webhook
                    {
                        Request = new WebhookRequest
                        {
                            Url = "http://localhost:12345/foo1",
                            Method = "post",
                            BodyData = new BodyData
                            {
                                BodyAsString = "OK 1!",
                                DetectedBodyType = BodyType.String
                            },
                            Delay = 1000
                        }
                    },
                    new Webhook
                    {
                        Request = new WebhookRequest
                        {
                            Url = "http://localhost:12345/foo2",
                            Method = "post",
                            BodyData = new BodyData
                            {
                                BodyAsString = "OK 2!",
                                DetectedBodyType = BodyType.String
                            },
                            MinimumRandomDelay = 3000,
                            MaximumRandomDelay = 7000
                        }
                    }
                )
                .WithWebhookFireAndForget(true)
                .RespondWith(Response.Create().WithBody("a-response"));

            System.Console.WriteLine(JsonConvert.SerializeObject(server.MappingModels, Formatting.Indented));

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();

            System.Console.WriteLine("Displaying all requests");
            var allRequests = server.LogEntries;
            System.Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

            System.Console.WriteLine("Press any key to quit");
            System.Console.ReadKey();

            server.Stop();
            server.Dispose();
        }
    }
}