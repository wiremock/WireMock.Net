// Copyright © WireMock.Net

using WireMock.Net.Testcontainers;

namespace WireMock.Net.TestContainers.ConsoleApp;

/// <summary>
/// Test if WireMock.Net.Testcontainers is compatible with the latest version of Testcontainers for .NET
/// </summary>
internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            await TestAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static async Task TestAsync()
    {
        var builder = new WireMockContainerBuilder()
            .WithLinuxImage()
            .WithWatchStaticMappings(true);

        _ = builder.Build();
    }
}