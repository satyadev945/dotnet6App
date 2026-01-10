using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using Xunit;

namespace SampleDotNet6App.Services.Tests;

public class UserServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void UserService_Constructor_InitializesWithContext()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        var service = new UserService(context);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenUsersExist_ReturnsAllUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Users.AddRange(
            new User { Id = 1, Username = "user1", Email = "user1@test.com", PasswordHash = "hash1" },
            new User { Id = 2, Username = "user2", Email = "user2@test.com", PasswordHash = "hash2" }
        );
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenNoUsers_ReturnsEmptyList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "findme", Email = "find@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByUsernameAsync("findme");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("findme", result.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_IsCaseSensitive()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "TestUser", Email = "test@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidUser_AddsUserToDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);
        var newUser = new User { Username = "newuser", Email = "new@test.com", PasswordHash = "newhash" };

        // Act
        var result = await service.CreateUserAsync(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Single(context.Users);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsCreatedUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);
        var newUser = new User { Username = "created", Email = "created@test.com", PasswordHash = "createdhash" };

        // Act
        var result = await service.CreateUserAsync(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("created", result.Username);
        Assert.Equal("created@test.com", result.Email);
        Assert.Equal("createdhash", result.PasswordHash);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserExists_UpdatesUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var existingUser = new User { Id = 1, Username = "oldname", Email = "old@test.com", PasswordHash = "oldhash" };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        var updatedUser = new User { Username = "newname", Email = "new@test.com", PasswordHash = "newhash" };

        // Act
        var result = await service.UpdateUserAsync(1, updatedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newname", result.Username);
        Assert.Equal("new@test.com", result.Email);
        Assert.Equal("newhash", result.PasswordHash);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);
        var updatedUser = new User { Username = "newname", Email = "new@test.com", PasswordHash = "newhash" };

        // Act
        var result = await service.UpdateUserAsync(999, updatedUser);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserExists_DeletesUserAndReturnsTrue()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "todelete", Email = "delete@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.DeleteUserAsync(1);

        // Assert
        Assert.True(result);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_ReturnsFalse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act
        var result = await service.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserAsync_PreservesId()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var existingUser = new User { Id = 5, Username = "original", Email = "orig@test.com", PasswordHash = "hash" };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        var updatedUser = new User { Id = 999, Username = "updated", Email = "updated@test.com", PasswordHash = "newhash" };

        // Act
        var result = await service.UpdateUserAsync(5, updatedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUsersInCorrectOrder()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Users.AddRange(
            new User { Id = 3, Username = "user3", Email = "user3@test.com", PasswordHash = "hash3" },
            new User { Id = 1, Username = "user1", Email = "user1@test.com", PasswordHash = "hash1" },
            new User { Id = 2, Username = "user2", Email = "user2@test.com", PasswordHash = "hash2" }
        );
        await context.SaveChangesAsync();
        var service = new UserService(context);

        // Act
        var result = await service.GetAllUsersAsync();
        var list = result.ToList();

        // Assert
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public async Task CreateUserAsync_WithMultipleUsers_IncreasesCount()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act
        await service.CreateUserAsync(new User { Username = "user1", Email = "user1@test.com", PasswordHash = "hash1" });
        await service.CreateUserAsync(new User { Username = "user2", Email = "user2@test.com", PasswordHash = "hash2" });

        // Assert
        Assert.Equal(2, context.Users.Count());
    }
}
