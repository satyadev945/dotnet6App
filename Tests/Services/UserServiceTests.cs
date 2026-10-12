using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _userService = new UserService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task SeedUsersAsync()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "alice", Email = "alice@example.com", PasswordHash = HashPassword("Password1!"), FirstName = "Alice", LastName = "Smith", Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow },
                new User { Id = 2, Username = "bob", Email = "bob@example.com", PasswordHash = HashPassword("Password2!"), FirstName = "Bob", LastName = "Jones", Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow },
                new User { Id = 3, Username = "charlie", Email = "charlie@example.com", PasswordHash = HashPassword("Password3!"), FirstName = "Charlie", LastName = "Brown", Role = "User", IsActive = false, CreatedAt = DateTime.UtcNow }
            };
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // ─── GetAllAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveUsers()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsUsersOrderedByUsername()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = (await _userService.GetAllAsync()).ToList();

            // Assert
            Assert.Equal("alice", result[0].Username);
            Assert.Equal("bob", result[1].Username);
        }

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        // ─── GetByIdAsync ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingActiveUser_ReturnsUser()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("alice", result.Username);
        }

        [Fact]
        public async Task GetByIdAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetByIdAsync(3);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // ─── GetByUsernameAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task GetByUsernameAsync_ExistingActiveUser_ReturnsUser()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetByUsernameAsync("alice");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("alice", result.Username);
        }

        [Fact]
        public async Task GetByUsernameAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetByUsernameAsync("charlie");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsernameAsync_NonExistentUsername_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.GetByUsernameAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        // ─── AuthenticateAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.AuthenticateAsync("alice", "Password1!");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("alice", result.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_UpdatesLastLoginAt()
        {
            // Arrange
            await SeedUsersAsync();
            var before = DateTime.UtcNow;

            // Act
            var result = await _userService.AuthenticateAsync("alice", "Password1!");
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.LastLoginAt);
            Assert.True(result.LastLoginAt >= before && result.LastLoginAt <= after);
        }

        [Fact]
        public async Task AuthenticateAsync_WrongPassword_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.AuthenticateAsync("alice", "WrongPassword!");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_NonExistentUser_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.AuthenticateAsync("nonexistent", "Password1!");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.AuthenticateAsync("charlie", "Password3!");

            // Assert
            Assert.Null(result);
        }

        // ─── RegisterAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsCreatedUser()
        {
            // Arrange
            var user = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _userService.RegisterAsync(user, "NewPassword1!");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.True(result.IsActive);
            Assert.NotEmpty(result.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_HashesPassword()
        {
            // Arrange
            var user = new User { Username = "hashtest", Email = "hashtest@example.com" };
            const string password = "TestPassword123!";

            // Act
            var result = await _userService.RegisterAsync(user, password);

            // Assert
            Assert.NotEqual(password, result.PasswordHash);
            Assert.NotEmpty(result.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_SetsCreatedAt()
        {
            // Arrange
            var user = new User { Username = "timetest", Email = "timetest@example.com" };
            var before = DateTime.UtcNow;

            // Act
            var result = await _userService.RegisterAsync(user, "Password1!");
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_SetsIsActiveTrue()
        {
            // Arrange
            var user = new User { Username = "activetest", Email = "activetest@example.com", IsActive = false };

            // Act
            var result = await _userService.RegisterAsync(user, "Password1!");

            // Assert
            Assert.True(result.IsActive);
        }

        // ─── UserExistsAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task UserExistsAsync_ExistingUsername_ReturnsTrue()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.UserExistsAsync("alice", "different@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_ExistingEmail_ReturnsTrue()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.UserExistsAsync("differentuser", "alice@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_NonExistentUsernameAndEmail_ReturnsFalse()
        {
            // Arrange
            await SeedUsersAsync();

            // Act
            var result = await _userService.UserExistsAsync("newuser", "newuser@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UserExistsAsync_EmptyDatabase_ReturnsFalse()
        {
            // Act
            var result = await _userService.UserExistsAsync("anyuser", "any@example.com");

            // Assert
            Assert.False(result);
        }
    }
}
