using Microsoft.Extensions.Logging;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Implementation of sample service operations
    /// </summary>
    public class SampleService : ISampleService
    {
        private readonly ILogger<SampleService> _logger;

        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets sample data with current timestamp
        /// </summary>
        /// <returns>Sample response with message and timestamp</returns>
        public async Task<SampleResponse> GetSampleDataAsync()
        {
            _logger.LogInformation("GetSampleDataAsync called at {Timestamp}", DateTime.UtcNow);

            var response = new SampleResponse
            {
                Message = "This is a sample API response",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            _logger.LogInformation("Sample data generated successfully");

            return await Task.FromResult(response);
        }
    }
}
