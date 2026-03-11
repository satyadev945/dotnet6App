using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SampleDotNet6App.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Authentication API controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService">User service</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="logger">Logger</param>
        public AuthController(IUserService userService, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint
        /// </summary>
        /// <param name="request">Login request</param>
        /// <returns>JWT token</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            var user = await _userService.AuthenticateAsync(request.Username, request.Password);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user.Id, user.Username);

            _logger.LogInformation("User {Username} logged in successfully", request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email
            });
        }

        private string GenerateJwtToken(int userId, string username)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-256-bit-secret-key-here-make-it-long-enough");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    /// <summary>
    /// Login request model
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Login response model
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
