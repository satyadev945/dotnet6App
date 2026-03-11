using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Users API controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService">User service</param>
        /// <param name="logger">Logger</param>
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        /// <response code="200">Returns the list of users</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("Getting all users");
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        /// <response code="200">Returns the user</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            _logger.LogInformation("Getting user with ID: {UserId}", id);
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>Created user</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Invalid user data</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new user: {Username}", user.Username);
            var createdUser = await _userService.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user data</param>
        /// <returns>Updated user</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid user data</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating user with ID: {UserId}", id);
            var updatedUser = await _userService.UpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                return NotFound();
            }

            return Ok(updatedUser);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content</returns>
        /// <response code="204">User deleted successfully</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
