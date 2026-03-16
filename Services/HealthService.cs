using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Health check service implementation
    /// </summary>
    public class HealthService : IHealthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HealthService> _logger;

        /// <summary>
        /// Constructor for HealthService
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger instance</param>
        public HealthService(ApplicationDbContext context, ILogger<HealthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Checks the health status of the application
        /// </summary>
        /// <returns>Health status response</returns>
        public async Task<HealthResponseDto> CheckHealthAsync()
        {
            try
            {
                _logger.LogDebug("Performing health check");

                // Check database connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    _logger.LogWarning("Database connection check failed");
                    return new HealthResponseDto { Status = "Unhealthy" };
                }

                _logger.LogDebug("Health check passed");
                return new HealthResponseDto { Status = "Healthy" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed with exception");
                return new HealthResponseDto { Status = "Unhealthy" };
            }
        }
    }
}
