using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleDotNet6App.Controllers;

/// <summary>
/// Health check API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
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
        _logger.LogInformation("Health check requested");
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
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
        _logger.LogInformation("Detailed health check requested");

        var healthInfo = new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            application = new
            {
                name = "SampleDotNet6App",
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            },
            system = new
            {
                osVersion = Environment.OSVersion.ToString(),
                processorCount = Environment.ProcessorCount,
                workingSet = Environment.WorkingSet,
                uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString()
            }
        };

        return Ok(healthInfo);
    }
}
