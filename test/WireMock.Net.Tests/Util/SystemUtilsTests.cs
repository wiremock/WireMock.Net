// Copyright © WireMock.Net

using FluentAssertions;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class SystemUtilsTests
{
    [Fact]
    public void Version()
    {
        SystemUtils.Version.Should().NotBeEmpty();
    }
}