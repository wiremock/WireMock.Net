// Copyright © WireMock.Net

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.WebApplication;

public class WireMockService : IWireMockService
{
    private WireMockServer? _server;
    private readonly ILogger _logger;
    private readonly WireMockServerSettings _settings;

    private class Logger : IWireMockLogger
    {
        private readonly ILogger _logger;

        public Logger(ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string formatString, params object[] args)
        {
            _logger.LogDebug(formatString, args);
        }

        public void Info(string formatString, params object[] args)
        {
            _logger.LogInformation(formatString, args);
        }

        public void Warn(string formatString, params object[] args)
        {
            _logger.LogWarning(formatString, args);
        }

        public void Error(string formatString, params object[] args)
        {
            _logger.LogError(formatString, args);
        }

        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminrequest)
        {
            string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
            _logger.LogDebug("Admin[{0}] {1}", isAdminrequest, message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.LogError(exception, message);
        }
    }

    public WireMockService(ILogger<WireMockService> logger, IOptions<WireMockServerSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        _settings.Logger = new Logger(logger);
    }

    public void Start()
    {
        _logger.LogInformation("WireMock.Net server starting");

        _server = WireMockServer.Start(_settings);

        _logger.LogInformation($"WireMock.Net server settings {JsonConvert.SerializeObject(_settings)}");
    }

    public void Stop()
    {
        _logger.LogInformation("WireMock.Net server stopping");
        _server?.Stop();
    }
}