using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Implementation of user service operations
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<UserService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid login attempt for user: {Username}", loginDto.Username);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Username}", loginDto.Username);
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            _logger.LogInformation("User logged in successfully: {Username}", loginDto.Username);

            return new LoginResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            _logger.LogInformation("Registering new user: {Username}", registerDto.Username);

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);

            return MapToDto(user);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Getting user with ID: {UserId}", id);
            var user = await _context.Users.FindAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users");
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetUserProfileAsync(string username)
        {
            _logger.LogInformation("Getting profile for user: {Username}", username);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user != null ? MapToDto(user) : null;
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? 
                "your-256-bit-secret-key-here-make-it-long-enough");

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

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}
