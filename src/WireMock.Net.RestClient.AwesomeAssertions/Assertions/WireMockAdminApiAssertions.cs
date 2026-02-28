// Copyright © WireMock.Net

#pragma warning disable CS1591
using WireMock.Admin.Requests;
using WireMock.Matchers;

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

public partial class WireMockAdminApiAssertions(IWireMockAdminApi subject, int? callsCount, AssertionChain chain)
{
    public const string Any = "*";

    public int? CallsCount { get; } = callsCount;

    public IReadOnlyList<LogRequestModel> RequestMessages { get; private set; } = subject.GetRequestsAsync().GetAwaiter().GetResult()
            .Select(logEntry => logEntry.Request)
            .OfType<LogRequestModel>()
            .ToList();

    public (Func<IReadOnlyList<LogRequestModel>, IReadOnlyList<LogRequestModel>> Filter, Func<IReadOnlyList<LogRequestModel>, bool> Condition) BuildFilterAndCondition(Func<LogRequestModel, bool> predicate)
    {
        IReadOnlyList<LogRequestModel> filter(IReadOnlyList<LogRequestModel> requests) => requests.Where(predicate).ToList();

        return (filter, requests => (CallsCount is null && filter(requests).Any()) || CallsCount == filter(requests).Count);
    }

    public (Func<IReadOnlyList<LogRequestModel>, IReadOnlyList<LogRequestModel>> Filter, Func<IReadOnlyList<LogRequestModel>, bool> Condition) BuildFilterAndCondition(Func<LogRequestModel, string?> expression, IStringMatcher matcher)
    {
        return BuildFilterAndCondition(r => matcher.IsMatch(expression(r)).IsPerfect());
    }

    public (Func<IReadOnlyList<LogRequestModel>, IReadOnlyList<LogRequestModel>> Filter, Func<IReadOnlyList<LogRequestModel>, bool> Condition) BuildFilterAndCondition(Func<LogRequestModel, object?> expression, IObjectMatcher matcher)
    {
        return BuildFilterAndCondition(r => matcher.IsMatch(expression(r)).IsPerfect());
    }

    public void FilterRequestMessages(Func<IReadOnlyList<LogRequestModel>, IReadOnlyList<LogRequestModel>> filter)
    {
        RequestMessages = filter(RequestMessages).ToList();
    }
}
