using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SampleDotNet6App.DTOs
{
    /// <summary>
    /// Sample response DTO
    /// </summary>
    public class SampleResponseDto
    {
        /// <summary>
        /// Sample message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp in ISO 8601 format
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }
}
