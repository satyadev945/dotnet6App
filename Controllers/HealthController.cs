using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Health check controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IHealthService _healthService;

        /// <summary>
        /// Constructor for HealthController
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="healthService">Health service instance</param>
        public HealthController(ILogger<HealthController> logger, IHealthService healthService)
        {
            _logger = logger;
            _healthService = healthService;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Health status</returns>
        /// <response code="200">Returns the health status</response>
        /// <response code="401">Unauthorized access</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(HealthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<HealthResponseDto>> GetHealth()
        {
            try
            {
                _logger.LogInformation("Health check endpoint called");
                
                var healthStatus = await _healthService.CheckHealthAsync();
                
                _logger.LogInformation("Health check completed with status: {Status}", healthStatus.Status);
                
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during health check");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new HealthResponseDto { Status = "Unhealthy" });
            }
        }
    }
}
