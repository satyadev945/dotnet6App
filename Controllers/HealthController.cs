using Microsoft.AspNetCore.Mvc;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Controller for health check endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns the health status of the API.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetHealth()
        {
            _logger.LogInformation("Health check requested at {Time}", DateTime.UtcNow);

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }

        /// <summary>
        /// Returns detailed health information.
        /// </summary>
        [HttpGet("detailed")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetDetailedHealth()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                checks = new[]
                {
                    new { name = "API", status = "Healthy" },
                    new { name = "Database", status = "Healthy" }
                }
            });
        }
    }
}
