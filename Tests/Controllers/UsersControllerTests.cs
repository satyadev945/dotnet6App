using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using System.Security.Claims;

namespace SampleDotNet6App.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly IMapper _mapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<UsersController>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _mockConfiguration.Setup(c => c["Jwt:Key"])
                .Returns("your-256-bit-secret-key-here-make-it-long-enough");

            _controller = new UsersController(
                _mockUserService.Object,
                _mapper,
                _mockConfiguration.Object,
                _mockLogger.Object);
        }

        private static User CreateTestUser(int id = 1, string username = "testuser", string role = "User")
        {
            return new User
            {
                Id = id,
                Username = username,
                Email = $"{username}@example.com",
                PasswordHash = "hashedpassword",
                FirstName = "Test",
                LastName = "User",
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ─── Login ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var user = CreateTestUser();
            var dto = new LoginDto { Username = "testuser", Password = "Password1!" };
            _mockUserService.Setup(s => s.AuthenticateAsync("testuser", "Password1!"))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto { Username = "testuser", Password = "WrongPassword" };
            _mockUserService.Setup(s => s.AuthenticateAsync("testuser", "WrongPassword"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // ─── Register ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_NewUser_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password1!",
                FirstName = "New",
                LastName = "User"
            };
            var createdUser = CreateTestUser(id: 5, username: "newuser");
            _mockUserService.Setup(s => s.UserExistsAsync("newuser", "newuser@example.com"))
                .ReturnsAsync(false);
            _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<User>(), "Password1!"))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        }

        [Fact]
        public async Task Register_ExistingUser_ReturnsConflict()
        {
            // Arrange
            var dto = new RegisterDto { Username = "existing", Email = "existing@example.com", Password = "Password1!" };
            _mockUserService.Setup(s => s.UserExistsAsync("existing", "existing@example.com"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        // ─── GetProfile ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetProfile_AuthenticatedUser_ReturnsOk()
        {
            // Arrange
            var user = CreateTestUser(id: 1);
            _mockUserService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetProfile_NoUserIdClaim_ReturnsUnauthorized()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetProfile_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((User?)null);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ─── GetAll ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<User>
            {
                CreateTestUser(1, "user1"),
                CreateTestUser(2, "user2")
            };
            _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAll_EmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<User>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ─── GetById ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingUser_ReturnsOk()
        {
            // Arrange
            var user = CreateTestUser(id: 1);
            _mockUserService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_NonExistentUser_ReturnsNotFound()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}
