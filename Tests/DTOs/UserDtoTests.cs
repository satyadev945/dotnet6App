using System;
using Xunit;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Tests.DTOs
{
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new UserDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
            Assert.Equal("User", dto.Role);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UserDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var dto = new UserDto
            {
                Id = 1,
                Username = "johndoe",
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = true,
                CreatedAt = createdAt
            };

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("johndoe", dto.Username);
            Assert.Equal("john@example.com", dto.Email);
            Assert.Equal("John", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal("Admin", dto.Role);
            Assert.True(dto.IsActive);
            Assert.Equal(createdAt, dto.CreatedAt);
        }

        [Fact]
        public void LoginRequestDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new LoginRequestDto();

            // Assert
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Password);
        }

        [Fact]
        public void LoginRequestDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new LoginRequestDto
            {
                Username = "testuser",
                Password = "password123"
            };

            // Assert
            Assert.Equal("testuser", dto.Username);
            Assert.Equal("password123", dto.Password);
        }

        [Fact]
        public void LoginResponseDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new LoginResponseDto();

            // Assert
            Assert.Equal(string.Empty, dto.Token);
            Assert.Null(dto.User);
        }

        [Fact]
        public void LoginResponseDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var expiresAt = DateTime.UtcNow.AddDays(7);
            var userDto = new UserDto { Id = 1, Username = "testuser" };

            // Act
            var dto = new LoginResponseDto
            {
                Token = "jwt.token.here",
                ExpiresAt = expiresAt,
                User = userDto
            };

            // Assert
            Assert.Equal("jwt.token.here", dto.Token);
            Assert.Equal(expiresAt, dto.ExpiresAt);
            Assert.NotNull(dto.User);
            Assert.Equal(1, dto.User.Id);
        }

        [Fact]
        public void CreateUserDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new CreateUserDto();

            // Assert
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.Password);
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        [Fact]
        public void CreateUserDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new CreateUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "SecurePass123",
                FirstName = "New",
                LastName = "User"
            };

            // Assert
            Assert.Equal("newuser", dto.Username);
            Assert.Equal("new@example.com", dto.Email);
            Assert.Equal("SecurePass123", dto.Password);
            Assert.Equal("New", dto.FirstName);
            Assert.Equal("User", dto.LastName);
        }

        [Fact]
        public void UserDto_NullOptionalFields_AreAllowed()
        {
            // Arrange & Act
            var dto = new UserDto
            {
                Id = 2,
                Username = "noname",
                Email = "noname@example.com",
                FirstName = null,
                LastName = null
            };

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        [Fact]
        public void LoginResponseDto_NullUser_IsAllowed()
        {
            // Arrange & Act
            var dto = new LoginResponseDto
            {
                Token = "some.token",
                User = null
            };

            // Assert
            Assert.Null(dto.User);
        }
    }
}
