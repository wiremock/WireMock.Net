// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class Interaction
{
    public required string Description { get; set; }

    public string? ProviderState { get; set; }

    public required PactRequest Request { get; set; }

    public required PactResponse Response { get; set; }
}