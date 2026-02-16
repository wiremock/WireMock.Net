// Copyright © WireMock.Net

using WireMock.Matchers.Request;

namespace WireMock.Logging;

/// <summary>
/// LogEntry
/// </summary>
public class LogEntry : ILogEntry
{
    /// <inheritdoc cref="ILogEntry.Guid" />
    public Guid Guid { get; set; }

    /// <inheritdoc cref="ILogEntry.RequestMessage" />
    public IRequestMessage? RequestMessage { get; set; }

    /// <inheritdoc cref="ILogEntry.ResponseMessage" />
    public IResponseMessage? ResponseMessage { get; set; }

    /// <inheritdoc cref="ILogEntry.RequestMatchResult" />
    public IRequestMatchResult? RequestMatchResult { get; set; }

    /// <inheritdoc cref="ILogEntry.MappingGuid" />
    public Guid? MappingGuid { get; set; }

    /// <inheritdoc cref="ILogEntry.MappingTitle" />
    public string? MappingTitle { get; set; }

    /// <inheritdoc cref="ILogEntry.PartialMappingGuid" />
    public Guid? PartialMappingGuid { get; set; }

    /// <inheritdoc cref="ILogEntry.PartialMappingTitle" />
    public string? PartialMappingTitle { get; set; }

    /// <inheritdoc cref="ILogEntry.PartialMatchResult" />
    public IRequestMatchResult? PartialMatchResult { get; set; }
}