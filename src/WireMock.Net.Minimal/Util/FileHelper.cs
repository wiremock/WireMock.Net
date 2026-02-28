// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using Stef.Validation;
using WireMock.Handlers;

namespace WireMock.Util;

internal static class FileHelper
{
    private const int NumberOfRetries = 3;
    private const int DelayOnRetry = 500;

    public static bool TryReadMappingFileWithRetryAndDelay(IFileSystemHandler handler, string path, [NotNullWhen(true)] out string? value)
    {
        Guard.NotNull(handler);
        Guard.NotNullOrEmpty(path);

        value = null;

        for (int i = 1; i <= NumberOfRetries; ++i)
        {
            try
            {
                value = handler.ReadMappingFile(path);
                return true;
            }
            catch
            {
                Thread.Sleep(DelayOnRetry);
            }
        }

        return false;
    }
}