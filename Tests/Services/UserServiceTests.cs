using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
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
            }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<UserService>>();
            _configurationMock = new Mock<IConfiguration>();

            // Setup configuration mock for JWT
            _configurationMock.Setup(c => c["Jwt:Key"])
                .Returns("your-256-bit-secret-key-here-make-it-long-enough");
            _configurationMock.Setup(c => c["Jwt:Issuer"])
                .Returns("SampleDotNet8App");
            _configurationMock.Setup(c => c["Jwt:Audience"])
                .Returns("SampleDotNet8App");
            _configurationMock.Setup(c => c.GetSection("Jwt:ExpiryInDays"))
                .Returns(Mock.Of<IConfigurationSection>(s => s.Value == "7"));

            _userService = new UserService(_context, _mapper, _loggerMock.Object, _configurationMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private async Task<User> SeedUserAsync(string username = "testuser", string role = "User", bool isActive = true)
        {
            var user = new User
            {
                Username = username,
                Email = $"{username}@example.com",
                PasswordHash = HashPassword("Password123!"),
                FirstName = "Test",
                LastName = "User",
                Role = role,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ---- GetAllUsersAsync Tests ----
        [Fact]
        public async Task GetAllUsersAsync_WithActiveUsers_ReturnsActiveUsers()
        {
            // Arrange
            await SeedUserAsync("user1");
            await SeedUserAsync("user2");
            await SeedUserAsync("inactive", isActive: false);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task GetAllUsersAsync_WithNoUsers_ReturnsEmptyList()
        {
            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersOrderedByUsername()
        {
            // Arrange
            await SeedUserAsync("zuser");
            await SeedUserAsync("auser");
            await SeedUserAsync("muser");

            // Act
            var result = (await _userService.GetAllUsersAsync()).ToList();

            // Assert
            Assert.Equal("auser", result[0].Username);
            Assert.Equal("muser", result[1].Username);
            Assert.Equal("zuser", result[2].Username);
        }

        // ---- GetUserByIdAsync Tests ----
        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            var user = await SeedUserAsync("findme");

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("findme", result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = await _userService.GetUserByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        // ---- GetUserByUsernameAsync Tests ----
        [Fact]
        public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            await SeedUserAsync("activeuser");

            // Act
            var result = await _userService.GetUserByUsernameAsync("activeuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("activeuser", result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithInactiveUser_ReturnsNull()
        {
            // Arrange
            await SeedUserAsync("inactiveuser", isActive: false);

            // Act
            var result = await _userService.GetUserByUsernameAsync("inactiveuser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithNonExistentUsername_ReturnsNull()
        {
            // Act
            var result = await _userService.GetUserByUsernameAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        // ---- LoginAsync Tests ----
        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            await SeedUserAsync("loginuser");
            var loginRequest = new LoginRequestDto
            {
                Username = "loginuser",
                Password = "Password123!"
            };

            // Act
            var result = await _userService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.NotNull(result.User);
            Assert.Equal("loginuser", result.User.Username);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
        {
            // Arrange
            await SeedUserAsync("loginuser2");
            var loginRequest = new LoginRequestDto
            {
                Username = "loginuser2",
                Password = "WrongPassword!"
            };

            // Act
            var result = await _userService.LoginAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "ghost",
                Password = "Password123!"
            };

            // Act
            var result = await _userService.LoginAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ReturnsNull()
        {
            // Arrange
            await SeedUserAsync("inactivelogin", isActive: false);
            var loginRequest = new LoginRequestDto
            {
                Username = "inactivelogin",
                Password = "Password123!"
            };

            // Act
            var result = await _userService.LoginAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_UpdatesLastLoginAt()
        {
            // Arrange
            var user = await SeedUserAsync("logintrack");
            Assert.Null(user.LastLoginAt);

            var loginRequest = new LoginRequestDto
            {
                Username = "logintrack",
                Password = "Password123!"
            };

            // Act
            await _userService.LoginAsync(loginRequest);

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser!.LastLoginAt);
        }

        // ---- RegisterAsync Tests ----
        [Fact]
        public async Task RegisterAsync_WithValidData_CreatesUser()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "NewPass123!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _userService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("newuser@example.com", result.Email);
            Assert.Equal("New", result.FirstName);
            Assert.Equal("User", result.LastName);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_SetsRoleToUser()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "roleuser",
                Email = "roleuser@example.com",
                Password = "Pass123!"
            };

            // Act
            var result = await _userService.RegisterAsync(registerDto);

            // Assert
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_SetsIsActiveTrue()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "activecheck",
                Email = "activecheck@example.com",
                Password = "Pass123!"
            };

            // Act
            var result = await _userService.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.IsActive);
        }

        // ---- UpdateUserAsync Tests ----
        [Fact]
        public async Task UpdateUserAsync_WithValidId_UpdatesUser()
        {
            // Arrange
            var user = await SeedUserAsync("updateme");
            var userDto = new UserDto
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@example.com"
            };

            // Act
            var result = await _userService.UpdateUserAsync(user.Id, userDto);

            // Assert
            Assert.True(result);
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.Equal("Updated", updatedUser!.FirstName);
            Assert.Equal("Name", updatedUser.LastName);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var userDto = new UserDto { FirstName = "Test" };

            // Act
            var result = await _userService.UpdateUserAsync(9999, userDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserAsync_WithEmptyFields_DoesNotOverwrite()
        {
            // Arrange
            var user = await SeedUserAsync("keeporiginal");
            var originalFirstName = user.FirstName;
            var userDto = new UserDto
            {
                FirstName = "",
                LastName = "",
                Email = ""
            };

            // Act
            var result = await _userService.UpdateUserAsync(user.Id, userDto);

            // Assert
            Assert.True(result);
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.Equal(originalFirstName, updatedUser!.FirstName);
        }

        // ---- DeactivateUserAsync Tests ----
        [Fact]
        public async Task DeactivateUserAsync_WithValidId_DeactivatesUser()
        {
            // Arrange
            var user = await SeedUserAsync("deactivateme");
            Assert.True(user.IsActive);

            // Act
            var result = await _userService.DeactivateUserAsync(user.Id);

            // Assert
            Assert.True(result);
            var deactivatedUser = await _context.Users.FindAsync(user.Id);
            Assert.False(deactivatedUser!.IsActive);
        }

        [Fact]
        public async Task DeactivateUserAsync_WithInvalidId_ReturnsFalse()
        {
            // Act
            var result = await _userService.DeactivateUserAsync(9999);

            // Assert
            Assert.False(result);
        }
    }
}
