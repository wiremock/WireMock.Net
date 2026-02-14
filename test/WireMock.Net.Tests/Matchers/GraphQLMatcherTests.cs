// Copyright © WireMock.Net

using AwesomeAssertions;
using GraphQLParser.Exceptions;
using WireMock.Exceptions;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Net.Tests.Matchers;

public class GraphQLMatcherTests
{
    private const string TestSchema = @"
  scalar DateTime

  input MessageInput {
    date: DateTime
    content: String
    author: String
  }

  type Message {
    id: ID!
    content: String
    author: String
  }

  type Mutation {
    createMessage(input: MessageInput): Message
    createAnotherMessage(date: DateTime, content: String, author: String): Message
    updateMessage(id: ID!, input: MessageInput): Message
  }

  type Query {
    greeting: String
    students: [Student]
    studentById(id: ID!):Student
  }

  type Student {
    id: ID!
    firstName: String
    lastName: String
    fullName: String 
  }";

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectQuery_IsMatch()
    {
        // Arrange
        var input = "{\"query\":\"{\\r\\n students {\\r\\n fullName\\r\\n id\\r\\n }\\r\\n}\"}";

        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);

        matcher.GetPatterns().Should().Contain(TestSchema);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectGraphQL_Query_IsMatch()
    {
        // Arrange
        var input = @"{
	""query"": ""query ($sid: ID!)\r\n{\r\n studentById(id: $sid) {\r\n fullName\r\n id\r\n }\r\n}"",
	""variables"": {
		""sid"": ""1""
	}
}";
        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);

        matcher.GetPatterns().Should().Contain(TestSchema);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectGraphQL_Mutation_IsMatch()
    {
        // Arrange
        var input = @"{
    ""query"": ""mutation CreateAnotherMessage($date: DateTime!, $content: String!, $author: String!) { createAnotherMessage(date: $date, content: $content, author: $author) { id } }"",
    ""variables"": { ""date"": ""2007-12-03T10:15:30Z"", ""content"": ""--content--"", ""author"": ""--author--"" }
}";

        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);

        matcher.GetPatterns().Should().Contain(TestSchema);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectGraphQL_UsingCustomType_Mutation_IsMatch()
    {
        // Arrange
        // Query is provided here: https://stackoverflow.com/questions/59608833/apollo-graphql-error-query-root-type-must-be-provided
        const string testSchema =
            """
              scalar DateTime
              scalar MyCustomScalar
            
              type Query {
                _empty: String  
              }
            
              type Message {
                id: ID!
              }
            
              type Mutation {
                createMessage(x: MyCustomScalar, dt: DateTime): Message
              }
            """;

        const string input =
            """
            {
                "query": "mutation CreateMessage($x: MyCustomScalar!, $dt: DateTime!) { createMessage(x: $x, dt: $dt) { id } }",
                "variables": { "x": 100, "dt": "2007-12-03T10:15:30Z" }
            }
            """;

        var customScalars = new Dictionary<string, Type> { { "MyCustomScalar", typeof(int) } };

        // Act
        var matcher = new GraphQLMatcher(testSchema, customScalars);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);

        matcher.GetPatterns().Should().Contain(testSchema);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectGraphQL_UsingCustomType_But_NoDefinedCustomScalars_Mutation_IsNoMatch()
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

        // Act
        Action action = () => _ = new GraphQLMatcher(testSchema);

        // Assert
        action.Should().Throw<WireMockException>().WithMessage("The GraphQL Scalar type 'MyCustomScalar' is not defined in the CustomScalars dictionary.");
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_IncorrectQuery_IsMismatch()
    {
        // Arrange
        var input = @"
{
  students {
    fullName
    id
    abc
  }
}";
        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchemaAsStringPattern_And_CorrectQuery_IsMatch()
    {
        // Arrange
        var input = "{\"query\":\"{\\r\\n students {\\r\\n fullName\\r\\n id\\r\\n }\\r\\n}\"}";
        var schema = new StringPattern
        {
            Pattern = TestSchema
        };

        // Act
        var matcher = new GraphQLMatcher(schema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_IncorrectQueryWithError_WithThrowExceptionTrue_ReturnsError()
    {
        // Arrange
        var input = "{\"query\":\"{\\r\\n studentsX {\\r\\n fullName\\r\\n X\\r\\n }\\r\\n}\"}";

        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception!.Message.Should().StartWith("Cannot query field 'studentsX' on type 'Query'");
    }

    [Fact]
    public void GraphQLMatcher_For_InvalidSchema_ThrowsGraphQLSyntaxErrorException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => _ = new GraphQLMatcher("in va lid");

        // Assert
        action.Should().Throw<GraphQLSyntaxErrorException>();
    }
}