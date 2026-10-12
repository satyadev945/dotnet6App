using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Service implementation for user operations and authentication.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<UserService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Username)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username && u.IsActive);

            if (user == null)
                return null;

            var passwordHash = HashPassword(loginRequest.Password);
            if (user.PasswordHash != passwordHash)
                return null;

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var expiry = DateTime.UtcNow.AddDays(
                _configuration.GetValue<int>("Jwt:ExpiryInDays", 7));

            _logger.LogInformation("User {Username} logged in successfully", user.Username);

            return new LoginResponseDto
            {
                Token = token,
                Expiry = expiry,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto registerDto)
        {
            var user = new Models.User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Registered new user {Username}", user.Username);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateUserAsync(int id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            if (!string.IsNullOrEmpty(userDto.FirstName)) user.FirstName = userDto.FirstName;
            if (!string.IsNullOrEmpty(userDto.LastName)) user.LastName = userDto.LastName;
            if (!string.IsNullOrEmpty(userDto.Email)) user.Email = userDto.Email;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated user with ID {UserId}", id);
            return true;
        }

        private string GenerateJwtToken(Models.User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "your-256-bit-secret-key-here-make-it-long-enough";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "SampleDotNet8App";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "SampleDotNet8App";
            var expiryDays = _configuration.GetValue<int>("Jwt:ExpiryInDays", 7);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
