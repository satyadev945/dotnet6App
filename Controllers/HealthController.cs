using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Controller for health check operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Basic health check
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetHealth()
        {
            _logger.LogInformation("Health check endpoint called");
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            });
        }

        /// <summary>
        /// Detailed health check
        /// </summary>
        /// <returns>Detailed health information</returns>
        [HttpGet("detailed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDetailedHealth()
        {
            _logger.LogInformation("Detailed health check endpoint called");
            
            var healthInfo = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                application = new
                {
                    name = "SampleDotNet6App",
                    version = "1.0.0",
                    framework = ".NET 6.0"
                },
                system = new
                {
                    machineName = Environment.MachineName,
                    osVersion = Environment.OSVersion.ToString(),
                    processorCount = Environment.ProcessorCount,
                    workingSet = Environment.WorkingSet,
                    uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString()
                }
            };

            return Ok(healthInfo);
        }
    }
}
