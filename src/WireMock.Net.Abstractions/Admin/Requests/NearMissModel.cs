// Copyright © WireMock.Net

namespace WireMock.Admin.Requests;

/// <summary>
/// NearMiss Model: a mapping which did not (fully) match a request.
/// </summary>
public class NearMissModel
{
    /// <summary>
    /// The unique identifier of the log entry for the request. [Optional]
    /// </summary>
    public Guid? LogEntryGuid { get; set; }

    /// <summary>
    /// The request.
    /// </summary>
    public LogRequestModel? Request { get; set; }

    /// <summary>
    /// The unique identifier of the mapping.
    /// </summary>
    public Guid MappingGuid { get; set; }

    /// <summary>
    /// The title of the mapping.
    /// </summary>
    public string? MappingTitle { get; set; }

    /// <summary>
    /// The request match result, containing the score per matcher.
    /// </summary>
    public LogRequestMatchModel? RequestMatchResult { get; set; }
}
