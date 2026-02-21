// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class PactRust
{
    public required string Ffi { get; set; }

    public required string Mockserver { get; set; }

    public required string Models { get; set; }
}