// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

[UsesVerify]
public partial class MappingConverterTests
{
    private static readonly VerifySettings VerifySettings = new();
    static MappingConverterTests()
    {
        VerifySettings.Init();
    }

    [Fact]
    public Task ToCSharpCode_With_Builder_And_AddStartIsTrue()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = true,
            ConverterType = MappingConverterType.Builder
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verifier.Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_Builder_And_AddStartIsFalse()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Builder
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verifier.Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_Server_And_AddStartIsTrue()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = true,
            ConverterType = MappingConverterType.Server
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verifier.Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_Server_And_AddStartIsFalse()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Server
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verifier.Verify(code, VerifySettings);
    }

    private IMapping CreateMapping()
    {
        var guid = new Guid("8e7b9ab7-e18e-4502-8bc9-11e6679811cc");
        var request = Request.Create()
            .UsingGet()
            .WithPath("/test_path")
            .WithParam("q", "42")
            .WithClientIP("112.123.100.99")
            .WithHeader("h-key", "h-value")
            .WithCookie("c-key", "c-value")
            .WithBody("b");
        var response = Response.Create()
            .WithHeader("Keep-Alive", "test")
            .WithBody("bbb")
            .WithDelay(12345)
            .WithTransformer();

        return new Mapping
        (
            guid,
            _updatedAt,
            string.Empty,
            string.Empty,
            null,
            _settings,
            request,
            response,
            42,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            data: null
        ).WithProbability(0.3);
    }
}
#endif