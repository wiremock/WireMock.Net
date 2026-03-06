// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class PactResponse
{
    public object? Body { get; set; }

    public IDictionary<string, string>? Headers { get; set; }

    public required int Status { get; set; }
}