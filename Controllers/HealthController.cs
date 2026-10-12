using Microsoft.AspNetCore.Mvc;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Controller for health check endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Framework = ".NET 8"
            });
        }

        /// <summary>
        /// Detailed health check endpoint.
        /// </summary>
        [HttpGet("detailed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDetailed()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Framework = ".NET 8",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = Environment.WorkingSet,
                Uptime = Environment.TickCount64
            });
        }
    }
}
