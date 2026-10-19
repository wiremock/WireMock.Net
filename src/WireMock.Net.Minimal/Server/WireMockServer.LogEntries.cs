// Copyright © WireMock.Net

using System.Collections.Specialized;
using System.Linq;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.Services;

namespace WireMock.Server;

public partial class WireMockServer
{
    /// <inheritdoc />
    [PublicAPI]
    public event NotifyCollectionChangedEventHandler LogEntriesChanged
    {
        add => _logEntriesChanged += value;
        remove => _logEntriesChanged -= value;
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<ILogEntry> LogEntries => _options.LogEntries.ToArray();

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<ILogEntry> FindLogEntries(params IRequestMatcher[] matchers)
    {
        Guard.NotNull(matchers);

        var results = new Dictionary<ILogEntry, RequestMatchResult>();

        var allLogEntries = LogEntries;
        foreach (var log in allLogEntries)
        {
            var requestMatchResult = new RequestMatchResult();

            if (log.RequestMessage != null)
            {
                foreach (var matcher in matchers)
                {
                    matcher.GetMatchingScore(log.RequestMessage, requestMatchResult);
                }
            }

            if (requestMatchResult.AverageTotalScore > MatchScores.AlmostPerfect)
            {
                results.Add(log, requestMatchResult);
            }
        }

        return results
            .OrderBy(x => x.Value)
            .Select(x => x.Key)
            .ToArray();
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<ILogEntry> FindUnmatchedLogEntries()
    {
        return LogEntries
            .Where(le => le.RequestMessage != null && !_options.AdminPaths.Includes(le.RequestMessage.Path))
            .Where(le => le.RequestMatchResult is not { IsPerfectMatch: true })
            .ToArray();
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<NearMiss> FindNearMisses(IRequestMessage requestMessage, int maxResults = DefaultMaxNearMisses)
    {
        Guard.NotNull(requestMessage);
        Guard.Condition(maxResults, m => m > 0);

        return FindNearMisses(requestMessage, null, maxResults);
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<NearMiss> FindNearMisses(ILogEntry logEntry, int maxResults = DefaultMaxNearMisses)
    {
        Guard.NotNull(logEntry);
        Guard.Condition(maxResults, m => m > 0);

        return logEntry.RequestMessage is null ? [] : FindNearMisses(logEntry.RequestMessage, logEntry.Guid, maxResults);
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<NearMiss> FindNearMissesForUnmatchedRequests(int maxResultsPerRequest = DefaultMaxNearMisses)
    {
        Guard.Condition(maxResultsPerRequest, m => m > 0);

        return FindUnmatchedLogEntries()
            .SelectMany(le => FindNearMisses(le, maxResultsPerRequest))
            .ToArray();
    }

    private const int DefaultMaxNearMisses = 3;

    private IMappingMatcher? _nearMissMappingMatcher;

    private IReadOnlyList<NearMiss> FindNearMisses(IRequestMessage requestMessage, Guid? logEntryGuid, int maxResults)
    {
        _nearMissMappingMatcher ??= new MappingMatcher(_options, new RandomizerDoubleBetween0And1());

        return _nearMissMappingMatcher
            .FindNearMisses(requestMessage, maxResults)
            .Select(nm => new NearMiss
            {
                RequestMessage = requestMessage,
                LogEntryGuid = logEntryGuid,
                MappingGuid = nm.Mapping.Guid,
                MappingTitle = nm.Mapping.Title,
                RequestMatchResult = nm.RequestMatchResult
            })
            .ToArray();
    }

    /// <inheritdoc cref="IWireMockServer.ResetLogEntries" />
    [PublicAPI]
    public void ResetLogEntries()
    {
        _options.LogEntries.Clear();
    }

    /// <inheritdoc cref="IWireMockServer.DeleteLogEntry" />
    [PublicAPI]
    public bool DeleteLogEntry(Guid guid)
    {
        // Check a LogEntry exists with the same GUID, if so, remove it.
        var existing = _options.LogEntries.ToList().FirstOrDefault(m => m.Guid == guid);
        if (existing != null)
        {
            _options.LogEntries.Remove(existing);
            return true;
        }

        return false;
    }

    private NotifyCollectionChangedEventHandler? _logEntriesChanged;

    private void LogEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_logEntriesChanged is { })
        {
            foreach (var handler in _logEntriesChanged.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, e);
                }
                catch (Exception exception)
                {
                    _options.Logger.Error("Error calling the LogEntriesChanged event handler: {0}", exception.Message);
                }
            }
        }
    }
}