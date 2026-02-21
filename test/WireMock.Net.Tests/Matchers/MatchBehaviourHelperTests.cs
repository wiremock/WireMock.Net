// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class MatchBehaviourHelperTests
{
    [Fact]
    public void MatchBehaviourHelper_Convert_AcceptOnMatch()
    {
        Check.That(MatchBehaviourHelper.Convert(MatchBehaviour.AcceptOnMatch, 0.0)).IsEqualTo(0.0);
        Check.That(MatchBehaviourHelper.Convert(MatchBehaviour.AcceptOnMatch, 0.5)).IsEqualTo(0.5);
        Check.That(MatchBehaviourHelper.Convert(MatchBehaviour.AcceptOnMatch, 1.0)).IsEqualTo(1.0);
    }

    [Fact]
    public void MatchBehaviourHelper_Convert_RejectOnMatch()
    {
        Check.That(MatchBehaviourHelper.Convert(MatchBehaviour.RejectOnMatch, 0.0)).IsEqualTo(1.0);
        Check.That(MatchBehaviourHelper.Convert(MatchBehaviour.RejectOnMatch, 0.5)).IsEqualTo(0.0);
        Check.That(MatchBehaviourHelper.Convert(MatchBehaviour.RejectOnMatch, 1.0)).IsEqualTo(0.0);
    }
}