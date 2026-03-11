using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Sample service interface
    /// </summary>
    public interface ISampleService
    {
        /// <summary>
        /// Get sample message with timestamp
        /// </summary>
        /// <returns>Sample response with message and timestamp</returns>
        Task<SampleResponseDto> GetSampleMessageAsync();
    }
}
