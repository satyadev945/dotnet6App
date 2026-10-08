using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly IConfiguration _configuration;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<UserService>>();

            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "super-secret-key-for-testing-purposes-must-be-long-enough-256bit" },
                { "Jwt:ExpiryInDays", "7" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new UserService(_context, _mapper, _configuration, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private async Task<User> SeedUserAsync(
            string username = "testuser",
            string email = "test@example.com",
            string password = "Password123!",
            string role = "User",
            bool isActive = true)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ─── GetAllUsersAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllUsersAsync_ReturnsOnlyActiveUsers()
        {
            // Arrange
            await SeedUserAsync("active1", "a1@test.com", isActive: true);
            await SeedUserAsync("active2", "a2@test.com", isActive: true);
            await SeedUserAsync("inactive", "i@test.com", isActive: false);

            // Act
            var result = (await _service.GetAllUsersAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersOrderedByUsername()
        {
            // Arrange
            await SeedUserAsync("zebra", "z@test.com");
            await SeedUserAsync("apple", "a@test.com");
            await SeedUserAsync("mango", "m@test.com");

            // Act
            var result = (await _service.GetAllUsersAsync()).ToList();

            // Assert
            Assert.Equal("apple", result[0].Username);
            Assert.Equal("mango", result[1].Username);
            Assert.Equal("zebra", result[2].Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsMappedUserDtos()
        {
            // Arrange
            await SeedUserAsync("alice", "alice@test.com");

            // Act
            var result = (await _service.GetAllUsersAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.IsType<UserDto>(result[0]);
            Assert.Equal("alice", result[0].Username);
        }

        // ─── GetUserByIdAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ReturnsUserDto()
        {
            // Arrange
            var user = await SeedUserAsync("bob", "bob@test.com");

            // Act
            var result = await _service.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("bob", result!.Username);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _service.GetUserByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_InactiveUser_ReturnsUserDto()
        {
            // Arrange – GetById does NOT filter by IsActive
            var user = await SeedUserAsync("inactive", "i@test.com", isActive: false);

            // Act
            var result = await _service.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result!.IsActive);
        }

        // ─── GetUserByUsernameAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUsername_ReturnsUserDto()
        {
            // Arrange
            await SeedUserAsync("charlie", "charlie@test.com");

            // Act
            var result = await _service.GetUserByUsernameAsync("charlie");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("charlie", result!.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistingUsername_ReturnsNull()
        {
            // Act
            var result = await _service.GetUserByUsernameAsync("nobody");

            // Assert
            Assert.Null(result);
        }

        // ─── RegisterAsync ────────────────────────────────────────────────────────

        [Fact]
        public async Task RegisterAsync_ValidDto_ReturnsUserDto()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "NewPass123!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("new@example.com", result.Email);
            Assert.Equal("New", result.FirstName);
            Assert.Equal("User", result.LastName);
        }

        [Fact]
        public async Task RegisterAsync_SetsRoleToUser()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "roletest",
                Email = "role@example.com",
                Password = "Pass123!"
            };

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task RegisterAsync_SetsIsActiveTrue()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "activetest",
                Email = "active@example.com",
                Password = "Pass123!"
            };

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task RegisterAsync_PersistsToDatabase()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "persisted",
                Email = "persisted@example.com",
                Password = "Pass123!"
            };

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            var inDb = await _context.Users.FindAsync(result.Id);
            Assert.NotNull(inDb);
            Assert.Equal("persisted", inDb!.Username);
        }

        [Fact]
        public async Task RegisterAsync_HashesPassword()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "hashtest",
                Email = "hash@example.com",
                Password = "PlainPassword"
            };

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert – password should NOT be stored in plain text
            var inDb = await _context.Users.FindAsync(result.Id);
            Assert.NotNull(inDb);
            Assert.NotEqual("PlainPassword", inDb!.PasswordHash);
        }

        // ─── LoginAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResponseDto()
        {
            // Arrange
            await SeedUserAsync("loginuser", "login@test.com", "Correct123!");
            var loginDto = new LoginDto { Username = "loginuser", Password = "Correct123!" };

            // Act
            var result = await _service.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result!.Token);
            Assert.Equal("loginuser", result.User.Username);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsNull()
        {
            // Arrange
            await SeedUserAsync("loginuser2", "login2@test.com", "Correct123!");
            var loginDto = new LoginDto { Username = "loginuser2", Password = "WrongPassword" };

            // Act
            var result = await _service.LoginAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "nobody", Password = "Pass123!" };

            // Act
            var result = await _service.LoginAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            await SeedUserAsync("inactivelogin", "il@test.com", "Pass123!", isActive: false);
            var loginDto = new LoginDto { Username = "inactivelogin", Password = "Pass123!" };

            // Act
            var result = await _service.LoginAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_SetsLastLoginAt()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);
            await SeedUserAsync("logintime", "lt@test.com", "Pass123!");
            var loginDto = new LoginDto { Username = "logintime", Password = "Pass123!" };

            // Act
            await _service.LoginAsync(loginDto);

            // Assert
            var inDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "logintime");
            Assert.NotNull(inDb!.LastLoginAt);
            Assert.True(inDb!.LastLoginAt >= before);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsNonExpiredToken()
        {
            // Arrange
            await SeedUserAsync("tokentest", "tt@test.com", "Pass123!");
            var loginDto = new LoginDto { Username = "tokentest", Password = "Pass123!" };

            // Act
            var result = await _service.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result!.ExpiresAt > DateTime.UtcNow);
        }
    }
}
