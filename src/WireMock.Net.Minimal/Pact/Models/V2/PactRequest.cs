// Copyright © WireMock.Net

#pragma warning disable CS1591
using WireMock.Constants;

namespace WireMock.Pact.Models.V2;

public class PactRequest
{
    public IDictionary<string, string>? Headers { get; set; }

    public required string Method { get; set; } = HttpRequestMethod.GET;

    public required string Path { get; set; } = "/";

    public string? Query { get; set; }

    public object? Body { get; set; }
}