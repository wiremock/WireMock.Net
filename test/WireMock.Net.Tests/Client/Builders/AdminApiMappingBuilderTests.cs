// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.Server;

namespace WireMock.Net.Tests.Client.Builders;

[ExcludeFromCodeCoverage]
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
        var ct = TestContext.Current.CancellationToken;

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
        var status = await mappingBuilder.BuildAndPostAsync(ct);

        // Assert
        status.Status.Should().Be("Mapping added");

        var getMappingResult = await api.GetMappingAsync(guid, ct);

        await Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }
}