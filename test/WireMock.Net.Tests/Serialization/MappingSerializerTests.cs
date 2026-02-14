// Copyright © WireMock.Net

using FluentAssertions;
using JsonConverter.Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Serialization;
#if NET8_0_OR_GREATER
using JsonConverter.System.Text.Json;
#endif

namespace WireMock.Net.Tests.Serialization;

public class MappingSerializerTests
{
    private const string SingleMappingJson =
        """
        {
            "Guid": "12345678-1234-1234-1234-123456789012",
            "Priority": 1,
            "Request": {
                "Path": "/test"
            },
            "Response": {
                "StatusCode": 200
            }
        }
        """;

    private const string ArrayMappingJson =
        """
        [
            {
                "Guid": "12345678-1234-1234-1234-123456789012",
                "Priority": 1,
                "Request": {
                    "Path": "/test1"
                },
                "Response": {
                    "StatusCode": 200
                }
            },
            {
                "Guid": "87654321-4321-4321-4321-210987654321",
                "Priority": 2,
                "Request": {
                    "Path": "/test2"
                },
                "Response": {
                    "StatusCode": 404
                }
            }
        ]
        """;

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_SingleObject_ShouldReturnArray()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(SingleMappingJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Guid.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        result[0].Priority.Should().Be(1);
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_Array_ShouldReturnArray()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(ArrayMappingJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Guid.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        result[0].Priority.Should().Be(1);
        result[1].Guid.Should().Be(Guid.Parse("87654321-4321-4321-4321-210987654321"));
        result[1].Priority.Should().Be(2);
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_EmptyArray_ShouldReturnEmptyArray()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var emptyArrayJson = "[]";

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(emptyArrayJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_InvalidJson_ShouldThrowException()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var invalidJson = "not valid json";

        // Act
        Action act = () => serializer.DeserializeJsonToArray<MappingModel>(invalidJson);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_ComplexMapping_ShouldDeserializeCorrectly()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var complexJson =
            """
            {
                "Guid": "12345678-1234-1234-1234-123456789012",
                "Title": "Test Mapping",
                "Description": "A test mapping",
                "Priority": 10,
                "Request": {
                    "Path": "/api/test",
                    "Methods": ["GET", "POST"]
                },
                "Response": {
                    "StatusCode": 201,
                    "Body": "Test Response"
                }
            }
            """;

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(complexJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Guid.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        result[0].Title.Should().Be("Test Mapping");
        result[0].Description.Should().Be("A test mapping");
        result[0].Priority.Should().Be(10);
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_NullValue_ShouldThrowException()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var nullJson = "null";

        // Act
        Action act = () => serializer.DeserializeJsonToArray<MappingModel>(nullJson);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithNewtonsoftJson_PrimitiveValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var jsonConverter = new NewtonsoftJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var primitiveJson = "\"string value\"";

        // Act
        Action act = () => serializer.DeserializeJsonToArray<MappingModel>(primitiveJson);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot deserialize the provided value to an array or object.");
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_SingleObject_ShouldReturnArray()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(SingleMappingJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Guid.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        result[0].Priority.Should().Be(1);
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_Array_ShouldReturnArray()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(ArrayMappingJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Guid.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        result[0].Priority.Should().Be(1);
        result[1].Guid.Should().Be(Guid.Parse("87654321-4321-4321-4321-210987654321"));
        result[1].Priority.Should().Be(2);
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_EmptyArray_ShouldReturnEmptyArray()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var emptyArrayJson = "[]";

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(emptyArrayJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_InvalidJson_ShouldThrowException()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var invalidJson = "not valid json";

        // Act
        Action act = () => serializer.DeserializeJsonToArray<MappingModel>(invalidJson);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_ComplexMapping_ShouldDeserializeCorrectly()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var complexJson =
            """
            {
                "Guid": "12345678-1234-1234-1234-123456789012",
                "Title": "Test Mapping",
                "Description": "A test mapping",
                "Priority": 10,
                "Request": {
                    "Path": "/api/test",
                    "Methods": ["GET", "POST"]
                },
                "Response": {
                    "StatusCode": 201,
                    "Body": "Test Response"
                }
            }
            """;

        // Act
        var result = serializer.DeserializeJsonToArray<MappingModel>(complexJson);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Guid.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        result[0].Title.Should().Be("Test Mapping");
        result[0].Description.Should().Be("A test mapping");
        result[0].Priority.Should().Be(10);
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_NullValue_ShouldThrowException()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var nullJson = "null";

        // Act
        Action act = () => serializer.DeserializeJsonToArray<MappingModel>(nullJson);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void MappingSerializer_DeserializeJsonToArray_WithSystemTextJson_PrimitiveValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var jsonConverter = new SystemTextJsonConverter();
        var serializer = new MappingSerializer(jsonConverter);
        var primitiveJson = "\"string value\"";

        // Act
        Action act = () => serializer.DeserializeJsonToArray<MappingModel>(primitiveJson);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot deserialize the provided value to an array or object.");
    }
#endif
}