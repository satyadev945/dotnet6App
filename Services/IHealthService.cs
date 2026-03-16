using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for health check service
    /// </summary>
    public interface IHealthService
    {
        /// <summary>
        /// Checks the health status of the application
        /// </summary>
        /// <returns>Health status response</returns>
        Task<HealthResponseDto> CheckHealthAsync();
    }
}
