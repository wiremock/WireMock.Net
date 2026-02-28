// Copyright © WireMock.Net

using System.Collections.Specialized;
using WireMock.Admin.Mappings;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Types;

namespace WireMock.Server;

/// <summary>
/// The fluent mock server interface.
/// </summary>
public interface IWireMockServer : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether this server is started.
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    /// Gets a value indicating whether this server is started with the admin interface enabled.
    /// </summary>
    bool IsStartedWithAdminInterface { get; }

    /// <summary>
    /// Gets the request logs.
    /// </summary>
    IReadOnlyList<ILogEntry> LogEntries { get; }

    /// <summary>
    /// Gets the mappings as MappingModels.
    /// </summary>
    IReadOnlyList<MappingModel> MappingModels { get; }

    // <summary>
    // Gets the mappings.
    // </summary>
    //[PublicAPI]
    //IEnumerable<IMapping> Mappings { get; }

    /// <summary>
    /// Gets the ports.
    /// </summary>
    List<int> Ports { get; }

    /// <summary>
    /// Gets the first port.
    /// </summary>
    int Port { get; }

    /// <summary>
    /// Gets the urls.
    /// </summary>
    string[] Urls { get; }

    /// <summary>
    /// Gets the first url.
    /// </summary>
    string? Url { get; }

    /// <summary>
    /// Gets the consumer.
    /// </summary>
    string? Consumer { get; }

    /// <summary>
    /// Gets the provider.
    /// </summary>
    string? Provider { get; }

    //ConcurrentDictionary<string, ScenarioState> Scenarios { get; }

    /// <summary>
    /// Occurs when [log entries changed].
    /// </summary>
    event NotifyCollectionChangedEventHandler LogEntriesChanged;

    /// <summary>
    /// Adds a 'catch all mapping'
    /// 
    /// - matches all Paths and any Methods
    /// - priority is set to 1000
    /// - responds with a 404 "No matching mapping found"
    /// </summary>
    void AddCatchAllMapping();

    /// <summary>
    /// The add request processing delay.
    /// </summary>
    /// <param name="delay">The delay.</param>
    void AddGlobalProcessingDelay(TimeSpan delay);

    /// <summary>
    /// Allows the partial mapping.
    /// </summary>
    void AllowPartialMapping(bool allow = true);

    /// <summary>
    /// Deletes a LogEntry.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    bool DeleteLogEntry(Guid guid);

    /// <summary>
    /// Deletes the mapping.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    bool DeleteMapping(Guid guid);

    /// <summary>
    /// Search log-entries based on matchers.
    /// </summary>
    /// <param name="matchers">The request matchers to use.</param>
    /// <returns>The <see cref="IReadOnlyList{ILogEntry}"/>.</returns>
    IReadOnlyList<ILogEntry> FindLogEntries(params IRequestMatcher[] matchers);

    // IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false);

    /// <summary>
    /// Reads a static mapping file and adds or updates a single mapping.
    /// 
    /// Calling this method manually forces WireMock.Net to read and apply the specified static mapping file.
    /// </summary>
    /// <param name="path">The path to the static mapping file.</param>
    bool ReadStaticMappingAndAddOrUpdate(string path);

    /// <summary>
    /// Reads the static mappings from a folder.
    /// (This method is also used when WireMockServerSettings.ReadStaticMappings is set to true.
    /// 
    /// Calling this method manually forces WireMock.Net to read and apply all static mapping files in the specified folder.
    /// </summary>
    /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
    void ReadStaticMappings(string? folder = null);

    /// <summary>
    /// Removes the authentication.
    /// </summary>
    void RemoveAuthentication();

    /// <summary>
    /// Resets LogEntries, Mappings and Scenarios.
    /// </summary>
    void Reset();

    /// <summary>
    /// Resets the Mappings.
    /// </summary>
    void ResetMappings();

    /// <summary>
    /// Resets all Scenarios.
    /// </summary>
    void ResetScenarios();

    /// <summary>
    /// Resets a specific Scenario by the name.
    /// </summary>
    bool ResetScenario(string name);

    /// <summary>
    /// Sets a scenario to a state.
    /// </summary>
    bool SetScenarioState(string name, string? state);

    /// <summary>
    /// Resets the LogEntries.
    /// </summary>
    void ResetLogEntries();

    /// <summary>
    /// Saves the static mappings.
    /// </summary>
    /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
    void SaveStaticMappings(string? folder = null);

    /// <summary>
    /// Sets the basic authentication.
    /// </summary>
    /// <param name="tenant">The Tenant.</param>
    /// <param name="audience">The Audience or Resource.</param>
    void SetAzureADAuthentication(string tenant, string audience);

    /// <summary>
    /// Sets the basic authentication.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    void SetBasicAuthentication(string username, string password);

    /// <summary>
    /// Sets the maximum RequestLog count.
    /// </summary>
    /// <param name="maxRequestLogCount">The maximum RequestLog count.</param>
    void SetMaxRequestLogCount(int? maxRequestLogCount);

    /// <summary>
    /// Sets RequestLog expiration in hours.
    /// </summary>
    /// <param name="requestLogExpirationDuration">The RequestLog expiration in hours.</param>
    void SetRequestLogExpirationDuration(int? requestLogExpirationDuration);

    /// <summary>
    /// Stop this server.
    /// </summary>
    void Stop();

    /// <summary>
    /// Watches the static mappings for changes.
    /// </summary>
    /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
    void WatchStaticMappings(string? folder = null);

    /// <summary>
    /// Register the mappings (via <see cref="MappingModel"/>).
    /// 
    /// This can be used if you have 1 or more <see cref="MappingModel"/> defined and want to register these in WireMock.Net directly instead of using the fluent syntax.
    /// </summary>
    /// <param name="mappings">The MappingModels</param>
    /// <returns><see cref="IWireMockServer"/></returns>
    IWireMockServer WithMapping(params MappingModel[] mappings);

    /// <summary>
    /// Register the mappings (via json string).
    /// 
    /// This can be used if you've the mappings as json string defined and want to register these in WireMock.Net directly instead of using the fluent syntax.
    /// </summary>
    /// <param name="mappings">The mapping(s) as json string.</param>
    /// <returns><see cref="IWireMockServer"/></returns>
    IWireMockServer WithMapping(string mappings);

    /// <summary>
    /// Get the C# code for a mapping.
    /// </summary>
    /// <param name="guid">The Mapping Guid.</param>
    /// <param name="converterType">The <see cref="MappingConverterType"/></param>
    /// <returns>C# code (null in case the mapping is not found)</returns>
    string? MappingToCSharpCode(Guid guid, MappingConverterType converterType);

    /// <summary>
    /// Get the C# code for all mappings.
    /// </summary>
    /// <param name="converterType">The <see cref="MappingConverterType"/></param>
    /// <returns>C# code</returns>
    string MappingsToCSharpCode(MappingConverterType converterType);
}