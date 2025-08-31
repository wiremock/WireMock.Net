// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using AnyOfTypes;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Newtonsoft.Json;
using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Handlers;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class MatcherMapperTests
{
    private readonly WireMockServerSettings _settings = new();
    private readonly MatcherMapper _sut;

    public MatcherMapperTests()
    {
        _sut = new MatcherMapper(_settings);
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_IMatcher_Null()
    {
        // Act
        var model = _sut.Map((IMatcher?)null);

        // Assert
        model.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_IMatchers_Null()
    {
        // Act
        var model = _sut.Map((IMatcher[]?)null);

        // Assert
        model.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_IMatchers()
    {
        // Assign
        var matcherMock1 = new Mock<IStringMatcher>();
        var matcherMock2 = new Mock<IStringMatcher>();

        // Act
        var models = _sut.Map(new[] { matcherMock1.Object, matcherMock2.Object });

        // Assert
        models.Should().HaveCount(2);
    }

//#if MIMEKIT
    [Fact]
    public void MatcherMapper_Map_Matcher_MimePartMatcher()
    {
        // Arrange
        var bytes = Convert.FromBase64String("c3RlZg==");
        var imagePngContentTypeMatcher = new ContentTypeMatcher("image/png");
        var imagePngContentDispositionMatcher = new ExactMatcher("attachment; filename=\"image.png\"");
        var imagePngContentTransferEncodingMatcher = new ExactMatcher("base64");
        var imagePngContentMatcher = new ExactObjectMatcher(bytes);
        var imagePngMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, imagePngContentTypeMatcher, imagePngContentDispositionMatcher, imagePngContentTransferEncodingMatcher, imagePngContentMatcher);

        // Act
        var model = _sut.Map(imagePngMatcher)!;

        // Assert
        model.Name.Should().Be(nameof(MimePartMatcher));
        model.MatchOperator.Should().BeNull();
        model.RejectOnMatch.Should().BeNull();

        model.ContentTypeMatcher!.Name.Should().Be(nameof(ContentTypeMatcher));
        model.ContentTypeMatcher.Pattern.Should().Be("image/png");

        model.ContentDispositionMatcher!.Name.Should().Be(nameof(ExactMatcher));
        model.ContentDispositionMatcher.Pattern.Should().Be("attachment; filename=\"image.png\"");

        model.ContentTransferEncodingMatcher!.Name.Should().Be(nameof(ExactMatcher));
        model.ContentTransferEncodingMatcher.Pattern.Should().Be("base64");

        model.ContentMatcher!.Name.Should().Be(nameof(ExactObjectMatcher));
        model.ContentMatcher.Pattern.Should().Be(bytes);
    }
//#endif

    [Fact]
    public void MatcherMapper_Map_Matcher_IStringMatcher()
    {
        // Assign
        var matcherMock = new Mock<IStringMatcher>();
        matcherMock.Setup(m => m.Name).Returns("test");
        matcherMock.Setup(m => m.GetPatterns()).Returns(["p1", "p2"]);

        // Act
        var model = _sut.Map(matcherMock.Object)!;

        // Assert
        model.IgnoreCase.Should().BeNull();
        model.Name.Should().Be("test");
        model.Pattern.Should().BeNull();
        model.Patterns.Should().HaveCount(2)
            .And.Contain("p1")
            .And.Contain("p2");
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_IStringMatcher_With_PatternAsFile()
    {
        // Arrange
        var pattern = new StringPattern { Pattern = "p", PatternAsFile = "pf" };

        var matcherMock = new Mock<IStringMatcher>();
        matcherMock.Setup(m => m.Name).Returns("test");
        matcherMock.Setup(m => m.GetPatterns()).Returns(new AnyOf<string, StringPattern>[] { pattern });

        // Act
        var model = _sut.Map(matcherMock.Object)!;

        // Assert
        model.IgnoreCase.Should().BeNull();
        model.Name.Should().Be("test");
        model.Pattern.Should().Be("p");
        model.Patterns.Should().BeNull();
        model.PatternAsFile.Should().Be("pf");
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_IIgnoreCaseMatcher()
    {
        // Assign
        var matcherMock = new Mock<IIgnoreCaseMatcher>();
        matcherMock.Setup(m => m.IgnoreCase).Returns(true);

        // Act
        var model = _sut.Map(matcherMock.Object)!;

        // Assert
        model.IgnoreCase.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_XPathMatcher()
    {
        // Assign
        var xmlNamespaceMap = new[]
        {
            new XmlNamespace { Prefix = "s", Uri = "http://schemas.xmlsoap.org/soap/envelope/" },
            new XmlNamespace { Prefix = "i", Uri = "http://www.w3.org/2001/XMLSchema-instance" },
            new XmlNamespace { Prefix = "q", Uri = "urn://MyWcfService" }
        };
        var matcher = new XPathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.And, xmlNamespaceMap);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.XmlNamespaceMap.Should().NotBeNull();
        model.XmlNamespaceMap.Should().BeEquivalentTo(xmlNamespaceMap);
    }

//#if GRAPHQL
    [Fact]
    public void MatcherMapper_Map_Matcher_GraphQLMatcher()
    {
        // Assign
        const string testSchema = @"
  scalar DateTime
  scalar MyCustomScalar

  type Message {
    id: ID!
  }

  type Mutation {
    createMessage(x: MyCustomScalar, dt: DateTime): Message
  }";

        var customScalars = new Dictionary<string, Type> { { "MyCustomScalar", typeof(string) } };
        var matcher = new GraphQLMatcher(testSchema, customScalars);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(GraphQLMatcher));
        model.Pattern.Should().Be(testSchema);
        model.CustomScalars.Should().BeEquivalentTo(customScalars);
    }
//#endif

//#if PROTOBUF
    [Fact]
    public void MatcherMapper_Map_Matcher_ProtoBufMatcher()
    {
        // Arrange
        IdOrTexts protoDefinition = new(null, @"
syntax = ""proto3"";

package greet;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
");
        const string messageType = "greet.HelloRequest";

        var jsonPattern = new { name = "stef" };
        var jsonMatcher = new JsonMatcher(jsonPattern);

        var matcher = new ProtoBufMatcher(() => protoDefinition, messageType, matcher: jsonMatcher);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(ProtoBufMatcher));
        model.Pattern.Should().Be(protoDefinition.Texts[0]);
        model.ProtoBufMessageType.Should().Be(messageType);
        model.ContentMatcher?.Name.Should().Be("JsonMatcher");
        model.ContentMatcher?.Pattern.Should().Be(jsonPattern);
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_ProtoBufMatcher_WithId()
    {
        // Arrange
        string id = "abc123";
        IdOrTexts protoDefinition = new(id, @"
syntax = ""proto3"";

package greet;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
");
        const string messageType = "greet.HelloRequest";

        var jsonPattern = new { name = "stef" };
        var jsonMatcher = new JsonMatcher(jsonPattern);

        var matcher = new ProtoBufMatcher(() => protoDefinition, messageType, matcher: jsonMatcher);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(ProtoBufMatcher));
        model.Pattern.Should().Be(id);
        model.ProtoBufMessageType.Should().Be(messageType);
        model.ContentMatcher?.Name.Should().Be("JsonMatcher");
        model.ContentMatcher?.Pattern.Should().Be(jsonPattern);
    }
//#endif

    [Fact]
    public void MatcherMapper_Map_MatcherModel_Null()
    {
        // Act
        var result = _sut.Map((MatcherModel?)null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_Exception()
    {
        // Assign
        var model = new MatcherModel { Name = "test" };

        // Act and Assert
        Check.ThatCode(() => _sut.Map(model)).Throws<NotSupportedException>();
    }

    //[Fact]
    //public void MatcherMapper_Map_MatcherModel_LinqMatcher_Pattern()
    //{
    //    // Assign
    //    var model = new MatcherModel
    //    {
    //        Name = "LinqMatcher",
    //        Pattern = "p"
    //    };

    //    // Act
    //    var matcher = (LinqMatcher)_sut.Map(model)!;

    //    // Assert
    //    matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
    //    matcher.GetPatterns().Should().Contain("p");
    //}

    //[Fact]
    //public void MatcherMapper_Map_MatcherModel_LinqMatcher_Patterns()
    //{
    //    // Assign
    //    var model = new MatcherModel
    //    {
    //        Name = "LinqMatcher",
    //        Patterns = ["p1", "p2"]
    //    };

    //    // Act
    //    var matcher = (LinqMatcher)_sut.Map(model)!;

    //    // Assert
    //    matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
    //    matcher.GetPatterns().Should().Contain("p1", "p2");
    //}

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_1_Value_As_String()
    {
        // Assign
        var pattern = "{ \"post1\": \"value1\", \"post2\": \"value2\" }";
        object[] patterns = [pattern];
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_2_Values_As_String()
    {
        // Assign
        var pattern1 = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var pattern2 = "{ \"post1\": \"value1\", \"post2\": \"value2\" }";
        object[] patterns = [pattern1, pattern2];
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Pattern_As_Object()
    {
        // Assign
        var pattern = new { AccountIds = new[] { 1, 2, 3 } };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_1_Value_As_Object()
    {
        // Assign
        object pattern = new { post1 = "value1", post2 = "value2" };
        var patterns = new[] { pattern };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_2_Values_As_Object()
    {
        // Assign
        object pattern1 = new { AccountIds = new[] { 1, 2, 3 } };
        object pattern2 = new { post1 = "value1", post2 = "value2" };
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Patterns_As_String()
    {
        // Assign
        var pattern1 = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var pattern2 = "{ \"X\": \"x\" }";
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = patterns
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Pattern_As_Object()
    {
        // Assign
        var pattern = new { AccountIds = new[] { 1, 2, 3 } };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Patterns_As_Object()
    {
        // Assign
        object pattern1 = new { AccountIds = new[] { 1, 2, 3 } };
        object pattern2 = new { X = "x" };
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_StringPattern_With_PatternAsFile()
    {
        // Assign
        var pattern = new StringPattern { Pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }", PatternAsFile = "pf" };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = pattern,
            Regex = true
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_Patterns_As_Object()
    {
        // Assign
        object pattern = new { X = "*" };
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Pattern = pattern,
            Regex = false
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_NotNullOrEmptyMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "NotNullOrEmptyMatcher",
            RejectOnMatch = true
        };

        // Act
        var matcher = _sut.Map(model)!;

        // Assert
        matcher.Should().BeAssignableTo<NotNullOrEmptyMatcher>();
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
    }

//#if MIMEKIT
    [Fact]
    public void MatcherMapper_Map_MatcherModel_MimePartMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "MimePartMatcher",
            ContentMatcher = new MatcherModel
            {
                Name = "ExactMatcher",
                Pattern = "x"
            },
            ContentDispositionMatcher = new MatcherModel
            {
                Name = "WildcardMatcher",
                Pattern = "y"
            },
            ContentTransferEncodingMatcher = new MatcherModel
            {
                Name = "RegexMatcher",
                Pattern = "z"
            },
            ContentTypeMatcher = new MatcherModel
            {
                Name = "ContentTypeMatcher",
                Pattern = "text/json"
            }
        };

        // Act
        var matcher = (MimePartMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.ContentMatcher.Should().BeAssignableTo<ExactMatcher>().Which.GetPatterns().Should().ContainSingle("x");
        matcher.ContentDispositionMatcher.Should().BeAssignableTo<WildcardMatcher>().Which.GetPatterns().Should().ContainSingle("y");
        matcher.ContentTransferEncodingMatcher.Should().BeAssignableTo<RegexMatcher>().Which.GetPatterns().Should().ContainSingle("z");
        matcher.ContentTypeMatcher.Should().BeAssignableTo<ContentTypeMatcher>().Which.GetPatterns().Should().ContainSingle("text/json");
    }
//#endif

    [Fact]
    public void MatcherMapper_Map_MatcherModel_XPathMatcher_WithXmlNamespaces_As_String()
    {
        // Assign
        var pattern = "/s:Envelope/s:Body/*[local-name()='QueryRequest']";
        var model = new MatcherModel
        {
            Name = "XPathMatcher",
            Pattern = pattern,
            XmlNamespaceMap = new[]
            {
                new XmlNamespace { Prefix = "s", Uri = "http://schemas.xmlsoap.org/soap/envelope/" }
            }
        };

        // Act
        var matcher = (XPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.XmlNamespaceMap.Should().NotBeNull();
        matcher.XmlNamespaceMap.Should().HaveCount(1);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_XPathMatcher_WithoutXmlNamespaces_As_String()
    {
        // Assign
        var pattern = "/s:Envelope/s:Body/*[local-name()='QueryRequest']";
        var model = new MatcherModel
        {
            Name = "XPathMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (XPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.XmlNamespaceMap.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_CSharpCodeMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "CSharpCodeMatcher",
            Patterns = ["return it == \"x\";"]
        };
        var sut = new MatcherMapper(new WireMockServerSettings { AllowCSharpCodeMatcher = true });

        // Act 1
        var matcher1 = (ICSharpCodeMatcher)sut.Map(model)!;

        // Assert 1
        matcher1.Should().NotBeNull();
        matcher1.IsMatch("x").Score.Should().Be(1.0d);

        // Act 2
        var matcher2 = (ICSharpCodeMatcher)sut.Map(model)!;

        // Assert 2
        matcher2.Should().NotBeNull();
        matcher2.IsMatch("x").Score.Should().Be(1.0d);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_CSharpCodeMatcher_NotAllowed_ThrowsException()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "CSharpCodeMatcher",
            Patterns = ["x"]
        };
        var sut = new MatcherMapper(new WireMockServerSettings { AllowCSharpCodeMatcher = false });

        // Act
        Action action = () => sut.Map(model);

        // Assert
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_ExactMatcher_Pattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactMatcher",
            Patterns = ["x"]
        };

        // Act
        var matcher = (ExactMatcher)_sut.Map(model)!;

        // Assert
        matcher.GetPatterns().Should().ContainSingle("x");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_ExactMatcher_Patterns()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactMatcher",
            Patterns = ["x", "y"]
        };

        // Act
        var matcher = (ExactMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_RegexFalse()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Regex = false,
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.IgnoreCase.Should().BeFalse();
        matcher.Value.Should().Be(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_RegexTrue()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Regex = true,
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.IgnoreCase.Should().BeFalse();
        matcher.Value.Should().Be(pattern);
        matcher.Regex.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_ExactObjectMatcher_ValidBase64StringPattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactObjectMatcher",
            Patterns = new object[] { "c3RlZg==" }
        };

        // Act
        var matcher = (ExactObjectMatcher)_sut.Map(model)!;

        // Assert
        Check.That((byte[])matcher.Value).ContainsExactly(115, 116, 101, 102);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_ExactObjectMatcher_InvalidBase64StringPattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactObjectMatcher",
            Patterns = new object[] { "_" }
        };

        // Act & Assert
        Check.ThatCode(() => _sut.Map(model)).Throws<ArgumentException>();
    }

    [Theory]
    [InlineData(MatchOperator.Or, 1.0d)]
    [InlineData(MatchOperator.And, 0.0d)]
    [InlineData(MatchOperator.Average, 0.5d)]
    public void MatcherMapper_Map_MatcherModel_RegexMatcher(MatchOperator matchOperator, double expected)
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "RegexMatcher",
            Patterns = ["x", "y"],
            IgnoreCase = true,
            MatchOperator = matchOperator.ToString()
        };

        // Act
        var matcher = (RegexMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");

        var result = matcher.IsMatch("X");
        result.Score.Should().Be(expected);
    }

    [Theory]
    [InlineData(MatchOperator.Or, 1.0d)]
    [InlineData(MatchOperator.And, 0.0d)]
    [InlineData(MatchOperator.Average, 0.5d)]
    public void MatcherMapper_Map_MatcherModel_WildcardMatcher_IgnoreCase(MatchOperator matchOperator, double expected)
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "WildcardMatcher",
            Patterns = ["x", "y"],
            IgnoreCase = true,
            MatchOperator = matchOperator.ToString()
        };

        // Act
        var matcher = (WildcardMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");

        var result = matcher.IsMatch("X");
        result.Score.Should().Be(expected);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_WildcardMatcher_With_PatternAsFile()
    {
        // Arrange
        var file = "c:\\test.txt";
        var fileContent = "c";
        var stringPattern = new StringPattern
        {
            Pattern = fileContent,
            PatternAsFile = file
        };
        var fileSystemHandleMock = new Mock<IFileSystemHandler>();
        fileSystemHandleMock.Setup(f => f.ReadFileAsString(file)).Returns(fileContent);

        var model = new MatcherModel
        {
            Name = "WildcardMatcher",
            PatternAsFile = file
        };

        var settings = new WireMockServerSettings
        {
            FileSystemHandler = fileSystemHandleMock.Object
        };
        var sut = new MatcherMapper(settings);

        // Act
        var matcher = (WildcardMatcher)sut.Map(model)!;

        // Assert
        matcher.GetPatterns().Should().HaveCount(1).And.Contain(new AnyOf<string, StringPattern>(stringPattern));

        var result = matcher.IsMatch("c");
        result.Score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SimMetricsMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "SimMetricsMatcher",
            Pattern = "x"
        };

        // Act
        var matcher = (SimMetricsMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SimMetricsMatcher_BlockDistance()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "SimMetricsMatcher.BlockDistance",
            Pattern = "x"
        };

        // Act
        var matcher = (SimMetricsMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SimMetricsMatcher_Throws1()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "error",
            Pattern = "x"
        };

        // Act
        Check.ThatCode(() => _sut.Map(model)).Throws<NotSupportedException>();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SimMetricsMatcher_Throws2()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "SimMetricsMatcher.error",
            Pattern = "x"
        };

        // Act
        Check.ThatCode(() => _sut.Map(model)).Throws<NotSupportedException>();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_MatcherModelToCustomMatcher()
    {
        // Arrange
        var patternModel = new CustomPathParamMatcherModel("/customer/{customerId}/document/{documentId}",
            new Dictionary<string, string>(2)
            {
                { "customerId", @"^[0-9]+$" },
                { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
            });
        var model = new MatcherModel
        {
            Name = nameof(CustomPathParamMatcher),
            Pattern = JsonConvert.SerializeObject(patternModel)
        };

        var settings = new WireMockServerSettings();
        settings.CustomMatcherMappings = settings.CustomMatcherMappings ?? new Dictionary<string, Func<MatcherModel, IMatcher>>();
        settings.CustomMatcherMappings[nameof(CustomPathParamMatcher)] = matcherModel =>
        {
            var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)matcherModel.Pattern!)!;
            return new CustomPathParamMatcher(
                matcherModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                matcherParams.Path,
                matcherParams.PathParams
            );
        };
        var sut = new MatcherMapper(settings);

        // Act
        var matcher = sut.Map(model) as CustomPathParamMatcher;

        // Assert
        matcher.Should().NotBeNull();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_CustomMatcherToMatcherModel()
    {
        // Arrange
        var matcher = new CustomPathParamMatcher("/customer/{customerId}/document/{documentId}",
            new Dictionary<string, string>(2)
            {
                { "customerId", @"^[0-9]+$" },
                { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
            });

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        using (new AssertionScope())
        {
            model.Should().NotBeNull();
            model.Name.Should().Be(nameof(CustomPathParamMatcher));

            var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)model.Pattern!)!;
            matcherParams.Path.Should().Be("/customer/{customerId}/document/{documentId}");
            matcherParams.PathParams.Should().BeEquivalentTo(new Dictionary<string, string>(2)
            {
                { "customerId", @"^[0-9]+$" },
                { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
            });
        }
    }

//#if GRAPHQL
    [Fact]
    public void MatcherMapper_Map_MatcherModel_GraphQLMatcher()
    {
        // Arrange
        const string testSchema = @"
  scalar DateTime
  scalar MyCustomScalar

  type Message {
    id: ID!
  }

  type Mutation {
    createMessage(x: MyCustomScalar, dt: DateTime): Message
  }";

        var customScalars = new Dictionary<string, Type> { { "MyCustomScalar", typeof(string) } };
        var model = new MatcherModel
        {
            Name = nameof(GraphQLMatcher),
            Pattern = testSchema,
            CustomScalars = customScalars
        };

        // Act
        var matcher = (IGraphQLMatcher)_sut.Map(model)!;

        // Assert
        matcher.GetPatterns().Should().HaveElementAt(0, testSchema);
        matcher.Name.Should().Be(nameof(GraphQLMatcher));
        matcher.CustomScalars.Should().BeEquivalentTo(customScalars);
    }
//#endif

//#if PROTOBUF
    [Fact]
    public void MatcherMapper_Map_MatcherModel_ProtoBufMatcher()
    {
        // Arrange
        const string protoDefinition = @"
syntax = ""proto3"";

package greet;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
";
        const string messageType = "greet.HelloRequest";

        var jsonMatcherPattern = new { name = "stef" };

        var model = new MatcherModel
        {
            Name = nameof(ProtoBufMatcher),
            Pattern = protoDefinition,
            ProtoBufMessageType = messageType,
            ContentMatcher = new MatcherModel
            {
                Name = nameof(JsonMatcher),
                Pattern = jsonMatcherPattern
            }
        };

        // Act
        var matcher = (ProtoBufMatcher)_sut.Map(model)!;

        // Assert
        matcher.ProtoDefinition().Texts.Should().ContainSingle(protoDefinition);
        matcher.Name.Should().Be(nameof(ProtoBufMatcher));
        matcher.MessageType.Should().Be(messageType);
        matcher.Matcher?.Value.Should().Be(jsonMatcherPattern);
    }
//#endif
}