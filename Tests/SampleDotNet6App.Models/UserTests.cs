using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Models.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
    }

    [Fact]
    public void User_SetId_UpdatesIdProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 123;

        // Assert
        Assert.Equal(123, user.Id);
    }

    [Fact]
    public void User_SetUsername_UpdatesUsernameProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = "testuser";

        // Assert
        Assert.Equal("testuser", user.Username);
    }

    [Fact]
    public void User_SetEmail_UpdatesEmailProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void User_SetPasswordHash_UpdatesPasswordHashProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = "hashedpassword123";

        // Assert
        Assert.Equal("hashedpassword123", user.PasswordHash);
    }

    [Fact]
    public void User_SetAllProperties_RetainsValues()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 42;
        user.Username = "johndoe";
        user.Email = "john@example.com";
        user.PasswordHash = "secure_hash";

        // Assert
        Assert.Equal(42, user.Id);
        Assert.Equal("johndoe", user.Username);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("secure_hash", user.PasswordHash);
    }

    [Fact]
    public void User_InitializerSyntax_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = "hashed_password"
        };

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("admin", user.Username);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal("hashed_password", user.PasswordHash);
    }

    [Fact]
    public void User_EmptyUsername_AcceptsEmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = "";

        // Assert
        Assert.Equal("", user.Username);
    }

    [Fact]
    public void User_EmptyEmail_AcceptsEmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "";

        // Assert
        Assert.Equal("", user.Email);
    }

    [Fact]
    public void User_EmptyPasswordHash_AcceptsEmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = "";

        // Assert
        Assert.Equal("", user.PasswordHash);
    }

    [Fact]
    public void User_LongUsername_AcceptsLongString()
    {
        // Arrange
        var user = new User();
        var longUsername = new string('a', 100);

        // Act
        user.Username = longUsername;

        // Assert
        Assert.Equal(longUsername, user.Username);
    }

    [Fact]
    public void User_LongEmail_AcceptsLongString()
    {
        // Arrange
        var user = new User();
        var longEmail = new string('a', 100) + "@example.com";

        // Act
        user.Email = longEmail;

        // Assert
        Assert.Equal(longEmail, user.Email);
    }
}
