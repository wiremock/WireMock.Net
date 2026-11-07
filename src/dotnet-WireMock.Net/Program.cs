// Copyright © WireMock.Net

using Microsoft.Extensions.Logging;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Server;

namespace WireMock.Net;

public class Program
{
    private static readonly int SleepTime = 30000;
    private static readonly ILogger XLogger = LoggerFactory.Create(o =>
    {
        o.SetMinimumLevel(LogLevel.Debug);
        o.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = false;
            options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
        });
    }).CreateLogger("WireMock.Net");
    private static readonly IWireMockLogger Logger = new WireMockLogger(XLogger);

    private static WireMockServer _server = null!;
    private static int _stopped;

    static async Task Main(string[] args)
    {
        if (!StandAloneApp.TryStart(args, out _server!, Logger))
        {
            return;
        }

        using var cancelationTokenSource = new CancellationTokenSource();

        Logger.Info("Press Ctrl+C to shut down");

        Console.CancelKeyPress += (s, e) =>
        {
            // Prevent the runtime from terminating the process (exit code 130 / -1073741510)
            // Main stops the server and returns normally.
            e.Cancel = true;
            Logger.Info("Shutdown requested because 'CancelKeyPress'");
            cancelationTokenSource.Cancel();
        };

        System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += ctx =>
        {
            Stop("AssemblyLoadContext.Default.Unloading", cancelationTokenSource);
        };

        while (!cancelationTokenSource.IsCancellationRequested)
        {
            Logger.Info("Server running : {0}", _server.IsStarted);

            try
            {
                await Task.Delay(SleepTime, cancelationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Shutdown requested
            }
        }

        Stop("shutdown requested", cancelationTokenSource);
    }

    private static void Stop(string why, CancellationTokenSource cancellationTokenSource)
    {
        if (Interlocked.Exchange(ref _stopped, 1) == 1)
        {
            return;
        }

        Logger.Info("Server stopping because '{0}'", why);
        _server.Stop();
        Logger.Info("Server stopped");

        try
        {
            cancellationTokenSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // Main already finished
        }
    }
}