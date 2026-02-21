// Copyright © WireMock.Net

#pragma warning disable CS1591
using System.Collections.Generic;

namespace WireMock.Pact.Models.V2;

public class ProviderState
{
    public required string Name { get; set; }

    public IDictionary<string, string>? Params { get; set; }
}