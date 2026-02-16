// Copyright © WireMock.Net

using System.Diagnostics;
using WireMock.Logging;
using WireMock.Owin.ActivityTracing;
using WireMock.Serialization;
using WireMock.Util;

namespace WireMock.Owin;

internal class WireMockMiddlewareLogger(
    IWireMockMiddlewareOptions _options,
    LogEntryMapper _logEntryMapper,
    IGuidUtils _guidUtils
) : IWireMockMiddlewareLogger
{
    public void LogRequestAndResponse(bool logRequest, RequestMessage request, IResponseMessage? response, MappingMatcherResult? match, MappingMatcherResult? partialMatch, Activity? activity)
    {
        var logEntry = new LogEntry
        {
            Guid = _guidUtils.NewGuid(),
            RequestMessage = request,
            ResponseMessage = response,

            MappingGuid = match?.Mapping?.Guid,
            MappingTitle = match?.Mapping?.Title,
            RequestMatchResult = match?.RequestMatchResult,

            PartialMappingGuid = partialMatch?.Mapping?.Guid,
            PartialMappingTitle = partialMatch?.Mapping?.Title,
            PartialMatchResult = partialMatch?.RequestMatchResult
        };

        WireMockActivitySource.EnrichWithLogEntry(activity, logEntry, _options.ActivityTracingOptions);
        activity?.Dispose();

        LogLogEntry(logEntry, logRequest);

        try
        {
            if (_options.SaveUnmatchedRequests == true && match?.RequestMatchResult is not { IsPerfectMatch: true })
            {
                var filename = $"{logEntry.Guid}.LogEntry.json";
                _options.FileSystemHandler?.WriteUnmatchedRequest(filename, JsonUtils.Serialize(logEntry));
            }
        }
        catch
        {
            // Empty catch
        }
    }

    public void LogLogEntry(LogEntry entry, bool addRequest)
    {
        if (entry.RequestMessage != null)
        {
            _options.Logger.DebugRequestResponse(_logEntryMapper.Map(entry), entry.RequestMessage.Path.StartsWith("/__admin/"));

            // If addRequest is set to true and MaxRequestLogCount is null or does have a value greater than 0, try to add a new request log.
            if (addRequest && _options.MaxRequestLogCount is null or > 0)
            {
                TryAddLogEntry(entry);
            }
        }

        // In case MaxRequestLogCount has a value greater than 0, try to delete existing request logs based on the count.
        if (_options.MaxRequestLogCount is > 0)
        {
            var logEntries = _options.LogEntries.Where(le => le.RequestMessage != null).ToList();

            foreach (var logEntry in logEntries.OrderBy(le => le.RequestMessage!.DateTime).Take(logEntries.Count - _options.MaxRequestLogCount.Value))
            {
                TryRemoveLogEntry(logEntry);
            }
        }

        // In case RequestLogExpirationDuration has a value greater than 0, try to delete existing request logs based on the date.
        if (_options.RequestLogExpirationDuration is > 0)
        {
            var logEntries = _options.LogEntries.Where(le => le.RequestMessage != null).ToList();

            var checkTime = DateTime.UtcNow.AddHours(-_options.RequestLogExpirationDuration.Value);
            foreach (var logEntry in logEntries.Where(le => le.RequestMessage!.DateTime < checkTime))
            {
                TryRemoveLogEntry(logEntry);
            }
        }
    }

    private void TryAddLogEntry(LogEntry logEntry)
    {
        try
        {
            _options.LogEntries.Add(logEntry);
        }
        catch
        {
            // Ignore exception (can happen during stress testing)
        }
    }

    private void TryRemoveLogEntry(LogEntry logEntry)
    {
        try
        {
            _options.LogEntries.Remove(logEntry);
        }
        catch
        {
            // Ignore exception (can happen during stress testing)
        }
    }
}