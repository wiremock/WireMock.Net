// Copyright © WireMock.Net

namespace WireMock.Models
{
    /// <summary>
    /// A simplified interface exposing the public, readable properties of MimePart.
    /// </summary>
    public interface IMimePartData : IMimeEntityData
    {
        /// <summary>
        /// Get the description of the content if available.
        /// </summary>
        /// <value>The description of the content.</value>
        string ContentDescription { get; }

        /// <summary>
        /// Get the duration of the content if available.
        /// </summary>
        /// <value>The duration of the content.</value>
        int? ContentDuration { get; }

        /// <summary>
        /// Get the md5sum of the content.
        /// </summary>
        /// <value>The md5sum of the content.</value>
        string ContentMd5 { get; }

        /// <summary>
        /// Get the content transfer encoding.
        /// </summary>
        /// <value>The content transfer encoding as an integer.</value>
        int ContentTransferEncoding { get; }

        /// <summary>
        /// Get the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; }

        /// <summary>
        /// Get the MIME content.
        /// </summary>
        /// <value>The MIME content.</value>
        object Content { get; }
    }
}
