using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<UsersController>> _loggerMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);
        }

        // ─── Login ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "alice", Password = "Pass123!" };
            var loginResponse = new LoginResponseDto
            {
                Token = "jwt.token",
                User = new UserDto { Id = 1, Username = "alice" },
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            _userServiceMock.Setup(s => s.LoginAsync(loginDto))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("jwt.token", response.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "alice", Password = "WrongPass" };
            _userServiceMock.Setup(s => s.LoginAsync(loginDto))
                .ReturnsAsync((LoginResponseDto?)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        // ─── Register ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Pass123!"
            };
            var userDto = new UserDto { Id = 5, Username = "newuser" };
            _userServiceMock.Setup(s => s.RegisterAsync(registerDto))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            var returnedUser = Assert.IsType<UserDto>(createdResult.Value);
            Assert.Equal("newuser", returnedUser.Username);
        }

        // ─── GetProfile ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetProfile_AuthenticatedUser_ReturnsOkWithProfile()
        {
            // Arrange
            var userDto = new UserDto { Id = 1, Username = "alice" };
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync("alice"))
                .ReturnsAsync(userDto);

            // Set up ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "alice")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("alice", returnedUser.Username);
        }

        [Fact]
        public async Task GetProfile_NoUsernameInClaims_ReturnsUnauthorized()
        {
            // Arrange – empty claims
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
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
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync("ghost"))
                .ReturnsAsync((UserDto?)null);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "ghost") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ─── GetAll ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "alice" },
                new UserDto { Id = 2, Username = "bob" }
            };
            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, ((List<UserDto>)returnedUsers).Count);
        }

        [Fact]
        public async Task GetAll_EmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(new List<UserDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ─── GetById ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkWithUser()
        {
            // Arrange
            var userDto = new UserDto { Id = 1, Username = "alice" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(1))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("alice", returnedUser.Username);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetUserByIdAsync(999))
                .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}
