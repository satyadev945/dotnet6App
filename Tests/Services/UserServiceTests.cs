using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserService _userService;

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
            _configurationMock = new Mock<IConfiguration>();

            // Setup configuration mock for JWT
            _configurationMock.Setup(c => c["Jwt:Key"])
                .Returns("your-256-bit-secret-key-here-make-it-long-enough");
            _configurationMock.Setup(c => c.GetSection("Jwt:ExpiryInDays"))
                .Returns(new Mock<IConfigurationSection>().Object);

            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(s => s.Value).Returns("7");
            _configurationMock.Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(configSection.Object);

            _userService = new UserService(_context, _mapper, _loggerMock.Object, _configurationMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region GetAllUsersAsync Tests

        [Fact]
        public async Task GetAllUsersAsync_NoUsers_ReturnsEmptyList()
        {
            // Arrange - empty database

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_WithActiveUsers_ReturnsActiveUsers()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "alice", Email = "alice@example.com", PasswordHash = "hash1", IsActive = true },
                new User { Username = "bob", Email = "bob@example.com", PasswordHash = "hash2", IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllUsersAsync_WithInactiveUsers_ExcludesInactiveUsers()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "active", Email = "active@example.com", PasswordHash = "hash1", IsActive = true },
                new User { Username = "inactive", Email = "inactive@example.com", PasswordHash = "hash2", IsActive = false }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("active", result.First().Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersOrderedByUsername()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "zara", Email = "zara@example.com", PasswordHash = "hash1", IsActive = true },
                new User { Username = "alice", Email = "alice@example.com", PasswordHash = "hash2", IsActive = true },
                new User { Username = "mike", Email = "mike@example.com", PasswordHash = "hash3", IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _userService.GetAllUsersAsync()).ToList();

            // Assert
            Assert.Equal("alice", result[0].Username);
            Assert.Equal("mike", result[1].Username);
            Assert.Equal("zara", result[2].Username);
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ReturnsUser()
        {
            // Arrange
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash", IsActive = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange - empty database

            // Act
            var result = await _userService.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_InactiveUser_ReturnsUser()
        {
            // Arrange - FindAsync doesn't filter by IsActive
            var user = new User { Username = "inactive", Email = "inactive@example.com", PasswordHash = "hash", IsActive = false };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region GetUserByUsernameAsync Tests

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingActiveUser_ReturnsUser()
        {
            // Arrange
            _context.Users.Add(new User { Username = "findme", Email = "findme@example.com", PasswordHash = "hash", IsActive = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByUsernameAsync("findme");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("findme", result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistingUsername_ReturnsNull()
        {
            // Arrange - empty database

            // Act
            var result = await _userService.GetUserByUsernameAsync("nobody");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            _context.Users.Add(new User { Username = "inactive", Email = "inactive@example.com", PasswordHash = "hash", IsActive = false });
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByUsernameAsync("inactive");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region CreateUserAsync Tests

        [Fact]
        public async Task CreateUserAsync_ValidDto_ReturnsCreatedUser()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Password123",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _userService.CreateUserAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("new@example.com", result.Email);
            Assert.Equal("New", result.FirstName);
            Assert.Equal("User", result.LastName);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_SetsRoleToUser()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "roletest",
                Email = "role@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _userService.CreateUserAsync(createDto);

            // Assert
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_SetsIsActiveTrue()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "activetest",
                Email = "active@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _userService.CreateUserAsync(createDto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_PersistsToDatabase()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "persisttest",
                Email = "persist@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _userService.CreateUserAsync(createDto);

            // Assert
            var dbUser = await _context.Users.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("persisttest", dbUser.Username);
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_HashesPassword()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "hashtest",
                Email = "hash@example.com",
                Password = "PlainPassword"
            };

            // Act
            var result = await _userService.CreateUserAsync(createDto);

            // Assert
            var dbUser = await _context.Users.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.NotEqual("PlainPassword", dbUser.PasswordHash);
            Assert.NotEmpty(dbUser.PasswordHash);
        }

        #endregion

        #region AuthenticateAsync Tests

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "authuser",
                Email = "auth@example.com",
                Password = "TestPassword123"
            };
            await _userService.CreateUserAsync(createDto);

            var loginRequest = new LoginRequestDto
            {
                Username = "authuser",
                Password = "TestPassword123"
            };

            // Act
            var result = await _userService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.NotNull(result.User);
            Assert.Equal("authuser", result.User.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "authuser2",
                Email = "auth2@example.com",
                Password = "CorrectPassword"
            };
            await _userService.CreateUserAsync(createDto);

            var loginRequest = new LoginRequestDto
            {
                Username = "authuser2",
                Password = "WrongPassword"
            };

            // Act
            var result = await _userService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "anypassword"
            };

            // Act
            var result = await _userService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_TokenIsNotEmpty()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "tokenuser",
                Email = "token@example.com",
                Password = "TokenPass123"
            };
            await _userService.CreateUserAsync(createDto);

            var loginRequest = new LoginRequestDto
            {
                Username = "tokenuser",
                Password = "TokenPass123"
            };

            // Act
            var result = await _userService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.True(result.Token.Length > 0);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_UpdatesLastLoginAt()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "logintime",
                Email = "logintime@example.com",
                Password = "LoginPass123"
            };
            var created = await _userService.CreateUserAsync(createDto);

            var loginRequest = new LoginRequestDto
            {
                Username = "logintime",
                Password = "LoginPass123"
            };

            // Act
            var before = DateTime.UtcNow;
            await _userService.AuthenticateAsync(loginRequest);
            var after = DateTime.UtcNow;

            // Assert
            var dbUser = await _context.Users.FindAsync(created.Id);
            Assert.NotNull(dbUser);
            Assert.NotNull(dbUser.LastLoginAt);
            Assert.True(dbUser.LastLoginAt >= before && dbUser.LastLoginAt <= after);
        }

        [Fact]
        public async Task AuthenticateAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            var user = new User
            {
                Username = "inactiveauth",
                Email = "inactiveauth@example.com",
                PasswordHash = ComputeHash("Password123"),
                IsActive = false
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequestDto
            {
                Username = "inactiveauth",
                Password = "Password123"
            };

            // Act
            var result = await _userService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region DeleteUserAsync Tests

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrueAndSoftDeletes()
        {
            // Arrange
            var user = new User { Username = "todelete", Email = "delete@example.com", PasswordHash = "hash", IsActive = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.DeleteUserAsync(user.Id);

            // Assert
            Assert.True(result);
            var dbUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(dbUser);
            Assert.False(dbUser.IsActive);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange - empty database

            // Act
            var result = await _userService.DeleteUserAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserAsync_DeletedUser_NotReturnedByGetAll()
        {
            // Arrange
            var user = new User { Username = "softdelete", Email = "soft@example.com", PasswordHash = "hash", IsActive = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _userService.DeleteUserAsync(user.Id);
            var allUsers = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Empty(allUsers);
        }

        #endregion

        private static string ComputeHash(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
