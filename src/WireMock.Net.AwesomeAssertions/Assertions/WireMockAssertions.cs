// Copyright © WireMock.Net

#pragma warning disable CS1591
using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

public partial class WireMockAssertions
{
    public const string Any = "*";

    public int? CallsCount { get; }
    public IReadOnlyList<IRequestMessage> RequestMessages { get; private set; }
    private readonly AssertionChain _chain;

    public WireMockAssertions(IWireMockServer subject, int? callsCount, AssertionChain chain)
    {
        CallsCount = callsCount;
        RequestMessages = subject.LogEntries.Select(logEntry => logEntry.RequestMessage).ToList();
        _chain = chain;
    }

    public (Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> Filter, Func<IReadOnlyList<IRequestMessage>, bool> Condition) BuildFilterAndCondition(Func<IRequestMessage, bool> predicate)
    {
        Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter = requests => requests.Where(predicate).ToList();

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