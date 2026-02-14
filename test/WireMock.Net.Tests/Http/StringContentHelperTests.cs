// Copyright © WireMock.Net

using System.Net.Http.Headers;
using FluentAssertions;
using WireMock.Http;

namespace WireMock.Net.Tests.Http;

public class StringContentHelperTests
{
    [Fact]
    public void StringContentHelper_Create_WithNullContentType()
    {
        // Act
        var result = StringContentHelper.Create("test", null);

        // Assert
        result.Headers.ContentType.Should().BeNull();
        result.ReadAsStringAsync().Result.Should().Be("test");
    }

    [Theory]
    [InlineData("application/json", "application/json")]
    [InlineData("application/soap+xml", "application/soap+xml")]
    [InlineData("application/soap+xml;charset=UTF-8", "application/soap+xml; charset=UTF-8")]
    [InlineData("application/soap+xml;charset=UTF-8;action=\"http://myCompany.Customer.Contract/ICustomerService/GetSomeConfiguration\"", "application/soap+xml; charset=UTF-8; action=\"http://myCompany.Customer.Contract/ICustomerService/GetSomeConfiguration\"")]
    public void StringContentHelper_Create(string test, string expected)
    {
        // Arrange
        var contentType = MediaTypeHeaderValue.Parse(test);

        // Act
        var result = StringContentHelper.Create("test", contentType);

        // Assert
        result.Headers.ContentType.ToString().Should().Be(expected);
        result.ReadAsStringAsync().Result.Should().Be("test");
    }
}