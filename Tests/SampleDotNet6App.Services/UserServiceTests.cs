using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Use real ConfigurationBuilder instead of Moq (avoids extension-method mock limitation)
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "super-secret-key-for-testing-purposes-32chars!!",
                    ["Jwt:ExpiryInDays"] = "7"
                })
                .Build();

            _service = new UserService(_context, _configuration);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ── Helper ────────────────────────────────────────────────────────────

        private async Task<User> SeedUserAsync(
            string username = "testuser",
            string email = "test@example.com",
            string password = "Password123!")
        {
            return await _service.CreateAsync(
                new User { Username = username, Email = email, IsActive = true },
                password);
        }

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithUsers_ReturnsAllUsers()
        {
            // Arrange
            await SeedUserAsync("user1", "user1@example.com");
            await SeedUserAsync("user2", "user2@example.com");

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        // ── GetByIdAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsUser()
        {
            // Arrange
            var created = await SeedUserAsync();

            // Act
            var result = await _service.GetByIdAsync(created.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result!.Id);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ZeroId_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            Assert.Null(result);
        }

        // ── GetByEmailAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task GetByEmailAsync_ExistingEmail_ReturnsUser()
        {
            // Arrange
            await SeedUserAsync(email: "find@example.com");

            // Act
            var result = await _service.GetByEmailAsync("find@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("find@example.com", result!.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
        {
            // Act
            var result = await _service.GetByEmailAsync("nobody@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_EmptyEmail_ReturnsNull()
        {
            // Act
            var result = await _service.GetByEmailAsync(string.Empty);

            // Assert
            Assert.Null(result);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
        {
            // Arrange
            var user = new User { Username = "newuser", Email = "new@example.com", IsActive = true };

            // Act
            var result = await _service.CreateAsync(user, "SecurePass1!");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("new@example.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_HashesPassword()
        {
            // Arrange
            var user = new User { Username = "hashtest", Email = "hash@example.com" };
            const string plainPassword = "MyPassword!";

            // Act
            var result = await _service.CreateAsync(user, plainPassword);

            // Assert
            Assert.NotEqual(plainPassword, result.PasswordHash);
            Assert.NotEmpty(result.PasswordHash);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_SetsCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);
            var user = new User { Username = "timetest", Email = "time@example.com" };

            // Act
            var result = await _service.CreateAsync(user, "Pass123!");

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_PersistsToDatabase()
        {
            // Arrange
            var user = new User { Username = "persist", Email = "persist@example.com" };

            // Act
            var result = await _service.CreateAsync(user, "Pass123!");
            var fromDb = await _context.Users.FindAsync(result.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("persist", fromDb!.Username);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_ExistingUser_ReturnsUpdatedUser()
        {
            // Arrange
            var created = await SeedUserAsync();
            var update = new User
            {
                Username = "updated_user",
                Email = "updated@example.com",
                FirstName = "Updated",
                LastName = "Name",
                IsActive = true
            };

            // Act
            var result = await _service.UpdateAsync(created.Id, update);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated_user", result!.Username);
            Assert.Equal("updated@example.com", result.Email);
            Assert.Equal("Updated", result.FirstName);
            Assert.Equal("Name", result.LastName);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var update = new User { Username = "ghost", Email = "ghost@example.com" };

            // Act
            var result = await _service.UpdateAsync(9999, update);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_SetsUpdatedAt()
        {
            // Arrange
            var created = await SeedUserAsync();
            var before = DateTime.UtcNow.AddSeconds(-1);
            var update = new User { Username = "u", Email = "u@example.com", IsActive = true };

            // Act
            var result = await _service.UpdateAsync(created.Id, update);

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.NotNull(result!.UpdatedAt);
            Assert.True(result.UpdatedAt >= before && result.UpdatedAt <= after);
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_PersistsChanges()
        {
            // Arrange
            var created = await SeedUserAsync();
            var update = new User { Username = "persisted_update", Email = "pu@example.com", IsActive = false };

            // Act
            await _service.UpdateAsync(created.Id, update);
            var fromDb = await _context.Users.FindAsync(created.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("persisted_update", fromDb!.Username);
            Assert.False(fromDb.IsActive);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var created = await SeedUserAsync();

            // Act
            var result = await _service.DeleteAsync(created.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingUser_RemovesFromDatabase()
        {
            // Arrange
            var created = await SeedUserAsync();

            // Act
            await _service.DeleteAsync(created.Id);
            var fromDb = await _context.Users.FindAsync(created.Id);

            // Assert
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ZeroId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            Assert.False(result);
        }

        // ── AuthenticateAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            await SeedUserAsync(email: "auth@example.com", password: "CorrectPass1!");

            // Act
            var token = await _service.AuthenticateAsync("auth@example.com", "CorrectPass1!");

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token!);
        }

        [Fact]
        public async Task AuthenticateAsync_WrongPassword_ReturnsNull()
        {
            // Arrange
            await SeedUserAsync(email: "wrongpass@example.com", password: "CorrectPass1!");

            // Act
            var token = await _service.AuthenticateAsync("wrongpass@example.com", "WrongPass!");

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task AuthenticateAsync_NonExistingEmail_ReturnsNull()
        {
            // Act
            var token = await _service.AuthenticateAsync("nobody@example.com", "AnyPass1!");

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task AuthenticateAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            var user = new User { Username = "inactive", Email = "inactive@example.com", IsActive = false };
            await _service.CreateAsync(user, "Pass123!");

            // Act
            var token = await _service.AuthenticateAsync("inactive@example.com", "Pass123!");

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsJwtFormat()
        {
            // Arrange
            await SeedUserAsync(email: "jwt@example.com", password: "JwtPass1!");

            // Act
            var token = await _service.AuthenticateAsync("jwt@example.com", "JwtPass1!");

            // Assert
            Assert.NotNull(token);
            // JWT tokens have 3 parts separated by dots
            var parts = token!.Split('.');
            Assert.Equal(3, parts.Length);
        }

        [Fact]
        public async Task AuthenticateAsync_EmptyEmail_ReturnsNull()
        {
            // Act
            var token = await _service.AuthenticateAsync(string.Empty, "Pass123!");

            // Assert
            Assert.Null(token);
        }
    }
}
