using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Data.Tests;

public class ApplicationDbContextTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void ApplicationDbContext_Constructor_InitializesSuccessfully()
    {
        // Arrange & Act
        var context = GetInMemoryDbContext();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Products);
        Assert.NotNull(context.Users);
    }

    [Fact]
    public void ApplicationDbContext_ProductsDbSet_IsNotNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        var productsDbSet = context.Products;

        // Assert
        Assert.NotNull(productsDbSet);
    }

    [Fact]
    public void ApplicationDbContext_UsersDbSet_IsNotNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        var usersDbSet = context.Users;

        // Assert
        Assert.NotNull(usersDbSet);
    }

    [Fact]
    public async Task ApplicationDbContext_AddProduct_SavesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var product = new Product { Id = 1, Name = "Test Product", Price = 10m, Stock = 5 };

        // Act
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Assert
        var savedProduct = await context.Products.FindAsync(1);
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Product", savedProduct.Name);
    }

    [Fact]
    public async Task ApplicationDbContext_AddUser_SavesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com", PasswordHash = "hash" };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.Users.FindAsync(1);
        Assert.NotNull(savedUser);
        Assert.Equal("testuser", savedUser.Username);
    }

    [Fact]
    public async Task ApplicationDbContext_RemoveProduct_DeletesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var product = new Product { Id = 1, Name = "To Delete", Price = 10m, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        context.Products.Remove(product);
        await context.SaveChangesAsync();

        // Assert
        var deletedProduct = await context.Products.FindAsync(1);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task ApplicationDbContext_RemoveUser_DeletesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "todelete", Email = "delete@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // Assert
        var deletedUser = await context.Users.FindAsync(1);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task ApplicationDbContext_UpdateProduct_UpdatesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var product = new Product { Id = 1, Name = "Original", Price = 10m, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        product.Name = "Updated";
        product.Price = 20m;
        await context.SaveChangesAsync();

        // Assert
        var updatedProduct = await context.Products.FindAsync(1);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated", updatedProduct.Name);
        Assert.Equal(20m, updatedProduct.Price);
    }

    [Fact]
    public async Task ApplicationDbContext_UpdateUser_UpdatesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, Username = "original", Email = "orig@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.Username = "updated";
        user.Email = "updated@test.com";
        await context.SaveChangesAsync();

        // Assert
        var updatedUser = await context.Users.FindAsync(1);
        Assert.NotNull(updatedUser);
        Assert.Equal("updated", updatedUser.Username);
        Assert.Equal("updated@test.com", updatedUser.Email);
    }

    [Fact]
    public async Task ApplicationDbContext_AddMultipleProducts_SavesAllSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var products = new[]
        {
            new Product { Id = 1, Name = "Product 1", Price = 10m, Stock = 5 },
            new Product { Id = 2, Name = "Product 2", Price = 20m, Stock = 10 },
            new Product { Id = 3, Name = "Product 3", Price = 30m, Stock = 15 }
        };

        // Act
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Assert
        var savedProducts = await context.Products.ToListAsync();
        Assert.Equal(3, savedProducts.Count);
    }

    [Fact]
    public async Task ApplicationDbContext_AddMultipleUsers_SavesAllSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var users = new[]
        {
            new User { Id = 1, Username = "user1", Email = "user1@test.com", PasswordHash = "hash1" },
            new User { Id = 2, Username = "user2", Email = "user2@test.com", PasswordHash = "hash2" }
        };

        // Act
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Assert
        var savedUsers = await context.Users.ToListAsync();
        Assert.Equal(2, savedUsers.Count);
    }

    [Fact]
    public async Task ApplicationDbContext_QueryProducts_ReturnsCorrectResults()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Expensive", Price = 100m, Stock = 5 },
            new Product { Id = 2, Name = "Cheap", Price = 10m, Stock = 10 }
        );
        await context.SaveChangesAsync();

        // Act
        var expensiveProducts = await context.Products.Where(p => p.Price > 50m).ToListAsync();

        // Assert
        Assert.Single(expensiveProducts);
        Assert.Equal("Expensive", expensiveProducts[0].Name);
    }

    [Fact]
    public async Task ApplicationDbContext_QueryUsers_ReturnsCorrectResults()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Users.AddRange(
            new User { Id = 1, Username = "admin", Email = "admin@test.com", PasswordHash = "hash1" },
            new User { Id = 2, Username = "user", Email = "user@test.com", PasswordHash = "hash2" }
        );
        await context.SaveChangesAsync();

        // Act
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");

        // Assert
        Assert.NotNull(adminUser);
        Assert.Equal("admin@test.com", adminUser.Email);
    }
}
