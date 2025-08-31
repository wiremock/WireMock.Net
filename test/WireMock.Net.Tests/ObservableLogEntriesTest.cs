// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NFluent;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests;

public class ObservableLogEntriesTest
{
    [Fact]
    public async Task WireMockServer_LogEntriesChanged_WithException_Should_LogError()
    {
        // Assign
        string path = $"/log_{Guid.NewGuid()}";
        var loggerMock = new Mock<IWireMockLogger>();
        loggerMock.Setup(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>()));
        var settings = new WireMockServerSettings
        {
            Logger = loggerMock.Object
        };
        var server = WireMockServer.Start(settings);

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(@"{ msg: ""Hello world!""}"));

        server.LogEntriesChanged += (sender, args) => throw new Exception();

        // Act
        await new HttpClient().GetAsync($"http://localhost:{server.Ports[0]}{path}");

        // Assert
        loggerMock.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task WireMockServer_LogEntriesChanged()
    {
        // Assign
        string path = $"/log_{Guid.NewGuid()}";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(@"{ msg: ""Hello world!""}"));

        int count = 0;
        server.LogEntriesChanged += (sender, args) => count++;

        // Act 1a
        await server.CreateClient().GetAsync(path);

        // Act 1b
        await server.CreateClient().GetAsync(path);

        // Assert
        count.Should().Be(2);

        server.Dispose();
    }

    [Fact]
    public async Task WireMockServer_LogEntriesChanged_Add_And_Remove_EventHandler()
    {
        // Assign
        string path = $"/log_{Guid.NewGuid()}";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(@"{ msg: ""Hello world!""}"));

        int count = 0;

        void OnServerOnLogEntriesChanged(object sender, NotifyCollectionChangedEventArgs args) => count++;

        // Add Handler
        server.LogEntriesChanged += OnServerOnLogEntriesChanged;

        // Act 1
        await server.CreateClient().GetAsync(path);

        // Assert 1
        count.Should().Be(1);

        // Remove Handler
        server.LogEntriesChanged -= OnServerOnLogEntriesChanged;

        // Act 2
        await server.CreateClient().GetAsync(path);

        // Assert 2
        count.Should().Be(1);

        server.Dispose();
    }

    [Fact]
    public async Task WireMockServer_LogEntriesChanged_Parallel()
    {
        int expectedCount = 10;

        // Assign
        string path = $"/log_p_{Guid.NewGuid()}";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithSuccess());

        int count = 0;
        server.LogEntriesChanged += (sender, args) => count++;

        var http = new HttpClient();

        // Act
        var listOfTasks = new List<Task<HttpResponseMessage>>();
        for (var i = 0; i < expectedCount; i++)
        {
            Thread.Sleep(50);
            listOfTasks.Add(http.GetAsync($"{server.Urls[0]}{path}"));
        }
        var responses = await Task.WhenAll(listOfTasks);
        var countResponsesWithStatusNotOk = responses.Count(r => r.StatusCode != HttpStatusCode.OK);

        // Assert
        Check.That(countResponsesWithStatusNotOk).Equals(0);
        Check.That(count).Equals(expectedCount);

        server.Dispose();
    }
}