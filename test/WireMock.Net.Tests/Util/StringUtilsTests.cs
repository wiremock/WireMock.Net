// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Matchers;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class StringUtilsTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("x", "x")]
    public void TryConvertToString_ShouldWorkCorrectly(string input, string expectedValue)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(false);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<bool>().And.Be(expectedValue);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("true", true, true)]
    [InlineData("false", false, true)]
    [InlineData("not a bool", false, false)] // Invalid case
    public void TryConvertToBool_ShouldWorkCorrectly(string input, bool expectedValue, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<bool>().And.Be(expectedValue);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("123", 123, true)]
    [InlineData("-456", -456, true)]
    [InlineData("not an int", 0, false)] // Invalid case
    public void TryConvertToInt_ShouldWorkCorrectly(string input, int expectedValue, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<int>().And.Be(expectedValue);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("12345678901", 12345678901L, true)]
    [InlineData("-9876543210", -9876543210L, true)]
    [InlineData("not a long", 0L, false)] // Invalid case
    public void TryConvertToLong_ShouldWorkCorrectly(string input, long expectedValue, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<long>().And.Be(expectedValue);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [CulturedTheory(["en-US"])]
    [InlineData("123.1", 123.1, true)]
    [InlineData("-456.1", -456.1, true)]
    [InlineData("not a double", 0.0, false)] // Invalid case
    public void TryConvertToDouble_ShouldWorkCorrectly(string input, double expectedValue, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            ((double)convertedValue).Should().BeApproximately(expectedValue, 0.01);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("3F2504E04F8911D39A0C0305E82C3301", false)]
    [InlineData("{3F2504E04F8911D39A0C0305E82C3301}", false)]
    [InlineData("(3F2504E04F8911D39A0C0305E82C3301)", false)]
    [InlineData("{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}", false)]
    [InlineData("3F2504E0-4F89-11D3-9A0C-0305E82C3301", true)]
    [InlineData("00000000-0000-0000-0000-000000000000", true)]
    [InlineData("3f2504e0-4f89-11d3-9a0c-0305e82c3301", true)] // Lowercase Guid
    [InlineData("not a guid", false)] // Invalid case
    public void TryConvertToGuid_ShouldWorkCorrectly(string input, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<Guid>();
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("2023-04-01", true)]
    [InlineData("01/01/2000", true)]
    [InlineData("not a date", false)] // Invalid case
    public void TryConvertToDateTime_ShouldWorkCorrectly(string input, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<DateTime>().And.Subject.As<DateTime>().Date.Should().Be(DateTime.Parse(input).Date);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("1.00:00:00", true)] // 1 day
    [InlineData("00:30:00", true)] // 30 minutes
    [InlineData("not a timespan", false)] // Invalid case
    public void TryConvertToTimeSpan_ShouldWorkCorrectly(string input, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<TimeSpan>().And.Subject.As<TimeSpan>().Should().Be(TimeSpan.Parse(input));
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("http://example.com", true)]
    [InlineData("https://example.com/path?query=string#fragment", true)]
    [InlineData("ftp://example.com", true)]
    [InlineData("file://example.com", false)]
    [InlineData("not a uri", false)] // Invalid case
    public void TryConvertToUri_ShouldWorkCorrectly(string input, bool expectedConversion)
    {
        var (isConverted, convertedValue) = StringUtils.TryConvertToKnownType(input);

        isConverted.Should().Be(expectedConversion);
        if (isConverted)
        {
            convertedValue.Should().BeOfType<Uri>().And.Subject.As<Uri>().AbsoluteUri.Should().Be(new Uri(input).AbsoluteUri);
        }
        else
        {
            convertedValue.Should().Be(input);
        }
    }

    [Theory]
    [InlineData("And", MatchOperator.And)]
    [InlineData("Or", MatchOperator.Or)]
    public void ParseMatchOperator_ShouldReturnCorrectEnumValue_WhenValidStringIsProvided(string value, MatchOperator expected)
    {
        // Arrange & Act
        var result = StringUtils.ParseMatchOperator(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, MatchOperator.Or)]
    [InlineData("", MatchOperator.Or)]
    [InlineData("and", MatchOperator.Or)]
    [InlineData("InvalidValue", MatchOperator.Or)]
    public void ParseMatchOperator_ShouldReturnDefaultEnumValue_WhenInvalidOrNullStringIsProvided(string? value, MatchOperator expected)
    {
        // Arrange & Act
        var result = StringUtils.ParseMatchOperator(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("'s")]
    [InlineData("\"s")]
    public void StringUtils_TryParseQuotedString_With_UnexpectedUnclosedString_Returns_False(string input)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("x")]
    public void StringUtils_TryParseQuotedString_With_InvalidStringLength_Returns_False(string? input)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("xx")]
    [InlineData("  ")]
    public void StringUtils_TryParseQuotedString_With_InvalidStringQuoteCharacter_Returns_False(string input)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Fact]
    public void StringUtils_TryParseQuotedString_With_UnexpectedUnrecognizedEscapeSequence_Returns_False()
    {
        // Arrange
        var input = new string(['"', '\\', 'u', '?', '"']);

        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("''", "")]
    [InlineData("'s'", "s")]
    [InlineData("'\\\\'", "\\")]
    [InlineData("'\\n'", "\n")]
    public void StringUtils_TryParseQuotedString_SingleQuotedString(string input, string expectedResult)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out var result, out var quote);

        // Assert
        valid.Should().BeTrue();
        result.Should().Be(expectedResult);
        quote.Should().Be('\'');
    }

    [Theory]
    [InlineData("\"\"", "")]
    [InlineData("\"\\\\\"", "\\")]
    [InlineData("\"\\n\"", "\n")]
    [InlineData("\"\\\\n\"", "\\n")]
    [InlineData("\"\\\\new\"", "\\new")]
    [InlineData("\"[]\"", "[]")]
    [InlineData("\"()\"", "()")]
    [InlineData("\"(\\\"\\\")\"", "(\"\")")]
    [InlineData("\"/\"", "/")]
    [InlineData("\"a\"", "a")]
    [InlineData("\"This \\\"is\\\" a test.\"", "This \"is\" a test.")]
    [InlineData(@"""This \""is\"" b test.""", @"This ""is"" b test.")]
    [InlineData("\"ab\\\"cd\"", "ab\"cd")]
    [InlineData("\"\\\"\"", "\"")]
    [InlineData("\"\\\"\\\"\"", "\"\"")]
    [InlineData("\"AB YZ 19 \uD800\udc05 \u00e4\"", "AB YZ 19 \uD800\udc05 \u00e4")]
    [InlineData("\"\\\\\\\\192.168.1.1\\\\audio\\\\new\"", "\\\\192.168.1.1\\audio\\new")]
    public void StringUtils_TryParseQuotedString_DoubleQuotedString(string input, string expectedResult)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out var result, out var quote);

        // Assert
        valid.Should().BeTrue();
        result.Should().Be(expectedResult);
        quote.Should().Be('"');
    }
}