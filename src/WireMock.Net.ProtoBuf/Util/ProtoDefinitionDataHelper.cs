// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Util;

/// <summary>
/// Some helper methods for Proto Definitions.
/// </summary>
internal static class ProtoDefinitionDataHelper
{
    /// <summary>
    /// Builds a dictionary of ProtoDefinitions from a directory.
    /// - The key will be the filename without extension.
    /// - The value will be the ProtoDefinition with an extra comment with the relative path to each <c>.proto</c> file so it can be used by the WireMockProtoFileResolver.
    /// </summary>
    /// <param name="directory">The directory to start from.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <c>System.Threading.CancellationToken.None</c>.</param>
    public static async Task<ProtoDefinitionData> FromDirectory(string directory, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrEmpty(directory);

        var fileNameMappedToProtoDefinition = new Dictionary<string, string>();
        var filePaths = Directory.EnumerateFiles(directory, "*.proto", SearchOption.AllDirectories);

        foreach (var filePath in filePaths)
        {
            // Get the relative path to the directory (note that this will be OS specific).
            var relativePath = FilePathUtils.GetRelativePath(directory, filePath);

            // Make it a valid proto import path
            var protoRelativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');

            // Build comment and get content from file.
            var comment = $"// {protoRelativePath}";
#if NET462
            var content = File.ReadAllText(filePath);
#else
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
#endif
            // Only add the comment if it's not already defined.
            var modifiedContent = !content.StartsWith(comment) ? $"{comment}\n{content}" : content;
            var key = Path.GetFileNameWithoutExtension(filePath);

            fileNameMappedToProtoDefinition.Add(key, modifiedContent);
        }

        var converter = SingletonFactory<Converter>.GetInstance();
        var resolver = new WireMockProtoFileResolver(fileNameMappedToProtoDefinition.Values);

        var messageTypeMappedToWithProtoDefinition = new Dictionary<string, string>();

        foreach (var protoDefinition in fileNameMappedToProtoDefinition.Values)
        {
            var infoRequest = new GetInformationRequest(protoDefinition, resolver);

            try
            {
                var info = await converter.GetInformationAsync(infoRequest, cancellationToken);
                foreach (var messageType  in info.MessageTypes)
                {
                    messageTypeMappedToWithProtoDefinition[messageType.Key] = protoDefinition;
                }
            }
            catch
            {
                // Ignore
            }
        }

        return new ProtoDefinitionData(fileNameMappedToProtoDefinition);
    }
}