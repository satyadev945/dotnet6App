using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Models
{
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
            Assert.Equal(string.Empty, user.FirstName);
            Assert.Equal(string.Empty, user.LastName);
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
        public void User_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                FirstName = "Test",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("hashedpassword", user.PasswordHash);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("User", user.LastName);
            Assert.Equal("Admin", user.Role);
            Assert.True(user.IsActive);
            Assert.NotNull(user.LastLoginAt);
        }

        [Fact]
        public void User_IsActive_CanBeSetToFalse()
        {
            // Arrange
            var user = new User();

            // Act
            user.IsActive = false;

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_LastLoginAt_CanBeSetToNull()
        {
            // Arrange
            var user = new User { LastLoginAt = DateTime.UtcNow };

            // Act
            user.LastLoginAt = null;

            // Assert
            Assert.Null(user.LastLoginAt);
        }

        [Fact]
        public void User_Role_DefaultIsUser()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal("User", user.Role);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Manager")]
        [InlineData("User")]
        public void User_Role_CanBeSetToVariousRoles(string role)
        {
            // Arrange
            var user = new User();

            // Act
            user.Role = role;

            // Assert
            Assert.Equal(role, user.Role);
        }

        [Fact]
        public void User_Id_CanBeUpdated()
        {
            // Arrange
            var user = new User();

            // Act
            user.Id = 42;

            // Assert
            Assert.Equal(42, user.Id);
        }
    }
}
