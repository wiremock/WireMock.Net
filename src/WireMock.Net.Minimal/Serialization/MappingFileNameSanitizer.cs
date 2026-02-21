// Copyright © WireMock.Net

using System.IO;
using System.Linq;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.Serialization;

/// <summary>
/// Creates sanitized file names for mappings
/// </summary>
public class MappingFileNameSanitizer
{
    private const char ReplaceChar = '_';

    private readonly WireMockServerSettings _settings;

    public MappingFileNameSanitizer(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
    }

    /// <summary>
    /// Creates sanitized file names for mappings
    /// </summary>
    public string BuildSanitizedFileName(IMapping mapping)
    {
        string name;
        if (!string.IsNullOrEmpty(mapping.Title))
        {
            // remove 'Proxy Mapping for ' and an extra space character after the HTTP request method
            name = mapping.Title!.Replace(ProxyAndRecordSettings.DefaultPrefixForSavedMappingFile, "").Replace(' '.ToString(), string.Empty);
            if (_settings.ProxyAndRecordSettings?.AppendGuidToSavedMappingFile == true)
            {
                name += $"{ReplaceChar}{mapping.Guid}";
            }
        }
        else
        {
            name = mapping.Guid.ToString();
        }

        if (!string.IsNullOrEmpty(_settings.ProxyAndRecordSettings?.PrefixForSavedMappingFile))
        {
            name = $"{_settings.ProxyAndRecordSettings.PrefixForSavedMappingFile}{ReplaceChar}{name}";
        }
        return $"{Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, ReplaceChar))}.json";
    }
}
