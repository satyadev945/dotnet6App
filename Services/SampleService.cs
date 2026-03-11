using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Sample service implementation
    /// </summary>
    public class SampleService : ISampleService
    {
        private readonly ILogger<SampleService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<SampleResponseDto> GetSampleMessageAsync()
        {
            try
            {
                _logger.LogInformation("Generating sample message");

                // Simulate async operation
                await Task.Delay(10);

                var response = new SampleResponseDto
                {
                    Message = "This is a sample message from the API",
                    Timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
                };

                _logger.LogInformation("Sample message generated successfully at {Timestamp}", response.Timestamp);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating sample message");
                throw;
            }
        }
    }
}
