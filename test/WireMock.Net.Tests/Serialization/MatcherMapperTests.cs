// Copyright © WireMock.Net

using AnyOfTypes;
using AwesomeAssertions.Execution;
using Moq;
using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Handlers;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Serialization;
using WireMock.Settings;

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
        var models = _sut.Map([matcherMock1.Object, matcherMock2.Object]);

        // Assert
        models.Should().HaveCount(2);
    }

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
        matcherMock.Setup(m => m.GetPatterns()).Returns([pattern]);

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

        // Act
        Action act = () => _sut.Map(model);

        // Assert
        act.Should().Throw<NotSupportedException>();
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
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Patterns_1_Value_As_Object()
    {
        // Assign
        object pattern = new { post1 = "value1", post2 = "value2" };
        var patterns = new[] { pattern };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Patterns_2_Values_As_Object()
    {
        // Assign
        object pattern1 = new { AccountIds = new[] { 1, 2, 3 } };
        object pattern2 = new { post1 = "value1", post2 = "value2" };
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"Name\": \"T*\" }";
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_Pattern_As_Object()
    {
        // Assign
        object pattern = new { X = "*" };
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_RegexTrue()
    {
        // Assign
        var pattern = "{ \"x\": \"^\\\\d+$\" }";
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Regex = true,
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Regex.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_RejectOnMatch()
    {
        // Assign
        var pattern = "{ \"x\": \"*\" }";
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Pattern = pattern,
            RejectOnMatch = true
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_IgnoreCaseTrue()
    {
        // Assign
        var pattern = "{ \"name\": \"t*\" }";
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Pattern = pattern,
            IgnoreCase = true
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.IgnoreCase.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    #region SystemTextJsonMatcher

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonMatcher_Pattern_As_Object()
    {
        // Assign
        var pattern = new { AccountIds = new[] { 1, 2, 3 } };
        var model = new MatcherModel
        {
            Name = "SystemTextJsonMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonMatcher_Patterns_As_String()
    {
        // Assign
        var pattern1 = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var pattern2 = "{ \"post1\": \"value1\" }";
        object[] patterns = [pattern1, pattern2];
        var model = new MatcherModel
        {
            Name = "SystemTextJsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (SystemTextJsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonMatcher_RejectOnMatch()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonMatcher",
            Pattern = pattern,
            RejectOnMatch = true
        };

        // Act
        var matcher = (SystemTextJsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonMatcher_IgnoreCaseTrue()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonMatcher",
            Pattern = pattern,
            IgnoreCase = true
        };

        // Act
        var matcher = (SystemTextJsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.IgnoreCase.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonMatcher_RegexTrue()
    {
        // Assign
        var pattern = "{ \"x\": \"^\\\\d+$\" }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonMatcher",
            Pattern = pattern,
            Regex = true
        };

        // Act
        var matcher = (SystemTextJsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.Regex.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonMatcher_To_MatcherModel()
    {
        // Assign
        var pattern = new { Id = 1, Name = "Test" };
        var matcher = new SystemTextJsonMatcher(MatchBehaviour.AcceptOnMatch, pattern, ignoreCase: true, regex: true);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonMatcher));
        model.Pattern.Should().BeEquivalentTo(pattern);
        model.IgnoreCase.Should().BeTrue();
        model.Regex.Should().BeTrue();
        model.RejectOnMatch.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonMatcher_RejectOnMatch_To_MatcherModel()
    {
        // Assign
        var pattern = "{ \"Id\": 1 }";
        var matcher = new SystemTextJsonMatcher(MatchBehaviour.RejectOnMatch, pattern);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonMatcher));
        model.Pattern.Should().Be(pattern);
        model.RejectOnMatch.Should().BeTrue();
        model.Regex.Should().BeFalse();
    }

    #endregion

    #region SystemTextJsonPartialMatcher

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_Pattern_As_Object()
    {
        // Assign
        var pattern = new { AccountIds = new[] { 1, 2, 3 } };
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_Patterns_As_String()
    {
        // Assign
        var pattern1 = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var pattern2 = "{ \"X\": \"x\" }";
        object[] patterns = [pattern1, pattern2];
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_RegexFalse()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Regex = false,
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.IgnoreCase.Should().BeFalse();
        matcher.Value.Should().Be(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_RegexTrue()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Regex = true,
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.IgnoreCase.Should().BeFalse();
        matcher.Value.Should().Be(pattern);
        matcher.Regex.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_RejectOnMatch()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Pattern = pattern,
            RejectOnMatch = true
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialMatcher_IgnoreCaseTrue()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialMatcher",
            Pattern = pattern,
            IgnoreCase = true
        };

        // Act
        var matcher = (SystemTextJsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.IgnoreCase.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPartialMatcher_To_MatcherModel()
    {
        // Assign
        var pattern = new { Id = 1, Name = "Test" };
        var matcher = new SystemTextJsonPartialMatcher(MatchBehaviour.AcceptOnMatch, pattern, ignoreCase: true, regex: true);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPartialMatcher));
        model.Pattern.Should().BeEquivalentTo(pattern);
        model.IgnoreCase.Should().BeTrue();
        model.Regex.Should().BeTrue();
        model.RejectOnMatch.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPartialMatcher_RejectOnMatch_To_MatcherModel()
    {
        // Assign
        var pattern = "{ \"Id\": 1 }";
        var matcher = new SystemTextJsonPartialMatcher(MatchBehaviour.RejectOnMatch, pattern);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPartialMatcher));
        model.Pattern.Should().Be(pattern);
        model.RejectOnMatch.Should().BeTrue();
        model.Regex.Should().BeFalse();
    }

    #endregion

    #region SystemTextJsonPartialWildcardMatcher

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialWildcardMatcher_Pattern_As_Object()
    {
        // Assign
        object pattern = new { X = "*" };
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialWildcardMatcher",
            Pattern = pattern,
            Regex = false
        };

        // Act
        var matcher = (SystemTextJsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialWildcardMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"Name\": \"T*\" }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialWildcardMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialWildcardMatcher_RegexTrue()
    {
        // Assign
        var pattern = "{ \"x\": \"^\\\\d+$\" }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialWildcardMatcher",
            Regex = true,
            Pattern = pattern
        };

        // Act
        var matcher = (SystemTextJsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Regex.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialWildcardMatcher_RejectOnMatch()
    {
        // Assign
        var pattern = "{ \"x\": \"*\" }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialWildcardMatcher",
            Pattern = pattern,
            RejectOnMatch = true
        };

        // Act
        var matcher = (SystemTextJsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPartialWildcardMatcher_IgnoreCaseTrue()
    {
        // Assign
        var pattern = "{ \"name\": \"t*\" }";
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPartialWildcardMatcher",
            Pattern = pattern,
            IgnoreCase = true
        };

        // Act
        var matcher = (SystemTextJsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.IgnoreCase.Should().BeTrue();
        matcher.Value.Should().Be(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPartialWildcardMatcher_To_MatcherModel()
    {
        // Assign
        var pattern = new { Id = 1, Name = "T*" };
        var matcher = new SystemTextJsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, pattern, ignoreCase: true);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPartialWildcardMatcher));
        model.Pattern.Should().BeEquivalentTo(pattern);
        model.IgnoreCase.Should().BeTrue();
        model.Regex.Should().BeFalse();
        model.RejectOnMatch.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPartialWildcardMatcher_RejectOnMatch_To_MatcherModel()
    {
        // Assign
        var pattern = "{ \"Name\": \"T*\" }";
        var matcher = new SystemTextJsonPartialWildcardMatcher(MatchBehaviour.RejectOnMatch, pattern);

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPartialWildcardMatcher));
        model.Pattern.Should().Be(pattern);
        model.RejectOnMatch.Should().BeTrue();
        model.Regex.Should().BeFalse();
    }

    #endregion

    #region SystemTextJsonPathMatcher

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPathMatcher_SinglePattern()
    {
        // Arrange
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPathMatcher",
            Pattern = "$.Id"
        };

        // Act
        var matcher = (SystemTextJsonPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.GetPatterns().Should().ContainSingle().Which.First.Should().Be("$.Id");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPathMatcher_MultiplePatterns()
    {
        // Arrange
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPathMatcher",
            Patterns = ["$.Id", "$.Name"],
            MatchOperator = "And"
        };

        // Act
        var matcher = (SystemTextJsonPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchOperator.Should().Be(MatchOperator.And);
        matcher.GetPatterns().Select(p => p.First).Should().BeEquivalentTo("$.Id", "$.Name");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_SystemTextJsonPathMatcher_RejectOnMatch()
    {
        // Arrange
        var model = new MatcherModel
        {
            Name = "SystemTextJsonPathMatcher",
            Pattern = "$.Id",
            RejectOnMatch = true
        };

        // Act
        var matcher = (SystemTextJsonPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPathMatcher_To_MatcherModel_SinglePattern()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.Id");

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPathMatcher));
        model.Pattern.Should().Be("$.Id");
        model.Patterns.Should().BeNull();
        model.RejectOnMatch.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPathMatcher_To_MatcherModel_MultiplePatterns()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.And, "$.Id", "$.Name");

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPathMatcher));
        model.Pattern.Should().BeNull();
        model.Patterns.Should().BeEquivalentTo(["$.Id", "$.Name"]);
        model.MatchOperator.Should().Be("And");
    }

    [Fact]
    public void MatcherMapper_Map_Matcher_SystemTextJsonPathMatcher_RejectOnMatch_To_MatcherModel()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, "$.Id");

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        model.Name.Should().Be(nameof(SystemTextJsonPathMatcher));
        model.Pattern.Should().Be("$.Id");
        model.RejectOnMatch.Should().BeTrue();
    }

    #endregion
}