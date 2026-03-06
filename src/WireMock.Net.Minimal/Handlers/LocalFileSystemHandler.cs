// Copyright © WireMock.Net

using WireMock.Util;
using Stef.Validation;

namespace WireMock.Handlers;

/// <summary>
/// Default implementation for a handler to interact with the local file system to read and write static mapping files.
/// </summary>
public class LocalFileSystemHandler : IFileSystemHandler
{
    private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");
    private static readonly string UnmatchedRequestsFolder = Path.Combine("requests", "unmatched");

    private readonly string _rootFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileSystemHandler"/> class.
    /// </summary>
    public LocalFileSystemHandler() : this(Directory.GetCurrentDirectory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileSystemHandler"/> class.
    /// </summary>
    /// <param name="rootFolder">The root folder.</param>
    public LocalFileSystemHandler(string rootFolder)
    {
        _rootFolder = rootFolder;
    }

    /// <inheritdoc cref="IFileSystemHandler.FolderExists"/>
    public virtual bool FolderExists(string path)
    {
        Guard.NotNullOrEmpty(path);

        return Directory.Exists(path);
    }

    /// <inheritdoc cref="IFileSystemHandler.CreateFolder"/>
    public virtual void CreateFolder(string path)
    {
        Guard.NotNullOrEmpty(path);

        Directory.CreateDirectory(path);
    }

    /// <inheritdoc cref="IFileSystemHandler.EnumerateFiles"/>
    public virtual IEnumerable<string> EnumerateFiles(string path, bool includeSubdirectories)
    {
        Guard.NotNullOrEmpty(path);

        return includeSubdirectories ? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories) : Directory.EnumerateFiles(path);
    }

    /// <inheritdoc cref="IFileSystemHandler.GetMappingFolder"/>
    public virtual string GetMappingFolder()
    {
        return Path.Combine(_rootFolder, AdminMappingsFolder);
    }

    /// <inheritdoc cref="IFileSystemHandler.ReadMappingFile"/>
    public virtual string ReadMappingFile(string path)
    {
        Guard.NotNullOrEmpty(path);

        return File.ReadAllText(path);
    }

    /// <inheritdoc cref="IFileSystemHandler.WriteMappingFile(string, string)"/>
    public virtual void WriteMappingFile(string path, string text)
    {
        Guard.NotNullOrEmpty(path);
        Guard.NotNull(text);

        File.WriteAllText(path, text);
    }

    /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsFile"/>
    public virtual byte[] ReadResponseBodyAsFile(string path)
    {
        Guard.NotNullOrEmpty(path);
        path = FilePathUtils.CleanPath(path)!;
        // If the file exists at the given path relative to the MappingsFolder, then return that.
        // Else the path will just be as-is.
        return File.ReadAllBytes(File.Exists(FilePathUtils.Combine(GetMappingFolder(), path)) ? FilePathUtils.Combine(GetMappingFolder(), path) : path);
    }

    /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsString"/>
    public virtual string ReadResponseBodyAsString(string path)
    {
        Guard.NotNullOrEmpty(path);
        path = FilePathUtils.CleanPath(path)!;
        // In case the path is a filename, the path will be adjusted to the MappingFolder.
        // Else the path will just be as-is.
        return File.ReadAllText(File.Exists(FilePathUtils.Combine(GetMappingFolder(), path)) ? FilePathUtils.Combine(GetMappingFolder(), path) : path);
    }

    /// <inheritdoc cref="IFileSystemHandler.FileExists"/>
    public virtual bool FileExists(string filename)
    {
        Guard.NotNullOrEmpty(filename);

        return File.Exists(AdjustPathForMappingFolder(filename));
    }

    /// <inheritdoc />
    public virtual void WriteFile(string filename, byte[] bytes)
    {
        Guard.NotNullOrEmpty(filename);
        Guard.NotNull(bytes);

        File.WriteAllBytes(AdjustPathForMappingFolder(filename), bytes);
    }

    /// <inheritdoc />
    public virtual void WriteFile(string folder, string filename, byte[] bytes)
    {
        Guard.NotNullOrEmpty(folder);
        Guard.NotNullOrEmpty(filename);
        Guard.NotNull(bytes);

        File.WriteAllBytes(FilePathUtils.Combine(folder, filename), bytes);
    }

    /// <inheritdoc cref="IFileSystemHandler.DeleteFile"/>
    public virtual void DeleteFile(string filename)
    {
        Guard.NotNullOrEmpty(filename);

        File.Delete(AdjustPathForMappingFolder(filename));
    }

    /// <inheritdoc cref="IFileSystemHandler.ReadFile"/>
    public virtual byte[] ReadFile(string filename)
    {
        Guard.NotNullOrEmpty(filename);

        return File.ReadAllBytes(AdjustPathForMappingFolder(filename));
    }

    /// <inheritdoc cref="IFileSystemHandler.ReadFileAsString"/>
    public virtual string ReadFileAsString(string filename)
    {
        return File.ReadAllText(AdjustPathForMappingFolder(Guard.NotNullOrEmpty(filename)));
    }

    /// <inheritdoc cref="IFileSystemHandler.GetUnmatchedRequestsFolder"/>
    public virtual string GetUnmatchedRequestsFolder()
    {
        return Path.Combine(_rootFolder, UnmatchedRequestsFolder);
    }

    /// <inheritdoc cref="IFileSystemHandler.WriteUnmatchedRequest"/>
    public virtual void WriteUnmatchedRequest(string filename, string text)
    {
        Guard.NotNullOrEmpty(filename);
        Guard.NotNull(text);

        var folder = GetUnmatchedRequestsFolder();
        if (!FolderExists(folder))
        {
            CreateFolder(folder);
        }

        File.WriteAllText(Path.Combine(folder, filename), text);
    }

    /// <summary>
    /// Adjusts the path to the MappingFolder.
    /// </summary>
    /// <param name="filename">The path.</param>
    /// <returns>Adjusted path</returns>
    private string AdjustPathForMappingFolder(string filename)
    {
        return Path.Combine(GetMappingFolder(), filename);
    }
}