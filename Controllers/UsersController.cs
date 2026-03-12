using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;
using System.Security.Claims;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Controller for user operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);
                var result = await _userService.LoginAsync(loginDto);
                
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// User registration
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequestDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for user: {Username}", registerDto.Username);
                var user = await _userService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed for user: {Username}", registerDto.Username);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                _logger.LogInformation("Getting profile for user: {Username}", username);
                var user = await _userService.GetUserProfileAsync(username);
                
                if (user == null)
                {
                    return NotFound(new { message = "User profile not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving the profile" });
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving users" });
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting user with ID: {UserId}", id);
                var user = await _userService.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving the user" });
            }
        }
    }
}
