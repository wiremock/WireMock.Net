// Copyright © WireMock.Net

using log4net.Config;
using System;
using System.IO;
using System.ServiceProcess;
using WireMock.Net.Service;
using WireMock.Server;
using WireMock.Settings;

namespace Wiremock.Net.Service
{
    public static class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "WireMock.Net.Service";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Start();
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        private static WireMockServer _server = null!;

        static void Main(string[] args)
        {
            //Setting the current directory explicitly is required if the application is running as Windows Service, 
            //as the current directory of a Windows Service is %WinDir%\System32 per default.
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            // running as service
            if (!Environment.UserInteractive)
            {
                using var service = new Service();
                ServiceBase.Run(service);
            }
            else
            {
                // running as console app
                Start();

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        private static void Start()
        {
            _server = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = [ "http://*:9091/" ],
                StartAdminInterface = true,
                ReadStaticMappings = true,
                Logger = new WireMockLog4NetLogger()
            });
        }

        private static void Stop()
        {
            _server.Stop();
        }
    }
}