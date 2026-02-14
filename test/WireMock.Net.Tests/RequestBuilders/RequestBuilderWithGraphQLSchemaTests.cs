// Copyright © WireMock.Net

using FluentAssertions;
using GraphQL.Types;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithGraphQLSchemaTests
{
    private const string TestSchema = @"
  input MessageInput {
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
    updateMessage(id: ID!, input: MessageInput): Message
  }

  type Query {
   greeting:String
   students:[Student]
   studentById(id:ID!):Student
  }

  type Student {
   id:ID!
   firstName:String
   lastName:String
   fullName:String 
  }";

    [Fact]
    public void RequestBuilder_WithGraphQLSchema_SchemaAsString()
    {
        // Act
        var requestBuilder = (Request)Request.Create().WithGraphQLSchema(TestSchema);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);
        ((RequestMessageGraphQLMatcher)matchers[0]).Matchers.Should().ContainItemsAssignableTo<GraphQLMatcher>();
    }

    [Fact]
    public void RequestBuilder_WithGraphQLSchema_SchemaAsISchema()
    {
        // Arrange
        var schema = Schema.For(TestSchema);

        // Act
        var requestBuilder = (Request)Request.Create().WithGraphQLSchema(schema);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);
        ((RequestMessageGraphQLMatcher)matchers[0]).Matchers.Should().ContainItemsAssignableTo<GraphQLMatcher>();
    }
}