using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace SampleDotNet6App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }

        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "unknown";

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = version,
                environment = _configuration["ApiSettings:Environment"] ?? "Production",
                swaggerEnabled = _configuration.GetValue<bool>("ApiSettings:EnableSwagger", false)
            });
        }
    }
}
