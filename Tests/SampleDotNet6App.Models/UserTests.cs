using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.Password);
        Assert.Null(user.FullName);
        Assert.NotEqual(default(DateTime), user.CreatedDate);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Id_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedId = 456;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Fact]
    public void Username_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedUsername = "testuser";

        // Act
        user.Username = expectedUsername;

        // Assert
        Assert.Equal(expectedUsername, user.Username);
    }

    [Fact]
    public void Username_SetEmpty_ReturnsEmpty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Username);
    }

    [Fact]
    public void Email_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedEmail = "test@example.com";

        // Act
        user.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public void Email_SetEmpty_ReturnsEmpty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void Password_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedPassword = "securePassword123";

        // Act
        user.Password = expectedPassword;

        // Assert
        Assert.Equal(expectedPassword, user.Password);
    }

    [Fact]
    public void Password_SetEmpty_ReturnsEmpty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Password = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Password);
    }

    [Fact]
    public void FullName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedFullName = "John Doe";

        // Act
        user.FullName = expectedFullName;

        // Assert
        Assert.Equal(expectedFullName, user.FullName);
    }

    [Fact]
    public void FullName_SetNull_ReturnsNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.FullName = null;

        // Assert
        Assert.Null(user.FullName);
    }

    [Fact]
    public void CreatedDate_InitializedToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var user = new User();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(user.CreatedDate >= beforeCreation);
        Assert.True(user.CreatedDate <= afterCreation);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(2024, 1, 1);

        // Act
        user.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.CreatedDate);
    }

    [Fact]
    public void IsActive_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void IsActive_SetFalse_ReturnsFalse()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void IsActive_SetTrue_ReturnsTrue()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void AllProperties_SetAndGet_ReturnsCorrectValues()
    {
        // Arrange
        var user = new User();
        var id = 100;
        var username = "admin";
        var email = "admin@example.com";
        var password = "hashedPassword";
        var fullName = "Administrator";
        var createdDate = DateTime.UtcNow;
        var isActive = false;

        // Act
        user.Id = id;
        user.Username = username;
        user.Email = email;
        user.Password = password;
        user.FullName = fullName;
        user.CreatedDate = createdDate;
        user.IsActive = isActive;

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Equal(fullName, user.FullName);
        Assert.Equal(createdDate, user.CreatedDate);
        Assert.False(user.IsActive);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("admin")]
    [InlineData("test_user")]
    public void Username_SetVariousValidValues_ReturnsCorrectValue(string username)
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = username;

        // Assert
        Assert.Equal(username, user.Username);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test@test.org")]
    [InlineData("admin@domain.co.uk")]
    public void Email_SetVariousValidValues_ReturnsCorrectValue(string email)
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = email;

        // Assert
        Assert.Equal(email, user.Email);
    }
}
