// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class Metadata
{
    public required string PactSpecificationVersion { get; set; }

    public required PactSpecification PactSpecification { get; set; }
}