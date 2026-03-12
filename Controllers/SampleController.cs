using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Controller for sample API operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SampleController : ControllerBase
    {
        private readonly ISampleService _sampleService;
        private readonly ILogger<SampleController> _logger;

        public SampleController(ISampleService sampleService, ILogger<SampleController> logger)
        {
            _sampleService = sampleService;
            _logger = logger;
        }

        /// <summary>
        /// Get sample data with current timestamp
        /// </summary>
        /// <returns>Sample response containing message and timestamp</returns>
        /// <response code="200">Returns the sample data</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(SampleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SampleResponse>> GetSample()
        {
            try
            {
                _logger.LogInformation("GET /api/sample endpoint called");

                var result = await _sampleService.GetSampleDataAsync();

                _logger.LogInformation("Sample data retrieved successfully");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving sample data");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while processing your request" });
            }
        }
    }
}
