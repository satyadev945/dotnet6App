using System;
using System.Collections.Generic;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal(string.Empty, user.Username);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PasswordHash);
            Assert.Equal("User", user.Role);
            Assert.True(user.IsActive);
            Assert.Null(user.FirstName);
            Assert.Null(user.LastName);
            Assert.Null(user.LastLoginAt);
        }

        [Fact]
        public void User_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var now = DateTime.UtcNow;
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
                CreatedAt = now,
                LastLoginAt = now
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
            Assert.Equal(now, user.CreatedAt);
            Assert.Equal(now, user.LastLoginAt);
        }

        [Fact]
        public void User_IsActive_CanBeSetToFalse()
        {
            // Arrange
            var user = new User { IsActive = true };

            // Act
            user.IsActive = false;

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_LastLoginAt_IsNullableDateTime()
        {
            // Arrange
            var user = new User();

            // Assert
            Assert.Null(user.LastLoginAt);

            // Act
            user.LastLoginAt = DateTime.UtcNow;

            // Assert
            Assert.NotNull(user.LastLoginAt);
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
    }
}
