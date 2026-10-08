using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    public class UserTests
    {
        // ── Constructor / Default Values ──────────────────────────────────────

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
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
        }

        [Fact]
        public void User_CreatedAt_DefaultsToUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var user = new User();
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.True(user.CreatedAt >= before && user.CreatedAt <= after);
        }

        // ── Property Setters ──────────────────────────────────────────────────

        [Fact]
        public void User_SetId_ReturnsCorrectValue()
        {
            var user = new User { Id = 7 };
            Assert.Equal(7, user.Id);
        }

        [Fact]
        public void User_SetUsername_ReturnsCorrectValue()
        {
            var user = new User { Username = "john.doe" };
            Assert.Equal("john.doe", user.Username);
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
        public void User_SetRole_Admin_ReturnsCorrectValue()
        {
            var user = new User { Role = "Admin" };
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void User_SetIsActive_False_ReturnsCorrectValue()
        {
            var user = new User { IsActive = false };
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_SetLastLoginAt_ReturnsCorrectValue()
        {
            var now = DateTime.UtcNow;
            var user = new User { LastLoginAt = now };
            Assert.Equal(now, user.LastLoginAt);
        }

        [Fact]
        public void User_SetRefreshToken_ReturnsCorrectValue()
        {
            var user = new User { RefreshToken = "refresh-token-value" };
            Assert.Equal("refresh-token-value", user.RefreshToken);
        }

        [Fact]
        public void User_SetRefreshTokenExpiry_ReturnsCorrectValue()
        {
            var expiry = DateTime.UtcNow.AddDays(7);
            var user = new User { RefreshTokenExpiry = expiry };
            Assert.Equal(expiry, user.RefreshTokenExpiry);
        }

        // ── Data Annotations Validation ───────────────────────────────────────

        [Fact]
        public void User_Validation_UsernameRequired_FailsWhenEmpty()
        {
            var user = new User
            {
                Username = string.Empty,
                Email = "test@test.com",
                PasswordHash = "hash"
            };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(user);
            Validator.TryValidateObject(user, ctx, results, true);

            Assert.Contains(results, r => r.MemberNames.Contains("Username"));
        }

        [Fact]
        public void User_Validation_EmailRequired_FailsWhenEmpty()
        {
            var user = new User
            {
                Username = "testuser",
                Email = string.Empty,
                PasswordHash = "hash"
            };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(user);
            Validator.TryValidateObject(user, ctx, results, true);

            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void User_Validation_InvalidEmail_FailsValidation()
        {
            var user = new User
            {
                Username = "testuser",
                Email = "not-an-email",
                PasswordHash = "hash"
            };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(user);
            Validator.TryValidateObject(user, ctx, results, true);

            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void User_Validation_ValidUser_PassesValidation()
        {
            var user = new User
            {
                Username = "validuser",
                Email = "valid@example.com",
                PasswordHash = "somehash"
            };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(user);
            var isValid = Validator.TryValidateObject(user, ctx, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void User_DefaultRole_IsUser()
        {
            var user = new User();
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public void User_DefaultIsActive_IsTrue()
        {
            var user = new User();
            Assert.True(user.IsActive);
        }
    }
}
