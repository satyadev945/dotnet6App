using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.Models;
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

        // ── Register ──────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ValidRequest_ReturnsCreatedAtAction()
        {
            var request = new RegisterRequest
            {
                Username = "newuser",
                Email = "new@test.com",
                Password = "password123"
            };

            _userServiceMock.Setup(s => s.GetUserByUsernameAsync("newuser"))
                            .ReturnsAsync((User?)null);

            var createdUser = new User
            {
                Id = 1,
                Username = "newuser",
                Email = "new@test.com"
            };
            _userServiceMock.Setup(s => s.RegisterAsync(It.IsAny<User>(), "password123"))
                            .ReturnsAsync(createdUser);

            var result = await _controller.Register(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetUser), createdResult.ActionName);
        }

        [Fact]
        public async Task Register_UsernameAlreadyExists_ReturnsBadRequest()
        {
            var request = new RegisterRequest
            {
                Username = "existing",
                Email = "existing@test.com",
                Password = "password123"
            };

            _userServiceMock.Setup(s => s.GetUserByUsernameAsync("existing"))
                            .ReturnsAsync(new User { Username = "existing" });

            var result = await _controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Username", "Required");
            var request = new RegisterRequest();

            var result = await _controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ── Login ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var request = new LoginRequest
            {
                Username = "validuser",
                Password = "validpassword"
            };

            var user = new User
            {
                Id = 1,
                Username = "validuser",
                Email = "valid@test.com",
                Role = "User"
            };

            _userServiceMock.Setup(s => s.AuthenticateAsync("validuser", "validpassword"))
                            .ReturnsAsync((user, "jwt-token-here"));

            var result = await _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginRequest
            {
                Username = "baduser",
                Password = "badpassword"
            };

            _userServiceMock.Setup(s => s.AuthenticateAsync("baduser", "badpassword"))
                            .ReturnsAsync(((User?)null, (string?)null));

            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_NullToken_ReturnsUnauthorized()
        {
            var request = new LoginRequest { Username = "user", Password = "pass" };
            var user = new User { Id = 1, Username = "user" };

            _userServiceMock.Setup(s => s.AuthenticateAsync("user", "pass"))
                            .ReturnsAsync((user, (string?)null));

            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // ── GetUsers ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_ReturnsOkWithUserList()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "u1@test.com", Role = "User" },
                new User { Id = 2, Username = "user2", Email = "u2@test.com", Role = "Admin" }
            };

            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                            .ReturnsAsync(users);

            var result = await _controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetUsers_EmptyList_ReturnsOkWithEmptyList()
        {
            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                            .ReturnsAsync(new List<User>());

            var result = await _controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        // ── GetUser ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUser_ExistingId_ReturnsOkWithUser()
        {
            var user = new User
            {
                Id = 1,
                Username = "founduser",
                Email = "found@test.com",
                Role = "User"
            };

            _userServiceMock.Setup(s => s.GetUserByIdAsync(1))
                            .ReturnsAsync(user);

            var result = await _controller.GetUser(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetUser_NonExistentId_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.GetUserByIdAsync(999))
                            .ReturnsAsync((User?)null);

            var result = await _controller.GetUser(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ── UpdateUser ────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUser_ExistingUser_ReturnsOkWithUpdatedUser()
        {
            var request = new UpdateUserRequest
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@test.com"
            };

            var updatedUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "Name"
            };

            _userServiceMock.Setup(s => s.UpdateUserAsync(1, It.IsAny<User>()))
                            .ReturnsAsync(updatedUser);

            var result = await _controller.UpdateUser(1, request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_NonExistentUser_ReturnsNotFound()
        {
            var request = new UpdateUserRequest { FirstName = "X" };

            _userServiceMock.Setup(s => s.UpdateUserAsync(999, It.IsAny<User>()))
                            .ReturnsAsync((User?)null);

            var result = await _controller.UpdateUser(999, request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Email", "Invalid email");
            var request = new UpdateUserRequest { Email = "not-an-email" };

            var result = await _controller.UpdateUser(1, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ── DeleteUser ────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteUser_ExistingUser_ReturnsNoContent()
        {
            _userServiceMock.Setup(s => s.DeleteUserAsync(1))
                            .ReturnsAsync(true);

            var result = await _controller.DeleteUser(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_NonExistentUser_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.DeleteUserAsync(999))
                            .ReturnsAsync(false);

            var result = await _controller.DeleteUser(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ── ChangePassword ────────────────────────────────────────────────────

        [Fact]
        public async Task ChangePassword_ValidRequest_ReturnsNoContent()
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "oldpass",
                NewPassword = "newpass123"
            };

            _userServiceMock.Setup(s => s.ChangePasswordAsync(1, "oldpass", "newpass123"))
                            .ReturnsAsync(true);

            var result = await _controller.ChangePassword(1, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ChangePassword_WrongCurrentPassword_ReturnsBadRequest()
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "wrongpass",
                NewPassword = "newpass123"
            };

            _userServiceMock.Setup(s => s.ChangePasswordAsync(1, "wrongpass", "newpass123"))
                            .ReturnsAsync(false);

            var result = await _controller.ChangePassword(1, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("NewPassword", "Too short");
            var request = new ChangePasswordRequest { CurrentPassword = "old", NewPassword = "new" };

            var result = await _controller.ChangePassword(1, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

    // ── DTO Tests ─────────────────────────────────────────────────────────────

    public class RegisterRequestTests
    {
        [Fact]
        public void RegisterRequest_DefaultValues_AreEmpty()
        {
            var req = new RegisterRequest();
            Assert.Equal(string.Empty, req.Username);
            Assert.Equal(string.Empty, req.Email);
            Assert.Equal(string.Empty, req.Password);
            Assert.Null(req.FirstName);
            Assert.Null(req.LastName);
        }

        [Fact]
        public void RegisterRequest_SetProperties_ReturnCorrectValues()
        {
            var req = new RegisterRequest
            {
                Username = "user",
                Email = "user@test.com",
                Password = "pass123",
                FirstName = "John",
                LastName = "Doe"
            };

            Assert.Equal("user", req.Username);
            Assert.Equal("user@test.com", req.Email);
            Assert.Equal("pass123", req.Password);
            Assert.Equal("John", req.FirstName);
            Assert.Equal("Doe", req.LastName);
        }
    }

    public class LoginRequestTests
    {
        [Fact]
        public void LoginRequest_DefaultValues_AreEmpty()
        {
            var req = new LoginRequest();
            Assert.Equal(string.Empty, req.Username);
            Assert.Equal(string.Empty, req.Password);
        }

        [Fact]
        public void LoginRequest_SetProperties_ReturnCorrectValues()
        {
            var req = new LoginRequest { Username = "user", Password = "pass" };
            Assert.Equal("user", req.Username);
            Assert.Equal("pass", req.Password);
        }
    }

    public class UpdateUserRequestTests
    {
        [Fact]
        public void UpdateUserRequest_DefaultValues_AreNull()
        {
            var req = new UpdateUserRequest();
            Assert.Null(req.FirstName);
            Assert.Null(req.LastName);
            Assert.Null(req.Email);
        }

        [Fact]
        public void UpdateUserRequest_SetProperties_ReturnCorrectValues()
        {
            var req = new UpdateUserRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@test.com"
            };

            Assert.Equal("Jane", req.FirstName);
            Assert.Equal("Smith", req.LastName);
            Assert.Equal("jane@test.com", req.Email);
        }
    }

    public class ChangePasswordRequestTests
    {
        [Fact]
        public void ChangePasswordRequest_DefaultValues_AreEmpty()
        {
            var req = new ChangePasswordRequest();
            Assert.Equal(string.Empty, req.CurrentPassword);
            Assert.Equal(string.Empty, req.NewPassword);
        }

        [Fact]
        public void ChangePasswordRequest_SetProperties_ReturnCorrectValues()
        {
            var req = new ChangePasswordRequest
            {
                CurrentPassword = "oldpass",
                NewPassword = "newpass123"
            };

            Assert.Equal("oldpass", req.CurrentPassword);
            Assert.Equal("newpass123", req.NewPassword);
        }
    }
}
