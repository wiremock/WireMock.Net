// Copyright © WireMock.Net

#pragma warning disable CS1591
using WireMock.Matchers;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

public partial class WireMockAssertions(IWireMockServer subject, int? callsCount)
{
    public const string Any = "*";

    public int? CallsCount { get; } = callsCount;
    public IReadOnlyList<IRequestMessage> RequestMessages { get; private set; } = subject.LogEntries.Select(logEntry => logEntry.RequestMessage).OfType<IRequestMessage>().ToList();

    public (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, bool> predicate)
    {
        IReadOnlyList<IRequestMessage> filter(IReadOnlyList<IRequestMessage> requests) => requests.Where(predicate).ToList();

        return (filter, requests => (CallsCount is null && filter(requests).Any()) || CallsCount == filter(requests).Count);
    }

    public (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, string?> expression, IStringMatcher matcher)
    {
        return BuildFilterAndCondition(r => matcher.IsMatch(expression(r)).IsPerfect());
    }

    public (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, object?> expression, IObjectMatcher matcher)
    {
        return BuildFilterAndCondition(r => matcher.IsMatch(expression(r)).IsPerfect());
    }

    public void FilterRequestMessages(Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter)
    {
        RequestMessages = filter(RequestMessages).ToList();
    }
}