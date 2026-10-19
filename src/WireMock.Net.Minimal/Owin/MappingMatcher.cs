// Copyright © WireMock.Net

using System.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Services;

namespace WireMock.Owin;

internal class MappingMatcher(IWireMockMiddlewareOptions options, IRandomizerDoubleBetween0And1 randomizerDoubleBetween0And1) : IMappingMatcher
{
    private readonly IWireMockMiddlewareOptions _options = Guard.NotNull(options);
    private readonly IRandomizerDoubleBetween0And1 _randomizerDoubleBetween0And1 = Guard.NotNull(randomizerDoubleBetween0And1);

    public (MappingMatcherResult? Match, MappingMatcherResult? Partial) FindBestMatch(RequestMessage request)
    {
        Guard.NotNull(request);

        var possibleMappings = new List<MappingMatcherResult>();

        var mappings = _options.Mappings.Values
            .Where(m => !m.IsDisabled)
            .Where(m => m.TimeSettings.IsValid())
            .Where(m => m.Probability is null || _randomizerDoubleBetween0And1.Generate() <= m.Probability)
            .ToArray();

        foreach (var mapping in mappings)
        {
            try
            {
                var nextState = GetNextState(mapping);

                var mappingMatcherResult = new MappingMatcherResult(mapping, mapping.GetRequestMatchResult(request, nextState));

                var exceptions = mappingMatcherResult.RequestMatchResult.MatchDetails
                    .Where(md => md.Exception != null)
                    .Select(md => md.Exception!)
                    .ToArray();

                if (exceptions.Length == 0)
                {
                    possibleMappings.Add(mappingMatcherResult);
                }
                else if (!request.AbsolutePath.StartsWith("/__admin", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var ex in exceptions)
                    {
                        LogException(mapping, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(mapping, ex);
            }
        }

        var partialMatches = possibleMappings
            .Where(pm => (pm.Mapping.IsAdminInterface && pm.RequestMatchResult.IsPerfectMatch) || !pm.Mapping.IsAdminInterface)
            .OrderBy(m => m.RequestMatchResult)
                .ThenBy(m => m.RequestMatchResult.TotalNumber)
                .ThenBy(m => m.Mapping.Priority)
                .ThenByDescending(m => m.Mapping.Probability)
                .ThenByDescending(m => m.Mapping.UpdatedAt)
            .Where(pm => pm.RequestMatchResult.AverageTotalScore > 0.0)
            .ToArray();
        var partialMatch = partialMatches.FirstOrDefault();

        if (_options.AllowPartialMapping == true)
        {
            return (partialMatch, partialMatch);
        }

        var match = possibleMappings
            .Where(m => m.RequestMatchResult.IsPerfectMatch)
            .OrderBy(m => m.Mapping.Priority)
                .ThenBy(m => m.RequestMatchResult)
                .ThenBy(m => m.RequestMatchResult.TotalNumber)
                .ThenByDescending(m => m.Mapping.Probability)
                .ThenByDescending(m => m.Mapping.UpdatedAt)
            .FirstOrDefault();

        return (match, partialMatch);
    }

    public IReadOnlyList<MappingMatcherResult> FindNearMisses(IRequestMessage request, int maxResults)
    {
        Guard.NotNull(request);

        var nearMisses = new List<MappingMatcherResult>();

        // Probability is not taken into account here: a mapping which was skipped based on probability can still be a near miss.
        foreach (var mapping in _options.Mappings.Values.Where(m => !m.IsAdminInterface && !m.IsDisabled && m.TimeSettings.IsValid()))
        {
            try
            {
                var requestMatchResult = mapping.GetRequestMatchResult(request, GetNextState(mapping));
                if (requestMatchResult.MatchDetails.All(md => md.Exception == null))
                {
                    nearMisses.Add(new MappingMatcherResult(mapping, requestMatchResult));
                }
            }
            catch (Exception ex)
            {
                LogException(mapping, ex);
            }
        }

        // A near miss is a mapping which matches the request partially: perfect matches and complete mismatches are excluded.
        return nearMisses
            .Where(nm => !nm.RequestMatchResult.IsPerfectMatch && nm.RequestMatchResult.AverageTotalScore > 0.0)
            .OrderBy(nm => nm.RequestMatchResult)
                .ThenBy(nm => nm.Mapping.Priority)
                .ThenByDescending(nm => nm.Mapping.UpdatedAt)
            .Take(maxResults)
            .ToArray();
    }

    private void LogException(IMapping mapping, Exception ex)
    {
        _options.Logger.Error($"Getting a Request MatchResult for Mapping '{mapping.Guid}' failed. This mapping will not be evaluated. Exception: {ex}");
    }

    private string? GetNextState(IMapping mapping)
    {
        // If the mapping does not have a scenario or the store does not contain this scenario,
        // just return null to indicate that there is no next state.
        if (mapping.Scenario == null)
        {
            return null;
        }

        return _options.ScenarioStateStore.TryGet(mapping.Scenario, out var state) ? state.NextState : null;
    }
}