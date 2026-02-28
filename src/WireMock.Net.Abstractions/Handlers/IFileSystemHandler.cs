// Copyright © WireMock.Net

namespace WireMock.Handlers;

/// <summary>
/// Handler to interact with the file system to handle folders and read and write (static mapping) files.
/// </summary>
public interface IFileSystemHandler
{
    /// <summary>
    /// Gets the folder where the static mappings are located. For local file system, this would be `{CurrentFolder}/__admin/mappings`.
    /// </summary>
    /// <returns>The folder name.</returns>
    string GetMappingFolder();

    /// <summary>
    /// Determines whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified directory exists.</returns>
    bool FolderExists(string path);

    /// <summary>
    ///  Creates all directories and subdirectories in the specified path unless they already exist.
    /// </summary>
    /// <param name="path">The path.</param>
    void CreateFolder(string path);

    /// <summary>
    ///  Returns an enumerable collection of file names in a specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="includeSubdirectories">A value indicating whether subdirectories should also be included when enumerating files.</param>
    /// <returns>An enumerable collection of the full names (including paths) for the files in the directory (and optionally subdirectories) specified by path.</returns>
    IEnumerable<string> EnumerateFiles(string path, bool includeSubdirectories);

    /// <summary>
    /// Read a static mapping file as text.
    /// </summary>
    /// <param name="path">The path (folder + filename with .json extension).</param>
    /// <returns>The file content as text.</returns>
    string ReadMappingFile(string path);

    /// <summary>
    /// Write the static mapping file.
    /// </summary>
    /// <param name="path">The path (folder + filename with .json extension).</param>
    /// <param name="text">The text.</param>
    void WriteMappingFile(string path, string text);

    /// <summary>
    /// Read a response body file as byte[].
    /// </summary>
    /// <param name="path">The path or filename from the file to read.</param>
    /// <returns>The file content as bytes.</returns>
    byte[] ReadResponseBodyAsFile(string path);

    /// <summary>
    /// Read a response body file as text.
    /// </summary>
    /// <param name="path">The path or filename from the file to read.</param>
    /// <returns>The file content as text.</returns>
    string ReadResponseBodyAsString(string path);

    /// <summary>
    /// Delete a file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    void DeleteFile(string filename);

    /// <summary>
    /// Determines whether the given path refers to an existing file on disk.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>true if path refers to an existing file; false if the file does not exist.</returns>
    bool FileExists(string filename);

    /// <summary>
    /// Write a file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="bytes">The bytes.</param>
    void WriteFile(string filename, byte[] bytes);

    /// <summary>
    /// Write a file.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="bytes">The bytes.</param>
    void WriteFile(string folder, string filename, byte[] bytes);

    /// <summary>
    /// Read a file as bytes.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>The file content as bytes.</returns>
    byte[] ReadFile(string filename);

    /// <summary>
    /// Read a file as string.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>The file content as a string.</returns>
    string ReadFileAsString(string filename);

    /// <summary>
    /// Gets the folder where the unmatched requests should be stored. For local file system, this would be `{CurrentFolder}/requests/unmatched`.
    /// </summary>
    /// <returns>The folder name.</returns>
    string GetUnmatchedRequestsFolder();

    /// <summary>
    /// Write an unmatched request to the Unmatched RequestsFolder.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="text">The text.</param>
    void WriteUnmatchedRequest(string filename, string text);
}