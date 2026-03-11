using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;
using System.ComponentModel.DataAnnotations;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Sample API controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SampleController : ControllerBase
    {
        private readonly ISampleService _sampleService;
        private readonly ILogger<SampleController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sampleService">Sample service</param>
        /// <param name="logger">Logger</param>
        public SampleController(ISampleService sampleService, ILogger<SampleController> logger)
        {
            _sampleService = sampleService ?? throw new ArgumentNullException(nameof(sampleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get sample message with timestamp
        /// </summary>
        /// <returns>Sample message and timestamp</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/sample
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "This is a sample message from the API",
        ///         "timestamp": "2024-01-15T10:30:00.000Z"
        ///     }
        /// </remarks>
        /// <response code="200">Returns sample message and timestamp</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(SampleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SampleResponseDto>> GetSample()
        {
            try
            {
                _logger.LogInformation("Sample endpoint called at {RequestTime}", DateTime.UtcNow);

                var response = await _sampleService.GetSampleMessageAsync();

                if (response == null)
                {
                    _logger.LogError("Sample service returned null response");
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        new { message = "An error occurred while processing your request" });
                }

                _logger.LogInformation("Sample endpoint completed successfully");
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null argument error in Sample endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Invalid request parameters" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in Sample endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while processing your request" });
            }
        }
    }
}
