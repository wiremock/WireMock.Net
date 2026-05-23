// Copyright © WireMock.Net

using System.Runtime.InteropServices;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Newtonsoft.Json;
using WireMock.Net.Testcontainers;

namespace WireMock.Net.TestcontainersExample;

internal class Program
{
    private static readonly ConsoleColor OriginalColor = Console.ForegroundColor;

    private static async Task Main(string[] args)
    {
        await TestLinux();

        await TestAutomatic();

        await TestLinuxWithVersionTag();

        await TestLinuxAlpineWithVersionTag();

        await TestWindowsWithVersionTag();

        await TestWindows();

        await TestCopy();
    }

    private static async Task TestWindows()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("WithWindows");
            await TestAsync("WithWindows");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestWindowsWithVersionTag()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Windows");
            await TestAsync("sheyenrath/wiremock.net-windows:1.6.5");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestLinux()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("WithLinux");
            await TestAsync("WithLinux");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestLinuxAlpineWithVersionTag()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Linux Alpine");
            await TestAsync("sheyenrath/wiremock.net-alpine:1.6.5");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestLinuxWithVersionTag()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Linux");
            await TestAsync("sheyenrath/wiremock.net:1.6.5");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestAutomatic()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Automatic");
            await TestAsync();
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestCopy()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Copy");
            await TestWindowsCopyAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = OriginalColor;
        }
    }

    private static async Task TestWindowsCopyAsync()
    {
        var builder = new WireMockContainerBuilder()
            .WithWatchStaticMappings(true);

        var container = builder.Build();

        await container.StartAsync();

        if (await GetImageOSAsync.Value == OSPlatform.Linux)
        {
            try
            {
                await container.CopyAsync(@"C:\temp-wiremock\__admin\mappings\StefBodyAsFileExample.json", "/app/__admin/mappings");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        Console.WriteLine("PublicUrl = " + container.GetPublicUrl());

        var adminClient = container.CreateWireMockAdminClient();

        var mappings = await adminClient.GetMappingsAsync();
        Console.WriteLine("mappings = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

        await container.StopAsync();
    }

    private static async Task TestAsync(string? image = null)
    {
        var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "WireMock.Net.Console.NET6", "__admin", "mappings");

        var dummyNetwork = new NetworkBuilder()
            .WithName($"Dummy Network for {image ?? "null"}")
            .WithReuse(true)
            // .WithCleanUp(true)
            .Build();

        var builder = new WireMockContainerBuilder()
            .WithNetwork(dummyNetwork)
            .WithAdminUserNameAndPassword("x", "y")
            .WithMappings(mappingsPath)
            .WithWatchStaticMappings(true);

        if (image != null)
        {
            builder = image switch
            {
                "WithWindows" => builder.WithWindowsImage(),
                "WithLinux" => builder.WithLinuxImage(),
                _ => builder.WithImage(image)
            };
        }

        var container = builder.Build();

        await container.StartAsync();

        await container.ReloadStaticMappingsAsync();

        //var logs = await container.GetLogsAsync(DateTime.Now.AddDays(-1));
        //Console.WriteLine("logs = " + logs.Stdout);

        Console.WriteLine("PublicUrl = " + container.GetPublicUrl());

        var restEaseApiClient = container.CreateWireMockAdminClient();

        var settings = await restEaseApiClient.GetSettingsAsync();
        Console.WriteLine("settings = " + JsonConvert.SerializeObject(settings, Formatting.Indented));

        var mappings = await restEaseApiClient.GetMappingsAsync();
        Console.WriteLine("mappingsStef = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

        var client = container.CreateClient();
        var result = await client.GetStringAsync("/static/mapping");
        Console.WriteLine("result = " + result);

        await container.StopAsync();
    }

    private static readonly Lazy<Task<OSPlatform>> GetImageOSAsync = new(async () =>
    {
        if (TestcontainersSettings.OS.DockerEndpointAuthConfig == null)
        {
            throw new InvalidOperationException($"The {nameof(TestcontainersSettings.OS.DockerEndpointAuthConfig)} is null. Check if Docker is started.");
        }

        var dockerClientBuilder = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder();
        using var dockerClient = dockerClientBuilder.Build();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
    });
}