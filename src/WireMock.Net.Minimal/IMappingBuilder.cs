// Copyright © WireMock.Net

using WireMock.Admin.Mappings;
using WireMock.Matchers.Request;
using WireMock.Server;
using WireMock.Types;

namespace WireMock;

/// <summary>
/// IMappingBuilder
/// </summary>
public interface IMappingBuilder
{
    /// <summary>
    /// The given.
    /// </summary>
    /// <param name="requestMatcher">The request matcher.</param>
    /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false);

    /// <summary>
    /// Gets all the mappings as a list.
    /// </summary>
    /// <returns>A list from <see cref="MappingModel"/>s.</returns>
    MappingModel[] GetMappings();

    /// <summary>
    /// Convert all mappings to JSON.
    /// </summary>
    /// <returns>JSON</returns>
    string ToJson();

    /// <summary>
    /// Save all mappings as a single JSON to a file.
    /// </summary>
    /// <param name="path">The file to write to.</param>
    void SaveMappingsToFile(string path);

    /// <summary>
    /// Save all mappings as multiple JSON files (each file is 1 mapping).
    /// </summary>
    /// <param name="folder">The folder to write the files to.</param>
    void SaveMappingsToFolder(string folder);

    /// <summary>
    /// Get the C# code for a mapping.
    /// </summary>
    /// <param name="guid">The Mapping Guid.</param>
    /// <param name="converterType">The <see cref="MappingConverterType"/></param>
    /// <returns>C# code (null in case the mapping is not found)</returns>
    string? ToCSharpCode(Guid guid, MappingConverterType converterType);

    /// <summary>
    /// Get the C# code for all mappings.
    /// </summary>
    /// <param name="converterType">The <see cref="MappingConverterType"/></param>
    /// <returns>C# code</returns>
    public string ToCSharpCode(MappingConverterType converterType);
}