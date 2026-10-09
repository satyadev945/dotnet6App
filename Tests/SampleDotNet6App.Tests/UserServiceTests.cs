using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Unit tests for UserService.
    /// </summary>
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly IConfiguration _configuration;
        private readonly UserService _service;

        public UserServiceTests()
        {
            // In-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // AutoMapper
            var mapperConfig = new MapperConfiguration(
                cfg => cfg.AddProfile<MappingProfile>(),
                NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            // Logger mock
            _loggerMock = new Mock<ILogger<UserService>>();

            // Use a real in-memory IConfiguration (avoids extension method mocking issues)
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "your-256-bit-secret-key-here-make-it-long-enough",
                    ["Jwt:ExpiryInDays"] = "7"
                })
                .Build();

            _service = new UserService(_context, _mapper, _loggerMock.Object, _configuration);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ─── Helper ────────────────────────────────────────────────────────────

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private User CreateTestUser(string username = "testuser", bool isActive = true)
        {
            var user = new User
            {
                Username = username,
                Email = $"{username}@example.com",
                PasswordHash = HashPassword("Password123"),
                FirstName = "Test",
                LastName = "User",
                Role = "User",
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        // ─── GetAllUsersAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetAllUsersAsync_NoUsers_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_WithActiveUsers_ReturnsActiveUsers()
        {
            // Arrange
            CreateTestUser("alice");
            CreateTestUser("bob");

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllUsersAsync_WithInactiveUsers_ExcludesInactiveUsers()
        {
            // Arrange
            CreateTestUser("active");
            CreateTestUser("inactive", isActive: false);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("active", result.First().Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUserDtos_NotNull()
        {
            // Arrange
            CreateTestUser("charlie");

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.All(result, u => Assert.NotNull(u));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsSortedByUsername()
        {
            // Arrange
            CreateTestUser("zara");
            CreateTestUser("alice");
            CreateTestUser("mike");

            // Act
            var result = (await _service.GetAllUsersAsync()).ToList();

            // Assert
            Assert.Equal("alice", result[0].Username);
            Assert.Equal("mike", result[1].Username);
            Assert.Equal("zara", result[2].Username);
        }

        // ─── GetUserByIdAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ReturnsUserDto()
        {
            // Arrange
            var user = CreateTestUser("findme");

            // Act
            var result = await _service.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("findme", result!.Username);
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
            // Arrange – FindAsync does not filter by IsActive
            var user = CreateTestUser("inactive", isActive: false);

            // Act
            var result = await _service.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
        }

        // ─── GetUserByUsernameAsync ────────────────────────────────────────────

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUsername_ReturnsUserDto()
        {
            // Arrange
            CreateTestUser("lookupuser");

            // Act
            var result = await _service.GetUserByUsernameAsync("lookupuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("lookupuser", result!.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistingUsername_ReturnsNull()
        {
            // Act
            var result = await _service.GetUserByUsernameAsync("nobody");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            CreateTestUser("inactiveuser", isActive: false);

            // Act
            var result = await _service.GetUserByUsernameAsync("inactiveuser");

            // Assert
            Assert.Null(result);
        }

        // ─── CreateUserAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task CreateUserAsync_ValidDto_ReturnsCreatedUserDto()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Password123",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _service.CreateUserAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("new@example.com", result.Email);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_PersistsToDatabase()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "persistuser",
                Email = "persist@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _service.CreateUserAsync(dto);

            // Assert
            var dbUser = await _context.Users.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("persistuser", dbUser!.Username);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_SetsIsActiveTrue()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "activeuser",
                Email = "active@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _service.CreateUserAsync(dto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_SetsRoleToUser()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "roleuser",
                Email = "role@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _service.CreateUserAsync(dto);

            // Assert
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_HashesPassword()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "hashuser",
                Email = "hash@example.com",
                Password = "PlainPassword"
            };

            // Act
            var result = await _service.CreateUserAsync(dto);

            // Assert – password should be stored as hash, not plain text
            var dbUser = await _context.Users.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.NotEqual("PlainPassword", dbUser!.PasswordHash);
        }

        // ─── AuthenticateAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            CreateTestUser("authuser");
            var request = new LoginRequestDto { Username = "authuser", Password = "Password123" };

            // Act
            var result = await _service.AuthenticateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result!.Token);
            Assert.NotNull(result.User);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            CreateTestUser("authuser2");
            var request = new LoginRequestDto { Username = "authuser2", Password = "WrongPassword" };

            // Act
            var result = await _service.AuthenticateAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "ghost", Password = "Password123" };

            // Act
            var result = await _service.AuthenticateAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            CreateTestUser("inactiveauth", isActive: false);
            var request = new LoginRequestDto { Username = "inactiveauth", Password = "Password123" };

            // Act
            var result = await _service.AuthenticateAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_TokenNotEmpty()
        {
            // Arrange
            CreateTestUser("tokenuser");
            var request = new LoginRequestDto { Username = "tokenuser", Password = "Password123" };

            // Act
            var result = await _service.AuthenticateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result!.Token));
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ExpiresAtInFuture()
        {
            // Arrange
            CreateTestUser("expireuser");
            var request = new LoginRequestDto { Username = "expireuser", Password = "Password123" };

            // Act
            var result = await _service.AuthenticateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result!.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_UpdatesLastLoginAt()
        {
            // Arrange
            var user = CreateTestUser("logintrack");
            Assert.Null(user.LastLoginAt);
            var request = new LoginRequestDto { Username = "logintrack", Password = "Password123" };

            // Act
            await _service.AuthenticateAsync(request);

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser!.LastLoginAt);
        }

        // ─── DeleteUserAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var user = CreateTestUser("deleteuser");

            // Act
            var result = await _service.DeleteUserAsync(user.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteUserAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_SetsIsActiveFalse()
        {
            // Arrange
            var user = CreateTestUser("softdelete");

            // Act
            await _service.DeleteUserAsync(user.Id);

            // Assert
            var dbUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(dbUser);
            Assert.False(dbUser!.IsActive);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_DoesNotRemoveFromDatabase()
        {
            // Arrange
            var user = CreateTestUser("keepindb");

            // Act
            await _service.DeleteUserAsync(user.Id);

            // Assert
            var dbUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(dbUser);
        }
    }
}
