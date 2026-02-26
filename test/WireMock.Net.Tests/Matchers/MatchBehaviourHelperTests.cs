// Copyright © WireMock.Net

using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class MatchBehaviourHelperTests
{
    [Fact]
    public void MatchBehaviourHelper_Convert_AcceptOnMatch()
    {
        MatchBehaviourHelper.Convert(MatchBehaviour.AcceptOnMatch, 0.0).Should().Be(0.0);
        MatchBehaviourHelper.Convert(MatchBehaviour.AcceptOnMatch, 0.5).Should().Be(0.5);
        MatchBehaviourHelper.Convert(MatchBehaviour.AcceptOnMatch, 1.0).Should().Be(1.0);
    }

    [Fact]
    public void MatchBehaviourHelper_Convert_RejectOnMatch()
    {
        MatchBehaviourHelper.Convert(MatchBehaviour.RejectOnMatch, 0.0).Should().Be(1.0);
        MatchBehaviourHelper.Convert(MatchBehaviour.RejectOnMatch, 0.5).Should().Be(0.0);
        MatchBehaviourHelper.Convert(MatchBehaviour.RejectOnMatch, 1.0).Should().Be(0.0);
    }
}