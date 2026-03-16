namespace SampleDotNet6App.DTOs
{
    /// <summary>
    /// Health check response DTO
    /// </summary>
    public class HealthResponseDto
    {
        /// <summary>
        /// Health status of the application
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
