// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class Pact
{
    public required Pacticipant Consumer { get; set; }

    public required List<Interaction> Interactions { get; set; } = [];

    public Metadata? Metadata { get; set; }

    public required Pacticipant Provider { get; set; }
}