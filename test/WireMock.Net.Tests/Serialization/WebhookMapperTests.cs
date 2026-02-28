// Copyright © WireMock.Net

using WireMock.Admin.Mappings;
using WireMock.Models;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.Serialization;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.Serialization;

public class WebhookMapperTests
{
    private static readonly VerifySettings VerifySettings = new();
    static WebhookMapperTests()
    {
        VerifySettings.Init();
    }

    [Fact]
    public Task WebhookMapper_Map_WebhookModel_BodyAsString_And_UseTransformerIsFalse()
    {
        // Assign
        var model = new WebhookModel
        {
            Request = new WebhookRequestModel
            {
                Url = "https://localhost",
                Method = "get",
                Headers = new Dictionary<string, string>
                {
                    { "x", "y" }
                },
                Body = "test",
                UseTransformer = false
            }
        };

        var result = WebhookMapper.Map(model);

        // Verify
        return Verify(result, VerifySettings);
    }

    [Fact]
    public Task WebhookMapper_Map_WebhookModel_BodyAsString_And_UseTransformerIsTrue()
    {
        // Assign
        var model = new WebhookModel
        {
            Request = new WebhookRequestModel
            {
                Url = "https://localhost",
                Method = "get",
                Headers = new Dictionary<string, string>
                {
                    { "x", "y" }
                },
                Body = "test",
                UseTransformer = true
            }
        };

        var result = WebhookMapper.Map(model);

        // Verify
        return Verify(result, VerifySettings);
    }

    [Fact]
    public Task WebhookMapper_Map_WebhookModel_BodyAsJson()
    {
        // Assign
        var model = new WebhookModel
        {
            Request = new WebhookRequestModel
            {
                Url = "https://localhost",
                Method = "get",
                Headers = new Dictionary<string, string>
                {
                    { "x", "y" }
                },
                BodyAsJson = new { n = 12345 },
                Delay = 4,
                MinimumRandomDelay = 5,
                MaximumRandomDelay = 6
            },
        };

        var result = WebhookMapper.Map(model);

        // Verify
        return Verify(result, VerifySettings);
    }

    [Fact]
    public Task WebhookMapper_Map_Webhook_To_Model()
    {
        // Assign
        var webhook = new Webhook
        {
            Request = new WebhookRequest
            {
                Url = "https://localhost",
                Method = "get",
                Headers = new Dictionary<string, WireMockList<string>>
                {
                    { "x", new WireMockList<string>("y") }
                },
                BodyData = new BodyData
                {
                    BodyAsJson = new { n = 12345 },
                    DetectedBodyType = BodyType.Json,
                    DetectedBodyTypeFromContentType = BodyType.Json
                },
                Delay = 4,
                MinimumRandomDelay = 5,
                MaximumRandomDelay = 6
            }
        };

        var result = WebhookMapper.Map(webhook);

        // Verify
        return Verify(result, VerifySettings);
    }
}