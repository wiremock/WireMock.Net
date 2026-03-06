// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using FluentAssertions;
using Newtonsoft.Json.Linq;

using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class JsonUtilsTests
{
    [Fact]
    public void JsonUtils_ParseJTokenToObject_For_String()
    {
        // Assign
        object value = "test";

        // Act
        string result = JsonUtils.ParseJTokenToObject<string>(value);

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void JsonUtils_ParseJTokenToObject_For_Int()
    {
        // Assign
        object value = 123;

        // Act
        var result = JsonUtils.ParseJTokenToObject<int>(value);

        // Assert
        result.Should().Be(123);
    }

    [Fact]
    public void JsonUtils_ParseJTokenToObject_For_Invalid_Throws()
    {
        // Assign
        object value = "{ }";

        // Act
        Action action = () => JsonUtils.ParseJTokenToObject<int>(value);

        // Assert
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void JsonUtils_GenerateDynamicLinqStatement_JToken()
    {
        // Assign
        JToken instance = "Test";

        // Act
        string line = JsonUtils.GenerateDynamicLinqStatement(instance);

        // Assert
        var queryable = new[] { instance }.AsQueryable().Select(line);
        bool result = queryable.Any("it == \"Test\"");
        result.Should().BeTrue();

        line.Should().Be("string(it)");
    }

    [Fact]
    public void JsonUtils_GenerateDynamicLinqStatement_JArray_Indexer()
    {
        // Assign
        var instance = new JObject
        {
            { "Items", new JArray(new JValue(4), new JValue(8)) }
        };

        // Act
        string line = JsonUtils.GenerateDynamicLinqStatement(instance);

        // Assert 1
        line.Should().Be("new ((new [] { long(Items[0]), long(Items[1])}) as Items)");

        // Assert 2
        var queryable = new[] { instance }.AsQueryable().Select(line);
        bool result = queryable.Any("Items != null");
        result.Should().BeTrue();
    }

    [Fact]
    public void JsonUtils_GenerateDynamicLinqStatement_JObject2()
    {
        // Assign
        var instance = new JObject
        {
            {"U", new JValue(new Uri("http://localhost:80/abc?a=5"))},
            {"N", new JValue((object?) null)},
            {"G", new JValue(Guid.NewGuid())},
            {"Flt", new JValue(10.0f)},
            {"Dbl", new JValue(Math.PI)},
            {"Check", new JValue(true)},
            {
                "Child", new JObject
                {
                    {"ChildId", new JValue(4)},
                    {"ChildDateTime", new JValue(new DateTime(2018, 2, 17))},
                    {"TS", new JValue(TimeSpan.FromMilliseconds(999))}
                }
            },
            {"I", new JValue(9)},
            {"L", new JValue(long.MaxValue)},
            {"Name", new JValue("Test")}
        };

        // Act
        string line = JsonUtils.GenerateDynamicLinqStatement(instance);

        // Assert 1
        line.Should().Be("new (Uri(U) as U, null as N, Guid(G) as G, double(Flt) as Flt, double(Dbl) as Dbl, bool(Check) as Check, new (long(Child.ChildId) as ChildId, DateTime(Child.ChildDateTime) as ChildDateTime, TimeSpan(Child.TS) as TS) as Child, long(I) as I, long(L) as L, string(Name) as Name)");

        // Assert 2
        var queryable = new[] { instance }.AsQueryable().Select(line);
        bool result = queryable.Any("I > 1 && L > 1");
        result.Should().BeTrue();
    }

    [Fact]
    public void JsonUtils_GenerateDynamicLinqStatement_Throws()
    {
        // Assign
        var instance = new JObject
        {
            { "B", new JValue(new byte[] {48, 49}) }
        };

        // Act and Assert
        Check.ThatCode(() => JsonUtils.GenerateDynamicLinqStatement(instance)).Throws<NotSupportedException>();
    }

    [Fact]
    public void JsonUtils_CreateTypeFromJObject()
    {
        // Assign
        var instance = new JObject
        {
            {"U", new JValue(new Uri("http://localhost:80/abc?a=5"))},
            {"N", new JValue((object?) null)},
            {"G", new JValue(Guid.NewGuid())},
            {"Flt", new JValue(10.0f)},
            {"Dbl", new JValue(Math.PI)},
            {"Check", new JValue(true)},
            {
                "Child", new JObject
                {
                    {"ChildId", new JValue(4)},
                    {"ChildDateTime", new JValue(new DateTime(2018, 2, 17))},
                    {"ChildTimeSpan", new JValue(TimeSpan.FromMilliseconds(999))}
                }
            },
            {"I", new JValue(9)},
            {"L", new JValue(long.MaxValue)},
            {"S", new JValue("Test")},
            {"C", new JValue('c')}
        };

        // Act
        var type = JsonUtils.CreateTypeFromJObject(instance);

        // Assert
        var setProperties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => pi.GetMethod != null).Select(pi => $"{pi.GetMethod}")
            .ToArray();

        setProperties.Should().HaveCount(11);
        setProperties.Should().BeEquivalentTo(new[]
        {
            "System.String get_U()",
            "System.Object get_N()",
            "System.Guid get_G()",
            "Single get_Flt()",
            "Single get_Dbl()",
            "Boolean get_Check()",
            "Child get_Child()",
            "Int64 get_I()",
            "Int64 get_L()",
            "System.String get_S()",
            "System.String get_C()"
        });
    }
}
