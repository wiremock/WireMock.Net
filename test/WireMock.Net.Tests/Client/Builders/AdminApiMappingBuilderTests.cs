// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests.Client.Builders;

[ExcludeFromCodeCoverage]
[UsesVerify]
public class AdminApiMappingBuilderTests
{
    private static readonly VerifySettings VerifySettings = new();
    static AdminApiMappingBuilderTests()
    {
        VerifyNewtonsoftJson.Enable(VerifySettings);
    }

    [Fact]
    public async Task GetMappingBuilder_BuildAndPostAsync()
    {
        using var server = WireMockServer.StartWithAdminInterface();

        var api = RestEase.RestClient.For<IWireMockAdminApi>(server.Url!);

        var guid = Guid.Parse("53241df5-582c-458a-a67b-6de3d1d0508e");
        var mappingBuilder = api.GetMappingBuilder();
        mappingBuilder.Given(m => m
            .WithTitle("This is my title 1")
            .WithGuid(guid)
            .WithRequest(req => req
                .UsingPost()
                .WithPath("/bla1")
                .WithHeader("Authorization", "*", true)
                .WithBody(body => body
                    .WithMatcher(matcher => matcher
                        .WithName("JsonPartialMatcher")
                        .WithPattern(new { test = "abc" })
                    )
                )
            )
            .WithResponse(rsp => rsp
                .WithBody("The Response")
            )
        );

        // Act
        var status = await mappingBuilder.BuildAndPostAsync();

        // Assert
        status.Status.Should().Be("Mapping added");

        var getMappingResult = await api.GetMappingAsync(guid);

        await Verifier.Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }
}
#endif