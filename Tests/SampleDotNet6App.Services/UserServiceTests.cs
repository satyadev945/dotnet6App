using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["Jwt:Key"])
                       .Returns("super-secret-key-that-is-long-enough-for-hmac256");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _configMock.Setup(c => c.GetSection("Jwt:ExpiryInDays"))
                       .Returns(Mock.Of<IConfigurationSection>(s => s.Value == "7"));

            _loggerMock = new Mock<ILogger<UserService>>();
            _service = new UserService(_context, _configMock.Object, _loggerMock.Object);
        }

        public void Dispose() => _context.Dispose();

        // ── Helper ────────────────────────────────────────────────────────────

        private async Task<User> SeedUserAsync(
            string username = "testuser",
            string email = "test@example.com",
            string role = "User",
            bool isActive = true)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = string.Empty,
                Role = role,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ── GetAllUsersAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetAllUsersAsync_ReturnsOnlyActiveUsers()
        {
            await SeedUserAsync("active1", "a1@test.com", isActive: true);
            await SeedUserAsync("inactive1", "i1@test.com", isActive: false);

            var result = (await _service.GetAllUsersAsync()).ToList();

            Assert.Single(result);
            Assert.Equal("active1", result[0].Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersOrderedByUsername()
        {
            await SeedUserAsync("zuser", "z@test.com");
            await SeedUserAsync("auser", "a@test.com");

            var result = (await _service.GetAllUsersAsync()).ToList();

            Assert.Equal("auser", result[0].Username);
            Assert.Equal("zuser", result[1].Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _service.GetAllUsersAsync();
            Assert.Empty(result);
        }

        // ── GetUserByIdAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetUserByIdAsync_ExistingActiveUser_ReturnsUser()
        {
            var seeded = await SeedUserAsync();
            var result = await _service.GetUserByIdAsync(seeded.Id);

            Assert.NotNull(result);
            Assert.Equal(seeded.Id, result!.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_InactiveUser_ReturnsNull()
        {
            var seeded = await SeedUserAsync(isActive: false);
            var result = await _service.GetUserByIdAsync(seeded.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.GetUserByIdAsync(9999);
            Assert.Null(result);
        }

        // ── GetUserByUsernameAsync ────────────────────────────────────────────

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUser_ReturnsUser()
        {
            await SeedUserAsync("findme", "findme@test.com");
            var result = await _service.GetUserByUsernameAsync("findme");

            Assert.NotNull(result);
            Assert.Equal("findme", result!.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistentUsername_ReturnsNull()
        {
            var result = await _service.GetUserByUsernameAsync("nobody");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_InactiveUser_ReturnsNull()
        {
            await SeedUserAsync("inactive", "inactive@test.com", isActive: false);
            var result = await _service.GetUserByUsernameAsync("inactive");
            Assert.Null(result);
        }

        // ── GetUserByEmailAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetUserByEmailAsync_ExistingUser_ReturnsUser()
        {
            await SeedUserAsync("emailuser", "email@test.com");
            var result = await _service.GetUserByEmailAsync("email@test.com");

            Assert.NotNull(result);
            Assert.Equal("email@test.com", result!.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_NonExistentEmail_ReturnsNull()
        {
            var result = await _service.GetUserByEmailAsync("nobody@test.com");
            Assert.Null(result);
        }

        // ── RegisterAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task RegisterAsync_ValidUser_CreatesAndReturnsUser()
        {
            var user = new User
            {
                Username = "newuser",
                Email = "new@test.com",
                PasswordHash = string.Empty
            };

            var result = await _service.RegisterAsync(user, "password123");

            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.NotEmpty(result.PasswordHash);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task RegisterAsync_SetsDefaultRoleToUser_WhenRoleIsEmpty()
        {
            var user = new User
            {
                Username = "norole",
                Email = "norole@test.com",
                PasswordHash = string.Empty,
                Role = string.Empty
            };

            var result = await _service.RegisterAsync(user, "password123");

            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task RegisterAsync_PreservesExistingRole()
        {
            var user = new User
            {
                Username = "adminuser",
                Email = "admin@test.com",
                PasswordHash = string.Empty,
                Role = "Admin"
            };

            var result = await _service.RegisterAsync(user, "password123");

            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task RegisterAsync_HashesPassword()
        {
            var user = new User
            {
                Username = "hashtest",
                Email = "hash@test.com",
                PasswordHash = string.Empty
            };

            var result = await _service.RegisterAsync(user, "plaintext");

            Assert.NotEqual("plaintext", result.PasswordHash);
            Assert.NotEmpty(result.PasswordHash);
        }

        // ── AuthenticateAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsUserAndToken()
        {
            var user = new User
            {
                Username = "authuser",
                Email = "auth@test.com",
                PasswordHash = string.Empty
            };
            await _service.RegisterAsync(user, "correctpassword");

            var (resultUser, token) = await _service.AuthenticateAsync("authuser", "correctpassword");

            Assert.NotNull(resultUser);
            Assert.NotNull(token);
            Assert.NotEmpty(token!);
        }

        [Fact]
        public async Task AuthenticateAsync_WrongPassword_ReturnsNullUserAndToken()
        {
            var user = new User
            {
                Username = "authuser2",
                Email = "auth2@test.com",
                PasswordHash = string.Empty
            };
            await _service.RegisterAsync(user, "correctpassword");

            var (resultUser, token) = await _service.AuthenticateAsync("authuser2", "wrongpassword");

            Assert.Null(resultUser);
            Assert.Null(token);
        }

        [Fact]
        public async Task AuthenticateAsync_NonExistentUser_ReturnsNullUserAndToken()
        {
            var (resultUser, token) = await _service.AuthenticateAsync("ghost", "anypassword");

            Assert.Null(resultUser);
            Assert.Null(token);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_UpdatesLastLoginAt()
        {
            var user = new User
            {
                Username = "logintime",
                Email = "logintime@test.com",
                PasswordHash = string.Empty
            };
            await _service.RegisterAsync(user, "pass123");

            var before = DateTime.UtcNow.AddSeconds(-1);
            var (resultUser, _) = await _service.AuthenticateAsync("logintime", "pass123");
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.NotNull(resultUser);
            Assert.NotNull(resultUser!.LastLoginAt);
            Assert.True(resultUser.LastLoginAt >= before && resultUser.LastLoginAt <= after);
        }

        // ── UpdateUserAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUserAsync_ExistingUser_UpdatesAndReturnsUser()
        {
            var seeded = await SeedUserAsync("updateme", "updateme@test.com");

            var update = new User
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@test.com"
            };

            var result = await _service.UpdateUserAsync(seeded.Id, update);

            Assert.NotNull(result);
            Assert.Equal("Updated", result!.FirstName);
            Assert.Equal("Name", result.LastName);
            Assert.Equal("updated@test.com", result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_NonExistentUser_ReturnsNull()
        {
            var update = new User { FirstName = "X", LastName = "Y", Email = "x@y.com" };
            var result = await _service.UpdateUserAsync(9999, update);
            Assert.Null(result);
        }

        // ── DeleteUserAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrueAndSoftDeletes()
        {
            var seeded = await SeedUserAsync("deleteme", "deleteme@test.com");

            var result = await _service.DeleteUserAsync(seeded.Id);

            Assert.True(result);
            var dbUser = await _context.Users.FindAsync(seeded.Id);
            Assert.NotNull(dbUser);
            Assert.False(dbUser!.IsActive);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistentUser_ReturnsFalse()
        {
            var result = await _service.DeleteUserAsync(9999);
            Assert.False(result);
        }

        // ── ChangePasswordAsync ───────────────────────────────────────────────

        [Fact]
        public async Task ChangePasswordAsync_CorrectCurrentPassword_ReturnsTrueAndChangesHash()
        {
            var user = new User
            {
                Username = "changepw",
                Email = "changepw@test.com",
                PasswordHash = string.Empty
            };
            await _service.RegisterAsync(user, "oldpassword");

            var result = await _service.ChangePasswordAsync(user.Id, "oldpassword", "newpassword");

            Assert.True(result);
            // Verify new password works
            var (authUser, token) = await _service.AuthenticateAsync("changepw", "newpassword");
            Assert.NotNull(authUser);
            Assert.NotNull(token);
        }

        [Fact]
        public async Task ChangePasswordAsync_WrongCurrentPassword_ReturnsFalse()
        {
            var user = new User
            {
                Username = "wrongpw",
                Email = "wrongpw@test.com",
                PasswordHash = string.Empty
            };
            await _service.RegisterAsync(user, "correctpassword");

            var result = await _service.ChangePasswordAsync(user.Id, "wrongpassword", "newpassword");

            Assert.False(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_NonExistentUser_ReturnsFalse()
        {
            var result = await _service.ChangePasswordAsync(9999, "any", "any");
            Assert.False(result);
        }

        // ── GenerateJwtToken ──────────────────────────────────────────────────

        [Fact]
        public void GenerateJwtToken_ValidUser_ReturnsNonEmptyToken()
        {
            var user = new User
            {
                Id = 1,
                Username = "tokenuser",
                Email = "token@test.com",
                Role = "User"
            };

            var token = _service.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void GenerateJwtToken_TokenHasThreeParts()
        {
            var user = new User
            {
                Id = 2,
                Username = "jwtuser",
                Email = "jwt@test.com",
                Role = "Admin"
            };

            var token = _service.GenerateJwtToken(user);
            var parts = token.Split('.');

            Assert.Equal(3, parts.Length);
        }

        [Fact]
        public void GenerateJwtToken_DifferentUsers_ProduceDifferentTokens()
        {
            var user1 = new User { Id = 1, Username = "user1", Email = "u1@test.com", Role = "User" };
            var user2 = new User { Id = 2, Username = "user2", Email = "u2@test.com", Role = "User" };

            var token1 = _service.GenerateJwtToken(user1);
            var token2 = _service.GenerateJwtToken(user2);

            Assert.NotEqual(token1, token2);
        }
    }
}
