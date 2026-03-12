namespace SampleDotNet6App.DTOs
{
    /// <summary>
    /// Response DTO for the sample API endpoint
    /// </summary>
    public class SampleResponse
    {
        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public string Timestamp { get; set; } = string.Empty;
    }
}
