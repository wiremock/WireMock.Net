// Copyright © WireMock.Net

using System.Collections.Specialized;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;

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
            foreach (var matcher in matchers)
            {
                matcher.GetMatchingScore(log.RequestMessage, requestMatchResult);
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