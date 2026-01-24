// Copyright © WireMock.Net

using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace WireMock.Net.Console.MimePart;

static class Program
{
    private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    static async Task Main(params string[] args)
    {
        Log.Info("Starting WireMock.Net.Console.MimePart...");

        XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

        await MainApp.RunAsync();
    }
}