using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    public class UserTests
    {
        // ── Constructor / default values ──────────────────────────────────────

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
            Assert.True(user.IsActive);
            Assert.Null(user.UpdatedAt);
        }

        [Fact]
        public void User_DefaultConstructor_CreatedAtIsUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var user = new User();

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.True(user.CreatedAt >= before && user.CreatedAt <= after);
        }

        // ── Property setters ──────────────────────────────────────────────────

        [Fact]
        public void User_SetId_ReturnsCorrectValue()
        {
            var user = new User { Id = 42 };
            Assert.Equal(42, user.Id);
        }

        [Fact]
        public void User_SetUsername_ReturnsCorrectValue()
        {
            var user = new User { Username = "john_doe" };
            Assert.Equal("john_doe", user.Username);
        }

        [Fact]
        public void User_SetEmail_ReturnsCorrectValue()
        {
            var user = new User { Email = "john@example.com" };
            Assert.Equal("john@example.com", user.Email);
        }

        [Fact]
        public void User_SetPasswordHash_ReturnsCorrectValue()
        {
            var user = new User { PasswordHash = "hashedpassword123" };
            Assert.Equal("hashedpassword123", user.PasswordHash);
        }

        [Fact]
        public void User_SetFirstName_ReturnsCorrectValue()
        {
            var user = new User { FirstName = "John" };
            Assert.Equal("John", user.FirstName);
        }

        [Fact]
        public void User_SetLastName_ReturnsCorrectValue()
        {
            var user = new User { LastName = "Doe" };
            Assert.Equal("Doe", user.LastName);
        }

        [Fact]
        public void User_SetIsActive_False_ReturnsCorrectValue()
        {
            var user = new User { IsActive = false };
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_SetUpdatedAt_ReturnsCorrectValue()
        {
            var now = DateTime.UtcNow;
            var user = new User { UpdatedAt = now };
            Assert.Equal(now, user.UpdatedAt);
        }

        [Fact]
        public void User_SetCreatedAt_ReturnsCorrectValue()
        {
            var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
            var user = new User { CreatedAt = dt };
            Assert.Equal(dt, user.CreatedAt);
        }

        // ── Full object initialisation ────────────────────────────────────────

        [Fact]
        public void User_FullInitialisation_AllPropertiesSet()
        {
            var created = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var updated = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            var user = new User
            {
                Id = 1,
                Username = "alice",
                Email = "alice@example.com",
                PasswordHash = "abc123hash",
                FirstName = "Alice",
                LastName = "Smith",
                CreatedAt = created,
                UpdatedAt = updated,
                IsActive = true
            };

            Assert.Equal(1, user.Id);
            Assert.Equal("alice", user.Username);
            Assert.Equal("alice@example.com", user.Email);
            Assert.Equal("abc123hash", user.PasswordHash);
            Assert.Equal("Alice", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.Equal(created, user.CreatedAt);
            Assert.Equal(updated, user.UpdatedAt);
            Assert.True(user.IsActive);
        }

        [Fact]
        public void User_Username_CanBeEmptyString()
        {
            var user = new User { Username = "" };
            Assert.Equal(string.Empty, user.Username);
        }

        [Fact]
        public void User_Email_CanBeEmptyString()
        {
            var user = new User { Email = "" };
            Assert.Equal(string.Empty, user.Email);
        }

        [Fact]
        public void User_FirstName_CanBeNull()
        {
            var user = new User { FirstName = null };
            Assert.Null(user.FirstName);
        }

        [Fact]
        public void User_LastName_CanBeNull()
        {
            var user = new User { LastName = null };
            Assert.Null(user.LastName);
        }

        [Fact]
        public void User_UpdatedAt_CanBeNull()
        {
            var user = new User { UpdatedAt = null };
            Assert.Null(user.UpdatedAt);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void User_SetId_VariousValues(int id)
        {
            var user = new User { Id = id };
            Assert.Equal(id, user.Id);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void User_IsActive_BothValues(bool isActive)
        {
            var user = new User { IsActive = isActive };
            Assert.Equal(isActive, user.IsActive);
        }
    }
}
