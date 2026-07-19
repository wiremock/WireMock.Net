// Copyright © WireMock.Net

using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;
using WireMock.Net.ConsoleApplication;

namespace WireMock.Net.Console.NET8;

static class Program
{
    private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);

    static async Task Main(params string[] args)
    {
        XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

        await MainApp.RunAsync();
    }
}