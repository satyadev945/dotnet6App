using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for sample service operations
    /// </summary>
    public interface ISampleService
    {
        /// <summary>
        /// Gets sample data with current timestamp
        /// </summary>
        /// <returns>Sample response with message and timestamp</returns>
        Task<SampleResponse> GetSampleDataAsync();
    }
}
