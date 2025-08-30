// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Admin.Scenarios;
using WireMock.Admin.Settings;
using WireMock.Types;

namespace WireMock.Client;

/// <summary>
/// The RestEase interface which defines all admin commands.
/// </summary>
[BasePath("__admin")]
public interface IWireMockAdminApi
{
    /// <summary>
    /// Authentication header
    /// </summary>
    [Header("Authorization")]
    AuthenticationHeaderValue Authorization { get; set; }

    /// <summary>
    /// Get health status.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    /// <returns>
    /// Returns HttpStatusCode <c>200</c> with a value <c>Healthy</c> to indicate that WireMock.Net is healthy.
    /// Else it returns HttpStatusCode <c>404</c>.
    /// </returns>
    [Get("health")]
    [AllowAnyStatusCode]
    Task<string> GetHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the settings.
    /// </summary>
    /// <returns>SettingsModel</returns>
    [Get("settings")]
    Task<SettingsModel> GetSettingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the settings.
    /// </summary>
    /// <param name="settings">SettingsModel</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Put("settings")]
    [Header("Content-Type", "application/json")]
    Task<StatusModel> PutSettingsAsync([Body] SettingsModel settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the settings
    /// </summary>
    /// <param name="settings">SettingsModel</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("settings")]
    [Header("Content-Type", "application/json")]
    Task<StatusModel> PostSettingsAsync([Body] SettingsModel settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the mappings.
    /// </summary>
    /// <returns>MappingModels</returns>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("mappings")]
    Task<IList<MappingModel>> GetMappingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the C# code from all mappings
    /// </summary>
    /// <returns>C# code</returns>
    /// <param name="mappingConverterType">The <see cref="MappingConverterType"/>, default is Server.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("mappings/code")]
    Task<string> GetMappingsCodeAsync([Query] MappingConverterType mappingConverterType = MappingConverterType.Server, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new mapping.
    /// </summary>
    /// <param name="mapping">MappingModel</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("mappings")]
    [Header("Content-Type", "application/json")]
    Task<StatusModel> PostMappingAsync([Body] MappingModel mapping, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new mappings.
    /// </summary>
    /// <param name="mappings">MappingModels</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("mappings")]
    [Header("Content-Type", "application/json")]
    Task<StatusModel> PostMappingsAsync([Body] IList<MappingModel> mappings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all mappings.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("mappings")]
    Task<StatusModel> DeleteMappingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete mappings according to GUIDs
    /// </summary>
    /// <param name="mappings">MappingModels</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("mappings")]
    [Header("Content-Type", "application/json")]
    Task<StatusModel> DeleteMappingsAsync([Body] IList<MappingModel> mappings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete (reset) all mappings.
    /// </summary>
    /// <param name="reloadStaticMappings">A value indicating whether to reload the static mappings after the reset.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("mappings/reset")]
    Task<StatusModel> ResetMappingsAsync(bool? reloadStaticMappings = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reload the static mappings.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("mappings/reloadStaticMappings")]
    Task<StatusModel> ReloadStaticMappingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a mapping based on the guid.
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <returns>MappingModel</returns>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("mappings/{guid}")]
    Task<MappingModel> GetMappingAsync([Path] Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a mapping based on the guid.
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <returns>MappingModel</returns>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("mappings/{guid}")]
    Task<MappingModel> GetMappingAsync([Path] string guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the C# code from a mapping based on the guid
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <param name="mappingConverterType">The optional mappingConverterType (can be Server or Builder)</param>
    /// <returns>C# code</returns>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("mappings/code/{guid}")]
    Task<string> GetMappingCodeAsync([Path] Guid guid, [Query] MappingConverterType mappingConverterType = MappingConverterType.Server, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a mapping based on the guid
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <param name="mapping">MappingModel</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Put("mappings/{guid}")]
    [Header("Content-Type", "application/json")]
    Task<StatusModel> PutMappingAsync([Path] Guid guid, [Body] MappingModel mapping, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a mapping based on the guid
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("mappings/{guid}")]
    Task<StatusModel> DeleteMappingAsync([Path] Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save the mappings
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("mappings/save")]
    Task<StatusModel> SaveMappingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the requests.
    /// </summary>
    /// <returns>LogRequestModels</returns>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("requests")]
    Task<IList<LogEntryModel>> GetRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all requests.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("requests")]
    Task<StatusModel> DeleteRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete (reset) all requests.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("requests/reset")]
    Task<StatusModel> ResetRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a request based on the guid
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <returns>LogEntryModel</returns>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("requests/{guid}")]
    Task<LogEntryModel> GetRequestAsync([Path] Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a request based on the guid
    /// </summary>
    /// <param name="guid">The Guid</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("requests/{guid}")]
    Task<StatusModel> DeleteRequestAsync([Path] Guid guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find requests based on the criteria (<see cref="RequestModel"/>)
    /// </summary>
    /// <param name="model">The RequestModel</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("requests/find")]
    [Header("Content-Type", "application/json")]
    Task<IList<LogEntryModel>> FindRequestsAsync([Body] RequestModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find requests based on the Mapping Guid.
    /// </summary>
    /// <param name="mappingGuid">The Mapping Guid</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("requests/find")]
    Task<IList<LogEntryModel>> FindRequestsByMappingGuidAsync([Query] Guid mappingGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all scenarios
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("scenarios")]
    Task<IList<ScenarioStateModel>> GetScenariosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete (reset) all scenarios
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("scenarios")]
    Task<StatusModel> DeleteScenariosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete (reset) all scenarios
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("scenarios/reset")]
    Task<StatusModel> ResetScenariosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete (reset) a specific scenario
    /// </summary>
    /// <param name="name">Scenario name.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("scenarios/{name}")]
    [AllowAnyStatusCode]
    Task<StatusModel> DeleteScenarioAsync([Path] string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete (reset) a specific scenario
    /// </summary>
    /// <param name="name">Scenario name.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("scenarios/{name}/reset")]
    [AllowAnyStatusCode]
    Task<StatusModel> ResetScenarioAsync([Path] string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the state for a scenario.
    /// </summary>
    /// <param name="name">Scenario name.</param>
    /// <param name="updateModel">Scenario state update model.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Put("scenarios/{name}/state")]
    [AllowAnyStatusCode]
    Task<StatusModel> PutScenarioStateAsync([Path] string name, [Body] ScenarioStateUpdateModel updateModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new File
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="body">The body</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("files/{filename}")]
    Task<StatusModel> PostFileAsync([Path] string filename, [Body] string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing File
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="body">The body</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Put("files/{filename}")]
    Task<StatusModel> PutFileAsync([Path] string filename, [Body] string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the content of an existing File
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Get("files/{filename}")]
    Task<string> GetFileAsync([Path] string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an existing File
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Delete("files/{filename}")]
    Task<StatusModel> DeleteFileAsync([Path] string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a Grpc ProtoDefinition at server-level.
    /// </summary>
    /// <param name="id">Unique identifier for the ProtoDefinition.</param>
    /// <param name="protoDefinition">The ProtoDefinition as text.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("protodefinitions/{id}")]
    Task<StatusModel> AddProtoDefinitionAsync([Path] string id, [Body] string protoDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a file exists
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Head("files/{filename}")]
    Task FileExistsAsync([Path] string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert an OpenApi / RAML document to mappings.
    /// </summary>
    /// <param name="text">The OpenApi or RAML document as text.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("openapi/convert")]
    Task<IReadOnlyList<MappingModel>> OpenApiConvertAsync([Body] string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert an OpenApi / RAML document to mappings and save these.
    /// </summary>
    /// <param name="text">The OpenApi or RAML document as text.</param>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    [Post("openapi/save")]
    Task<StatusModel> OpenApiSaveAsync([Body] string text, CancellationToken cancellationToken = default);
}
