using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleDotNet6App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IMapper mapper, IConfiguration configuration, ILogger<UsersController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login and receive a JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.AuthenticateAsync(dto.Username, dto.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password." });

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            token,
            user = _mapper.Map<UserDto>(user),
            expiresAt = DateTime.UtcNow.AddDays(7)
        });
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await _userService.UserExistsAsync(dto.Username, dto.Email))
            return Conflict(new { message = "Username or email already exists." });

        var user = _mapper.Map<User>(dto);
        var created = await _userService.RegisterAsync(user, dto.Password);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<UserDto>(created));
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(_mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found." });

        return Ok(_mapper.Map<UserDto>(user));
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(
            _configuration["Jwt:Key"] ?? "your-256-bit-secret-key-here-make-it-long-enough");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
