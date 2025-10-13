// Copyright © WireMock.Net

using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using WireMock.Net.ConsoleApplication;

namespace WireMock.Net.Console.NET8;

static class Program
{
    private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    static async Task Main(params string[] args)
    {
        XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

        await MainApp.RunAsync();
    }
}