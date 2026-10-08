using Microsoft.AspNetCore.Mvc;

namespace SampleDotNet6App.Controllers
{
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
        /// Basic health check
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Detailed health information
        /// </summary>
        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                machineName = Environment.MachineName,
                processorCount = Environment.ProcessorCount,
                uptime = Environment.TickCount64
            });
        }
    }
}
