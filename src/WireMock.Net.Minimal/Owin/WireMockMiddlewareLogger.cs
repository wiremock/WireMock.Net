// Copyright © WireMock.Net

using System.Diagnostics;
using WireMock.Logging;
using WireMock.Owin.ActivityTracing;
using WireMock.Serialization;
using WireMock.Util;

namespace WireMock.Owin;

internal class WireMockMiddlewareLogger(
    IWireMockMiddlewareOptions options,
    LogEntryMapper logEntryMapper,
    IGuidUtils guidUtils,
    IDateTimeUtils dateTimeUtils
) : IWireMockMiddlewareLogger
{
    public void LogRequestAndResponse(bool logRequest, RequestMessage request, IResponseMessage? response, MappingMatcherResult? match, MappingMatcherResult? partialMatch, Activity? activity)
    {
        var logEntry = new LogEntry
        {
            Guid = guidUtils.NewGuid(),
            RequestMessage = request,
            ResponseMessage = response,

            MappingGuid = match?.Mapping?.Guid,
            MappingTitle = match?.Mapping?.Title,
            RequestMatchResult = match?.RequestMatchResult,

            PartialMappingGuid = partialMatch?.Mapping?.Guid,
            PartialMappingTitle = partialMatch?.Mapping?.Title,
            PartialMatchResult = partialMatch?.RequestMatchResult
        };

        WireMockActivitySource.EnrichWithLogEntry(activity, logEntry, options.ActivityTracingOptions);
        activity?.Dispose();

        LogLogEntry(logEntry, logRequest);

        try
        {
            if (options.SaveUnmatchedRequests == true && match?.RequestMatchResult is not { IsPerfectMatch: true })
            {
                var filename = $"{logEntry.Guid}.LogEntry.json";
                options.FileSystemHandler?.WriteUnmatchedRequest(filename, options.DefaultJsonSerializer.Serialize(logEntry));
            }
        }
        catch
        {
            // Empty catch
        }
    }

    public void LogLogEntry(LogEntry entry, bool addRequest)
    {
        options.Logger.DebugRequestResponse(logEntryMapper.Map(entry), entry.RequestMessage?.Path.StartsWith("/__admin/") == true);

        // If addRequest is set to true and MaxRequestLogCount is null or does have a value greater than 0, try to add a new request log.
        if (addRequest && options.MaxRequestLogCount is null or > 0)
        {
            TryAddLogEntry(entry);
        }


        // In case MaxRequestLogCount has a value greater than 0, try to delete existing request logs based on the count.
        if (options.MaxRequestLogCount is > 0)
        {
            var logEntries = options.LogEntries.ToList();

            foreach (var logEntry in logEntries
                .OrderBy(le => le.RequestMessage?.DateTime ?? le.ResponseMessage?.DateTime)
                .Take(logEntries.Count - options.MaxRequestLogCount.Value))
            {
                TryRemoveLogEntry(logEntry);
            }
        }

        // In case RequestLogExpirationDuration has a value greater than 0, try to delete existing request logs based on the date.
        if (options.RequestLogExpirationDuration is > 0)
        {
            var logEntries = options.LogEntries.ToList();

            var checkTime = dateTimeUtils.UtcNow.AddHours(-options.RequestLogExpirationDuration.Value);
            foreach (var logEntry in logEntries.Where(le => le.RequestMessage?.DateTime < checkTime || le.ResponseMessage?.DateTime < checkTime))
            {
                TryRemoveLogEntry(logEntry);
            }
        }
    }

    private void TryAddLogEntry(LogEntry logEntry)
    {
        try
        {
            options.LogEntries.Add(logEntry);
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
            options.LogEntries.Remove(logEntry);
        }
        catch
        {
            // Ignore exception (can happen during stress testing)
        }
    }
}