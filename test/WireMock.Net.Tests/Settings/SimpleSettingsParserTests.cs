// Copyright © WireMock.Net

using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Net.Tests.Settings;

public class SimpleSettingsParserTests
{
    private readonly SimpleSettingsParser _parser;

    public SimpleSettingsParserTests()
    {
        _parser = new SimpleSettingsParser();
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_Arguments()
    {
        // Assign
        _parser.Parse(new[] { "--test1", "one", "--test2", "2", "--test3", "3", "--test4", "true", "--test5", "Https" });

        // Act
        string? stringValue = _parser.GetStringValue("test1");
        int? intOptional = _parser.GetIntValue("test2");
        int intWithDefault = _parser.GetIntValue("test3", 42);
        bool? boolWithDefault = _parser.GetBoolValue("test4");
        HostingScheme? enumOptional = _parser.GetEnumValue<HostingScheme>("test5");
        HostingScheme enumWithDefault = _parser.GetEnumValue("test99", HostingScheme.HttpAndHttps);

        // Assert
        stringValue.Should().Be("one");
        intOptional.Should().Be(2);
        intWithDefault.Should().Be(3);
        boolWithDefault.Should().Be(true);
        enumOptional.Should().Be(HostingScheme.Https);
        enumWithDefault.Should().Be(HostingScheme.HttpAndHttps);
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_Environment()
    {
        // Assign
        var env = new Dictionary<string, string>
        {
            { "WireMockServerSettings__test1", "one" },
            { "WireMockServerSettings__test2", "two" }
        };
        _parser.Parse(new string[0], env);

        // Act
        string? value1 = _parser.GetStringValue("test1");
        string? value2 = _parser.GetStringValue("test2");

        // Assert
        value1.Should().Be("one");
        value2.Should().Be("two");
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_ArgumentsAsCombinedKeyAndValue()
    {
        // Assign
        _parser.Parse(new[] { "--test1 one", "--test2 two", "--test3 three" });

        // Act
        string? value1 = _parser.GetStringValue("test1");
        string? value2 = _parser.GetStringValue("test2");
        string? value3 = _parser.GetStringValue("test3");

        // Assert
        value1.Should().Be("one");
        value2.Should().Be("two");
        value3.Should().Be("three");
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_ArgumentsMixed()
    {
        // Assign
        _parser.Parse(new[] { "--test1 one", "--test2", "two", "--test3 three" });

        // Act
        string? value1 = _parser.GetStringValue("test1");
        string? value2 = _parser.GetStringValue("test2");
        string? value3 = _parser.GetStringValue("test3");

        // Assert
        value1.Should().Be("one");
        value2.Should().Be("two");
        value3.Should().Be("three");
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_GetBoolValue()
    {
        // Assign
        _parser.Parse(new[] { "'--test1", "false'", "--test2 true" });

        // Act
        bool value1 = _parser.GetBoolValue("test1");
        bool value2 = _parser.GetBoolValue("test2");
        bool value3 = _parser.GetBoolValue("test3", true);

        // Assert
        value1.Should().Be(false);
        value2.Should().Be(true);
        value3.Should().Be(true);
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_GetBoolWithDefault()
    {
        // Assign
        _parser.Parse(new[] { "--test1", "true", "--test2", "false" });

        // Act
        bool value1 = _parser.GetBoolWithDefault("test1", "test1_fallback", defaultValue: false);
        bool value2 = _parser.GetBoolWithDefault("missing", "test2", defaultValue: true);
        bool value3 = _parser.GetBoolWithDefault("missing1", "missing2", defaultValue: true);

        // Assert
        value1.Should().Be(true);
        value2.Should().Be(false);
        value3.Should().Be(true);
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_Environment_GetBoolValue()
    {
        // Assign
        var env = new Dictionary<string, string>
        {
            { "WireMockServerSettings__test1", "false" },
            { "WireMockServerSettings__test2", "true" }
        };
        _parser.Parse(new string[0], env);

        // Act
        bool value1 = _parser.GetBoolValue("test1");
        bool value2 = _parser.GetBoolValue("test2");
        bool value3 = _parser.GetBoolValue("test3", true);

        // Assert
        value1.Should().Be(false);
        value2.Should().Be(true);
        value3.Should().Be(true);
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_GetIntValue()
    {
        // Assign
        _parser.Parse(new[] { "--test1", "42", "--test2 55" });

        // Act
        int? value1 = _parser.GetIntValue("test1");
        int? value2 = _parser.GetIntValue("test2");
        int? value3 = _parser.GetIntValue("test3", 100);
        int? value4 = _parser.GetIntValue("test4");

        // Assert
        value1.Should().Be(42);
        value2.Should().Be(55);
        value3.Should().Be(100);
        value4.Should().BeNull();
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_Environment_GetIntValue()
    {
        // Assign
        var env = new Dictionary<string, string>
        {
            { "WireMockServerSettings__test1", "42" },
            { "WireMockServerSETTINGS__TEST2", "55" }
        };
        _parser.Parse(new string[0], env);

        // Act
        int? value1 = _parser.GetIntValue("test1");
        int? value2 = _parser.GetIntValue("test2");
        int? value3 = _parser.GetIntValue("test3", 100);
        int? value4 = _parser.GetIntValue("test4");

        // Assert
        value1.Should().Be(42);
        value2.Should().Be(55);
        value3.Should().Be(100);
        value4.Should().BeNull();
    }

    [Fact]
    public void SimpleCommandLineParser_Parse_GetObjectValueFromJson()
    {
        // Assign
        _parser.Parse(new[] { @"--json {""k1"":""v1"",""k2"":""v2""}" });

        // Act
        var value = _parser.GetObjectValueFromJson<Dictionary<string, string>>("json");

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "k1", "v1" },
            { "k2", "v2" }
        };
        value.Should().BeEquivalentTo(expected);
    }
}
