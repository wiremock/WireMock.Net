// Copyright © WireMock.Net

using System.IO;
using Stef.Validation;

namespace WireMock.Util;

internal static class FilePathUtils
{
    /// <summary>
    /// Robust handling of the user defined path.
    /// Also supports Unix and Windows platforms
    /// </summary>
    /// <param name="path">The path to clean</param>
    public static string? CleanPath(string? path)
    {
        return path?.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Removes leading directory separator chars from the filepath, which could break Path.Combine
    /// </summary>
    /// <param name="path">The path to remove the loading DirectorySeparatorChars</param>
    public static string? RemoveLeadingDirectorySeparators(string? path)
    {
        return path?.TrimStart(Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Combine two paths
    /// </summary>
    /// <param name="root">The root path</param>
    /// <param name="path">The path</param>
    public static string Combine(string root, string? path)
    {
        Guard.NotNull(root);

        var result = RemoveLeadingDirectorySeparators(path);
        return result == null ? root : Path.Combine(root, result);
    }

    /// <summary>
    /// Returns a relative path from one path to another.
    /// </summary>
    /// <param name="relativeTo">The source path the result should be relative to. This path is always considered to be a directory..</param>
    /// <param name="path">The destination path.</param>
    /// <returns>The relative path, or path if the paths don't share the same root.</returns>
    public static string GetRelativePath(string relativeTo, string path)
    {
#if NETCOREAPP3_1 || NET5_0_OR_GREATER || NETSTANDARD2_1
        return Path.GetRelativePath(relativeTo, path);
#else
        Guard.NotNull(relativeTo);
        Guard.NotNull(path);

        static string AppendDirectorySeparatorChar(string path)
        {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        var fromUri = new System.Uri(AppendDirectorySeparatorChar(relativeTo));
        var toUri = new System.Uri(AppendDirectorySeparatorChar(path));

        if (fromUri.Scheme != toUri.Scheme)
        {
            return path;
        }

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

        if (string.Equals(toUri.Scheme, "FILE", System.StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        return relativePath;
#endif
    }
}