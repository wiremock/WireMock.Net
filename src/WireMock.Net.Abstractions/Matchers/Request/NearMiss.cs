// Copyright © WireMock.Net

namespace WireMock.Matchers.Request;

/// <summary>
/// A mapping which did not (fully) match a request, together with the match result explaining why.
/// </summary>
public class NearMiss
{
    /// <summary>
    /// The request which was evaluated against the mapping.
    /// </summary>
    public required IRequestMessage RequestMessage { get; init; }

    /// <summary>
    /// The unique identifier of the log entry for the request. [Optional]
    /// </summary>
    public Guid? LogEntryGuid { get; init; }

    /// <summary>
    /// The unique identifier of the mapping.
    /// </summary>
    public required Guid MappingGuid { get; init; }

    /// <summary>
    /// The title of the mapping. [Optional]
    /// </summary>
    public string? MappingTitle { get; init; }

    /// <summary>
    /// The match result, containing the score per matcher.
    /// </summary>
    public required IRequestMatchResult RequestMatchResult { get; init; }
}
