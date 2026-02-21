// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Admin.Mappings;

namespace WireMock.Client.Builders;

/// <summary>
/// AdminApiMappingBuilder
/// </summary>
public class AdminApiMappingBuilder
{
    private readonly List<Action<MappingModelBuilder>> _mappingModelBuilderActions = new();

    private readonly IWireMockAdminApi _api;

    /// <summary>
    /// AdminApiMappingBuilder
    /// </summary>
    /// <param name="api">The <see cref="IWireMockAdminApi"/>.</param>
    public AdminApiMappingBuilder(IWireMockAdminApi api)
    {
        _api = Guard.NotNull(api);
    }

    /// <summary>
    /// The Given
    /// </summary>
    /// <param name="mappingModelBuilderAction">The action.</param>
    public void Given(Action<MappingModelBuilder> mappingModelBuilderAction)
    {
        _mappingModelBuilderActions.Add(Guard.NotNull(mappingModelBuilderAction));
    }

    /// <summary>
    /// Build the mappings and post these using the <see cref="IWireMockAdminApi"/> to the WireMock.Net server.
    /// </summary>
    /// <param name="cancellationToken">The optional CancellationToken.</param>
    /// <returns><see cref="StatusModel"/></returns>
    public Task<StatusModel> BuildAndPostAsync(CancellationToken cancellationToken = default)
    {
        var modelMappings = new List<MappingModel>();

        foreach (var mappingModelBuilderAction in _mappingModelBuilderActions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mappingModelBuilder = new MappingModelBuilder();
            mappingModelBuilderAction(mappingModelBuilder);

            modelMappings.Add(mappingModelBuilder.Build());
        }

        return _api.PostMappingsAsync(modelMappings, cancellationToken);
    }
}