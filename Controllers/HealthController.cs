using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.Data;

namespace SampleDotNet6App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
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
        /// Detailed health check
        /// </summary>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            var dbHealthy = false;
            string dbMessage = "Unknown";

            try
            {
                dbHealthy = await _context.Database.CanConnectAsync();
                dbMessage = dbHealthy ? "Connected" : "Cannot connect";
            }
            catch (Exception ex)
            {
                dbMessage = $"Error: {ex.Message}";
                _logger.LogError(ex, "Database health check failed");
            }

            var result = new
            {
                status = dbHealthy ? "Healthy" : "Degraded",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                database = new
                {
                    status = dbHealthy ? "Healthy" : "Unhealthy",
                    message = dbMessage
                }
            };

            return dbHealthy ? Ok(result) : StatusCode(503, result);
        }
    }
}
