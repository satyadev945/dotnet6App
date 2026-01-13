using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data.Tests;

public class ApplicationDbContextTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Products_DbSet_IsNotNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var products = context.Products;

        // Assert
        Assert.NotNull(products);
    }

    [Fact]
    public void Users_DbSet_IsNotNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var users = context.Users;

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public void Products_AddProduct_SavesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 10
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        // Assert
        Assert.Equal(1, context.Products.Count());
        Assert.Equal("Test Product", context.Products.First().Name);
    }

    [Fact]
    public void Users_AddUser_SavesSuccessfully()
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

        // Act
        context.Users.Add(user);
        context.SaveChanges();

        // Assert
        Assert.Equal(1, context.Users.Count());
        Assert.Equal("testuser", context.Users.First().Username);
    }

    [Fact]
    public void Products_AddMultipleProducts_SavesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var products = new[]
        {
            new Product { Name = "Product1", Price = 10m, Stock = 5 },
            new Product { Name = "Product2", Price = 20m, Stock = 10 },
            new Product { Name = "Product3", Price = 30m, Stock = 15 }
        };

        // Act
        context.Products.AddRange(products);
        context.SaveChanges();

        // Assert
        Assert.Equal(3, context.Products.Count());
    }

    [Fact]
    public void Users_AddMultipleUsers_SavesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var users = new[]
        {
            new User { Username = "user1", Email = "user1@test.com", Password = "pass1" },
            new User { Username = "user2", Email = "user2@test.com", Password = "pass2" },
            new User { Username = "user3", Email = "user3@test.com", Password = "pass3" }
        };

        // Act
        context.Users.AddRange(users);
        context.SaveChanges();

        // Assert
        Assert.Equal(3, context.Users.Count());
    }

    [Fact]
    public void Products_UpdateProduct_SavesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var product = new Product { Name = "Original", Price = 50m, Stock = 10 };
        context.Products.Add(product);
        context.SaveChanges();

        // Act
        product.Name = "Updated";
        product.Price = 75m;
        context.SaveChanges();

        // Assert
        var updated = context.Products.First();
        Assert.Equal("Updated", updated.Name);
        Assert.Equal(75m, updated.Price);
    }

    [Fact]
    public void Users_UpdateUser_SavesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User { Username = "original", Email = "original@test.com", Password = "pass" };
        context.Users.Add(user);
        context.SaveChanges();

        // Act
        user.Username = "updated";
        user.Email = "updated@test.com";
        context.SaveChanges();

        // Assert
        var updated = context.Users.First();
        Assert.Equal("updated", updated.Username);
        Assert.Equal("updated@test.com", updated.Email);
    }

    [Fact]
    public void Products_DeleteProduct_RemovesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var product = new Product { Name = "Test", Price = 10m, Stock = 5 };
        context.Products.Add(product);
        context.SaveChanges();

        // Act
        context.Products.Remove(product);
        context.SaveChanges();

        // Assert
        Assert.Equal(0, context.Products.Count());
    }

    [Fact]
    public void Users_DeleteUser_RemovesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user = new User { Username = "test", Email = "test@test.com", Password = "pass" };
        context.Users.Add(user);
        context.SaveChanges();

        // Act
        context.Users.Remove(user);
        context.SaveChanges();

        // Assert
        Assert.Equal(0, context.Users.Count());
    }

    [Fact]
    public void Products_QueryByName_ReturnsCorrectProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Products.AddRange(
            new Product { Name = "Laptop", Price = 1000m, Stock = 5 },
            new Product { Name = "Mouse", Price = 25m, Stock = 50 },
            new Product { Name = "Keyboard", Price = 75m, Stock = 30 }
        );
        context.SaveChanges();

        // Act
        var laptop = context.Products.FirstOrDefault(p => p.Name == "Laptop");

        // Assert
        Assert.NotNull(laptop);
        Assert.Equal(1000m, laptop.Price);
        Assert.Equal(5, laptop.Stock);
    }

    [Fact]
    public void Users_QueryByUsername_ReturnsCorrectUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Users.AddRange(
            new User { Username = "admin", Email = "admin@test.com", Password = "pass1" },
            new User { Username = "user", Email = "user@test.com", Password = "pass2" }
        );
        context.SaveChanges();

        // Act
        var admin = context.Users.FirstOrDefault(u => u.Username == "admin");

        // Assert
        Assert.NotNull(admin);
        Assert.Equal("admin@test.com", admin.Email);
    }

    [Fact]
    public void Products_QueryByPriceRange_ReturnsCorrectProducts()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Products.AddRange(
            new Product { Name = "Cheap", Price = 10m, Stock = 100 },
            new Product { Name = "Medium", Price = 50m, Stock = 50 },
            new Product { Name = "Expensive", Price = 200m, Stock = 10 }
        );
        context.SaveChanges();

        // Act
        var mediumPriced = context.Products.Where(p => p.Price >= 40m && p.Price <= 100m).ToList();

        // Assert
        Assert.Single(mediumPriced);
        Assert.Equal("Medium", mediumPriced.First().Name);
    }

    [Fact]
    public void Users_QueryByIsActive_ReturnsCorrectUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Users.AddRange(
            new User { Username = "active1", Email = "active1@test.com", Password = "pass", IsActive = true },
            new User { Username = "active2", Email = "active2@test.com", Password = "pass", IsActive = true },
            new User { Username = "inactive", Email = "inactive@test.com", Password = "pass", IsActive = false }
        );
        context.SaveChanges();

        // Act
        var activeUsers = context.Users.Where(u => u.IsActive).ToList();

        // Assert
        Assert.Equal(2, activeUsers.Count);
    }

    [Fact]
    public void Products_EmptyDatabase_ReturnsZeroCount()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var count = context.Products.Count();

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void Users_EmptyDatabase_ReturnsZeroCount()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var count = context.Users.Count();

        // Assert
        Assert.Equal(0, count);
    }
}
