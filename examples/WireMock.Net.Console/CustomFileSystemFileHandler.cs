// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
using WireMock.Handlers;

namespace WireMock.Net.ConsoleApplication
{
    internal class CustomFileSystemFileHandler : IFileSystemHandler
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");
        private static readonly string UnmatchedRequestsFolder = Path.Combine("requests", "unmatched");

        /// <inheritdoc cref="IFileSystemHandler.FolderExists"/>
        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.CreateFolder"/>
        public void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.EnumerateFiles"/>
        public IEnumerable<string> EnumerateFiles(string path, bool includeSubdirectories)
        {
            return includeSubdirectories ? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories) : Directory.EnumerateFiles(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.GetMappingFolder"/>
        public string GetMappingFolder()
        {
            return Path.Combine(@"c:\temp-wiremock", AdminMappingsFolder);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadMappingFile"/>
        public string ReadMappingFile(string path)
        {
            return File.ReadAllText(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteMappingFile"/>
        public void WriteMappingFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsFile"/>
        public byte[] ReadResponseBodyAsFile(string path)
        {
            return File.ReadAllBytes(Path.GetFileName(path) == path ? Path.Combine(GetMappingFolder(), path) : path);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsFile"/>
        public string ReadResponseBodyAsString(string path)
        {
            return File.ReadAllText(Path.GetFileName(path) == path ? Path.Combine(GetMappingFolder(), path) : path);
        }

        /// <inheritdoc cref="IFileSystemHandler.FileExists"/>
        public bool FileExists(string path)
        {
            return File.Exists(AdjustPath(path));
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteFile(string, byte[])"/>
        public void WriteFile(string path, byte[] bytes)
        {
            File.WriteAllBytes(AdjustPath(path), bytes);
        }

        public void WriteFile(string folder, string filename, byte[] bytes)
        {
            File.WriteAllBytes(Path.Combine(folder, filename), bytes);

        }

        /// <inheritdoc cref="IFileSystemHandler.DeleteFile"/>
        public void DeleteFile(string path)
        {
            File.Delete(AdjustPath(path));
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadFile"/>
        public byte[] ReadFile(string path)
        {
            return File.ReadAllBytes(AdjustPath(path));
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadFileAsString"/>
        public string ReadFileAsString(string path)
        {
            return File.ReadAllText(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.GetUnmatchedRequestsFolder"/>
        public string GetUnmatchedRequestsFolder()
        {
            return Path.Combine(@"c:\temp-wiremock", UnmatchedRequestsFolder);
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteUnmatchedRequest"/>
        public void WriteUnmatchedRequest(string filename, string text)
        {
            var folder = GetUnmatchedRequestsFolder();
            Directory.CreateDirectory(folder);

            File.WriteAllText(Path.Combine(folder, filename), text);
        }

        /// <summary>
        /// Adjusts the path to the MappingFolder.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Adjusted path</returns>
        private string AdjustPath(string path)
        {
            return Path.Combine(GetMappingFolder(), path);
        }
    }
}