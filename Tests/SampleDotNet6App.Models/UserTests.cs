using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    public class UserTests
    {
        [Fact]
        public void User_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal(string.Empty, user.Username);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PasswordHash);
            Assert.Equal("User", user.Role);
            Assert.True(user.IsActive);
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
            Assert.True(user.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public void User_SetUsername_ReturnsCorrectUsername()
        {
            // Arrange
            var user = new User();

            // Act
            user.Username = "john.doe";

            // Assert
            Assert.Equal("john.doe", user.Username);
        }

        [Fact]
        public void User_SetEmail_ReturnsCorrectEmail()
        {
            // Arrange
            var user = new User();

            // Act
            user.Email = "john@example.com";

            // Assert
            Assert.Equal("john@example.com", user.Email);
        }

        [Fact]
        public void User_SetPasswordHash_ReturnsCorrectHash()
        {
            // Arrange
            var user = new User();

            // Act
            user.PasswordHash = "hashedpassword123";

            // Assert
            Assert.Equal("hashedpassword123", user.PasswordHash);
        }

        [Fact]
        public void User_SetFirstName_ReturnsCorrectFirstName()
        {
            // Arrange
            var user = new User();

            // Act
            user.FirstName = "John";

            // Assert
            Assert.Equal("John", user.FirstName);
        }

        [Fact]
        public void User_SetLastName_ReturnsCorrectLastName()
        {
            // Arrange
            var user = new User();

            // Act
            user.LastName = "Doe";

            // Assert
            Assert.Equal("Doe", user.LastName);
        }

        [Fact]
        public void User_SetRole_ReturnsCorrectRole()
        {
            // Arrange
            var user = new User();

            // Act
            user.Role = "Admin";

            // Assert
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void User_SetIsActiveFalse_ReturnsInactive()
        {
            // Arrange
            var user = new User();

            // Act
            user.IsActive = false;

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_LastLoginAt_DefaultIsNull()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Null(user.LastLoginAt);
        }

        [Fact]
        public void User_SetLastLoginAt_ReturnsCorrectDate()
        {
            // Arrange
            var user = new User();
            var now = DateTime.UtcNow;

            // Act
            user.LastLoginAt = now;

            // Assert
            Assert.Equal(now, user.LastLoginAt);
        }

        [Fact]
        public void User_SetId_ReturnsCorrectId()
        {
            // Arrange
            var user = new User();

            // Act
            user.Id = 10;

            // Assert
            Assert.Equal(10, user.Id);
        }

        [Fact]
        public void User_FirstName_DefaultIsNull()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Null(user.FirstName);
        }

        [Fact]
        public void User_LastName_DefaultIsNull()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Null(user.LastName);
        }

        [Fact]
        public void User_FullInitialization_AllPropertiesSet()
        {
            // Arrange & Act
            var createdAt = DateTime.UtcNow;
            var user = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "hash123",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreatedAt = createdAt
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("admin", user.Username);
            Assert.Equal("admin@example.com", user.Email);
            Assert.Equal("hash123", user.PasswordHash);
            Assert.Equal("Admin", user.FirstName);
            Assert.Equal("User", user.LastName);
            Assert.Equal("Admin", user.Role);
            Assert.True(user.IsActive);
            Assert.Equal(createdAt, user.CreatedAt);
        }
    }
}
