// Copyright © WireMock.Net

using NFluent;
using WireMock.RegularExpressions;

namespace WireMock.Net.Tests.RegularExpressions;

public class RegexExtendedTests
{
    /// <summary>
    /// Input guid used for testing
    /// </summary>
    public Guid InputGuid => Guid.NewGuid();

    [Fact]
    public void RegexExtended_GuidB_Pattern()
    {
        var guidbUpper = @".*\GUIDB.*";
        var guidbLower = @".*\guidb.*";

        var inputLower = InputGuid.ToString("B");
        var inputUpper = InputGuid.ToString("B").ToUpper();
        var regexLower = new RegexExtended(guidbLower);
        var regexUpper = new RegexExtended(guidbUpper);

        Check.That(regexLower.IsMatch(inputLower)).Equals(true);
        Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
        Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
        Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
    }

    [Fact]
    public void RegexExtended_GuidD_Pattern()
    {
        var guiddUpper = @".*\GUIDD.*";
        var guiddLower = @".*\guidd.*";

        var inputLower = InputGuid.ToString("D");
        var inputUpper = InputGuid.ToString("D").ToUpper();
        var regexLower = new RegexExtended(guiddLower);
        var regexUpper = new RegexExtended(guiddUpper);

        Check.That(regexLower.IsMatch(inputLower)).Equals(true);
        Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
        Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
        Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
    }

    [Fact]
    public void RegexExtended_GuidN_Pattern()
    {
        var guidnUpper = @".*\GUIDN.*";
        var guidnLower = @".*\guidn.*";

        var inputLower = InputGuid.ToString("N");
        var inputUpper = InputGuid.ToString("N").ToUpper();
        var regexLower = new RegexExtended(guidnLower);
        var regexUpper = new RegexExtended(guidnUpper);

        Check.That(regexLower.IsMatch(inputLower)).Equals(true);
        Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
        Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
        Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
    }

    [Fact]
    public void RegexExtended_GuidP_Pattern()
    {
        var guidpUpper = @".*\GUIDP.*";
        var guidpLower = @".*\guidp.*";

        var inputLower = InputGuid.ToString("P");
        var inputUpper = InputGuid.ToString("P").ToUpper();
        var regexLower = new RegexExtended(guidpLower);
        var regexUpper = new RegexExtended(guidpUpper);

        Check.That(regexLower.IsMatch(inputLower)).Equals(true);
        Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
        Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
        Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
    }

    [Fact]
    public void RegexExtended_GuidX_Pattern()
    {
        var guidxUpper = @".*\GUIDX.*";
        var guidxLower = @".*\guidx.*";

        var inputLower = InputGuid.ToString("X");
        var inputUpper = InputGuid.ToString("X").ToUpper().Replace("X", "x");
        var regexLower = new RegexExtended(guidxLower);
        var regexUpper = new RegexExtended(guidxUpper);

        Check.That(regexLower.IsMatch(inputLower)).Equals(true);
        Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
        Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
        Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
    }
}