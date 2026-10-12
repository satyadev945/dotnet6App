using Microsoft.AspNetCore.Mvc;

namespace SampleDotNet6App.Controllers;

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
            version = "10.0",
            framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription
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
            version = "10.0",
            framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            machineName = Environment.MachineName,
            processorCount = Environment.ProcessorCount,
            workingSet = Environment.WorkingSet
        });
    }
}
