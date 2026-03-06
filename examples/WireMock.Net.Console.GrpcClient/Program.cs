// Copyright © WireMock.Net

using Greet;
using Grpc.Net.Client;
using Policy2;

await TestPolicyAsync();
await TestGreeterAsync();
return;

async Task TestGreeterAsync()
{
    var channel = GrpcChannel.ForAddress("http://localhost:9093/grpc3", new GrpcChannelOptions
    {
        Credentials = Grpc.Core.ChannelCredentials.Insecure
    });

    var client = new Greeter.GreeterClient(channel);

    var reply = await client.SayHelloAsync(new HelloRequest { Name = "stef" });

    Console.WriteLine("Greeting: " + reply.Message);
}

async Task TestPolicyAsync()
{
    var channel = GrpcChannel.ForAddress("http://localhost:9093/grpc-policy", new GrpcChannelOptions
    {
        Credentials = Grpc.Core.ChannelCredentials.Insecure
    });

    var client = new PolicyService2.PolicyService2Client(channel);

    var reply = await client.GetCancellationDetailAsync(new GetCancellationDetailRequest
    {
        Client = new Client
        {
            CorrelationId = "abc"
        }
    });

    Console.WriteLine("PolicyService2:reply.CancellationName " + reply.CancellationName);
}