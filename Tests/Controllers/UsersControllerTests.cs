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
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Controllers
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

        private static UserDto CreateUserDto(int id = 1, string username = "testuser", string role = "User")
        {
            return new UserDto
            {
                Id = id,
                Username = username,
                Email = $"{username}@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private void SetupUserClaims(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        // ---- Login Tests ----
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Username = "admin", Password = "Admin123!" };
            var loginResponse = new LoginResponseDto
            {
                Token = "jwt.token.here",
                Expiry = DateTime.UtcNow.AddDays(7),
                User = CreateUserDto(1, "admin", "Admin")
            };
            _userServiceMock.Setup(s => s.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("jwt.token.here", response.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Username = "admin", Password = "WrongPass" };
            _userServiceMock.Setup(s => s.LoginAsync(loginRequest))
                .ReturnsAsync((LoginResponseDto?)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        // ---- Register Tests ----
        [Fact]
        public async Task Register_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Pass123!"
            };
            var userDto = CreateUserDto(5, "newuser");
            _userServiceMock.Setup(s => s.RegisterAsync(registerDto))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(5, ((UserDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Register_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Username", "Username is required");
            var registerDto = new RegisterUserDto();

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ---- GetProfile Tests ----
        [Fact]
        public async Task GetProfile_WithAuthenticatedUser_ReturnsOkWithProfile()
        {
            // Arrange
            SetupUserClaims("testuser");
            var userDto = CreateUserDto(1, "testuser");
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync("testuser"))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("testuser", returnedUser.Username);
        }

        [Fact]
        public async Task GetProfile_WithNoUsernameClaim_ReturnsUnauthorized()
        {
            // Arrange - no claims set
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetProfile_WithNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            SetupUserClaims("ghostuser");
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync("ghostuser"))
                .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ---- GetAll Tests ----
        [Fact]
        public async Task GetAll_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<UserDto> { CreateUserDto(1), CreateUserDto(2, "user2") };
            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAll_WithEmptyList_ReturnsOkWithEmptyList()
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

        // ---- GetById Tests ----
        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithUser()
        {
            // Arrange
            var userDto = CreateUserDto(1);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(1))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(1, returnedUser.Id);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetUserByIdAsync(9999))
                .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetById(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ---- Deactivate Tests ----
        [Fact]
        public async Task Deactivate_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeactivateUserAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Deactivate(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Deactivate_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeactivateUserAsync(9999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Deactivate(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
