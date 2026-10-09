using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    /// <summary>
    /// Unit tests for the User model.
    /// </summary>
    public class UserTests
    {
        [Fact]
        public void User_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal(0, user.Id);
            Assert.Equal(string.Empty, user.Username);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PasswordHash);
            Assert.Null(user.FirstName);
            Assert.Null(user.LastName);
            Assert.Equal("User", user.Role);
            Assert.True(user.IsActive);
            Assert.Null(user.LastLoginAt);
        }

        [Fact]
        public void User_CreatedAt_DefaultsToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var user = new User();
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(user.CreatedAt >= before && user.CreatedAt <= after);
        }

        [Fact]
        public void User_SetId_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.Id = 42;

            // Assert
            Assert.Equal(42, user.Id);
        }

        [Fact]
        public void User_SetUsername_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.Username = "testuser";

            // Assert
            Assert.Equal("testuser", user.Username);
        }

        [Fact]
        public void User_SetEmail_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.Email = "test@example.com";

            // Assert
            Assert.Equal("test@example.com", user.Email);
        }

        [Fact]
        public void User_SetPasswordHash_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.PasswordHash = "hashedpassword123";

            // Assert
            Assert.Equal("hashedpassword123", user.PasswordHash);
        }

        [Fact]
        public void User_SetFirstName_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.FirstName = "John";

            // Assert
            Assert.Equal("John", user.FirstName);
        }

        [Fact]
        public void User_SetLastName_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.LastName = "Doe";

            // Assert
            Assert.Equal("Doe", user.LastName);
        }

        [Fact]
        public void User_SetRole_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.Role = "Admin";

            // Assert
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void User_SetIsActive_False_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();

            // Act
            user.IsActive = false;

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_SetLastLoginAt_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();
            var loginTime = DateTime.UtcNow;

            // Act
            user.LastLoginAt = loginTime;

            // Assert
            Assert.Equal(loginTime, user.LastLoginAt);
        }

        [Fact]
        public void User_SetCreatedAt_ReturnsCorrectValue()
        {
            // Arrange
            var user = new User();
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            user.CreatedAt = createdAt;

            // Assert
            Assert.Equal(createdAt, user.CreatedAt);
        }

        [Fact]
        public void User_FullyPopulated_AllPropertiesSet()
        {
            // Arrange & Act
            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = 1,
                Username = "johndoe",
                Email = "john@example.com",
                PasswordHash = "abc123hash",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now,
                LastLoginAt = now
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("johndoe", user.Username);
            Assert.Equal("john@example.com", user.Email);
            Assert.Equal("abc123hash", user.PasswordHash);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("Admin", user.Role);
            Assert.True(user.IsActive);
            Assert.Equal(now, user.CreatedAt);
            Assert.Equal(now, user.LastLoginAt);
        }

        [Fact]
        public void User_NullableFields_CanBeNull()
        {
            // Arrange & Act
            var user = new User
            {
                FirstName = null,
                LastName = null,
                LastLoginAt = null
            };

            // Assert
            Assert.Null(user.FirstName);
            Assert.Null(user.LastName);
            Assert.Null(user.LastLoginAt);
        }
    }
}
