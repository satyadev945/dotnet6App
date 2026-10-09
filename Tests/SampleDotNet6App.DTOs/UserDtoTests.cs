using System;
using Xunit;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.DTOs.Tests
{
    /// <summary>
    /// Unit tests for UserDto and related DTOs.
    /// </summary>
    public class UserDtoTests
    {
        // ─── UserDto ───────────────────────────────────────────────────────────

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
            var now = DateTime.UtcNow;

            // Act
            var dto = new UserDto
            {
                Id = 1,
                Username = "alice",
                Email = "alice@example.com",
                FirstName = "Alice",
                LastName = "Smith",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now
            };

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("alice", dto.Username);
            Assert.Equal("alice@example.com", dto.Email);
            Assert.Equal("Alice", dto.FirstName);
            Assert.Equal("Smith", dto.LastName);
            Assert.Equal("Admin", dto.Role);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
        }

        [Fact]
        public void UserDto_NullableFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new UserDto { FirstName = null, LastName = null };

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        // ─── LoginRequestDto ───────────────────────────────────────────────────

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
                Username = "bob",
                Password = "secret123"
            };

            // Assert
            Assert.Equal("bob", dto.Username);
            Assert.Equal("secret123", dto.Password);
        }

        // ─── LoginResponseDto ──────────────────────────────────────────────────

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
            var now = DateTime.UtcNow;
            var userDto = new UserDto { Id = 1, Username = "alice" };

            // Act
            var dto = new LoginResponseDto
            {
                Token = "jwt.token.here",
                ExpiresAt = now,
                User = userDto
            };

            // Assert
            Assert.Equal("jwt.token.here", dto.Token);
            Assert.Equal(now, dto.ExpiresAt);
            Assert.NotNull(dto.User);
            Assert.Equal(1, dto.User!.Id);
        }

        [Fact]
        public void LoginResponseDto_UserCanBeNull()
        {
            // Arrange & Act
            var dto = new LoginResponseDto { User = null };

            // Assert
            Assert.Null(dto.User);
        }

        // ─── CreateUserDto ─────────────────────────────────────────────────────

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
                Password = "P@ssw0rd",
                FirstName = "New",
                LastName = "User"
            };

            // Assert
            Assert.Equal("newuser", dto.Username);
            Assert.Equal("new@example.com", dto.Email);
            Assert.Equal("P@ssw0rd", dto.Password);
            Assert.Equal("New", dto.FirstName);
            Assert.Equal("User", dto.LastName);
        }

        [Fact]
        public void CreateUserDto_NullableFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new CreateUserDto { FirstName = null, LastName = null };

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }
    }
}
