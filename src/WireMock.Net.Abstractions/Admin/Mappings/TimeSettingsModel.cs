// Copyright © WireMock.Net

namespace WireMock.Models
{
    /// <summary>
    /// TimeSettingsModel: Start, End and TTL
    /// </summary>
    [FluentBuilder.AutoGenerateBuilder]
    public class TimeSettingsModel
    {
        /// <summary>
        /// Gets or sets the DateTime from which this mapping should be used. In case this is not defined, it's used (default behavior).
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Gets or sets the DateTime from until this mapping should be used. In case this is not defined, it's used forever (default behavior).
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Gets or sets the TTL (Time To Live) in seconds for this mapping. In case this is not defined, it's used (default behavior).
        /// </summary>
        public int? TTL { get; set; }
    }
}