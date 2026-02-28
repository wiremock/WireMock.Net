// Copyright © WireMock.Net

namespace WireMock.Types
{
    /// <summary>
    /// Policies to use when using CORS.
    /// </summary>
    [Flags]
    public enum CorsPolicyOptions
    {
        /// <summary>
        /// Cors is disabled
        /// </summary>
        None = 0,

        /// <summary>
        /// Ensures that the policy allows any header.
        /// </summary>
        AllowAnyHeader = 0b00000001,

        /// <summary>
        /// Ensures that the policy allows any method.
        /// </summary>
        AllowAnyMethod = 0b00000010,

        /// <summary>
        /// Ensures that the policy allows any origin.
        /// </summary>
        AllowAnyOrigin = 0b00000100,

        /// <summary>
        /// Ensures that the policy allows any header, method and origin.
        /// </summary>
        AllowAll = AllowAnyHeader | AllowAnyMethod | AllowAnyOrigin
    }
}