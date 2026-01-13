using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests;

public class UserServiceTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void Constructor_WithValidContext_CreatesInstance()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var service = new UserService(context);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithUsers_ReturnsAllUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Users.AddRange(
            new User { Username = "user1", Email = "user1@test.com", Password = "pass1" },
            new User { Username = "user2", Email = "user2@test.com", Password = "pass2" },
            new User { Username = "user3", Email = "user3@test.com", Password = "pass3" }
        );
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingId_ReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            FullName = "Test User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("Test User", result.FullName);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithNonExistentUsername_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithExistingUsername_ReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidUser_CreatesAndReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "securepassword",
            FullName = "New User"
        };

        // Act
        var result = await service.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("newuser@example.com", result.Email);
        Assert.Equal("New User", result.FullName);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
    }

    [Fact]
    public async Task CreateUserAsync_SetsCreatedDate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var user = new User { Username = "test", Email = "test@test.com", Password = "pass" };

        // Act
        var result = await service.CreateUserAsync(user);
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(result.CreatedDate >= beforeCreation);
        Assert.True(result.CreatedDate <= afterCreation);
    }

    [Fact]
    public async Task CreateUserAsync_AddsUserToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User { Username = "test", Email = "test@test.com", Password = "pass" };

        // Act
        await service.CreateUserAsync(user);

        // Assert
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User { Username = "test", Email = "test@test.com", Password = "pass" };

        // Act
        var result = await service.UpdateUserAsync(999, user);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingId_UpdatesAndReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingUser = new User
        {
            Username = "original",
            Email = "original@test.com",
            Password = "password",
            FullName = "Original Name",
            IsActive = true
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        var updatedUser = new User
        {
            Username = "updated",
            Email = "updated@test.com",
            FullName = "Updated Name",
            IsActive = false
        };

        // Act
        var result = await service.UpdateUserAsync(existingUser.Id, updatedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated", result.Username);
        Assert.Equal("updated@test.com", result.Email);
        Assert.Equal("Updated Name", result.FullName);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task UpdateUserAsync_DoesNotUpdatePassword()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingUser = new User
        {
            Username = "test",
            Email = "test@test.com",
            Password = "originalpassword"
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        var updatedUser = new User
        {
            Username = "test",
            Email = "test@test.com",
            Password = "newpassword"
        };

        // Act
        var result = await service.UpdateUserAsync(existingUser.Id, updatedUser);

        // Assert
        Assert.Equal("originalpassword", result.Password);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User { Username = "test", Email = "test@test.com", Password = "pass" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.DeleteUserAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_RemovesUserFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User { Username = "test", Email = "test@test.com", Password = "pass" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        await service.DeleteUserAsync(user.Id);

        // Assert
        Assert.Equal(0, await context.Users.CountAsync());
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.ValidateCredentialsAsync("nonexistent", "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithCorrectCredentials_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "correctpassword",
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.ValidateCredentialsAsync("testuser", "correctpassword");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "correctpassword",
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.ValidateCredentialsAsync("testuser", "wrongpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithInactiveUser_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "correctpassword",
            IsActive = false
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.ValidateCredentialsAsync("testuser", "correctpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateUserAsync_WithNullFullName_CreatesUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User
        {
            Username = "test",
            Email = "test@test.com",
            Password = "pass",
            FullName = null
        };

        // Act
        var result = await service.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.FullName);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNullFullName_UpdatesUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingUser = new User
        {
            Username = "test",
            Email = "test@test.com",
            Password = "pass",
            FullName = "Original Name"
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        var updatedUser = new User
        {
            Username = "test",
            Email = "test@test.com",
            FullName = null
        };

        // Act
        var result = await service.UpdateUserAsync(existingUser.Id, updatedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.FullName);
    }
}
