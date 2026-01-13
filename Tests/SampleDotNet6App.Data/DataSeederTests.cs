using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data.Tests;

public class DataSeederTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsProductsAndUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(3, context.Products.Count());
        Assert.Equal(2, context.Users.Count());
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsCorrectProducts()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.Contains(products, p => p.Name == "Laptop");
        Assert.Contains(products, p => p.Name == "Mouse");
        Assert.Contains(products, p => p.Name == "Keyboard");
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsLaptopWithCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var laptop = await context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop");
        Assert.NotNull(laptop);
        Assert.Equal("High-performance laptop for developers", laptop.Description);
        Assert.Equal(1299.99m, laptop.Price);
        Assert.Equal(50, laptop.Stock);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsMouseWithCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var mouse = await context.Products.FirstOrDefaultAsync(p => p.Name == "Mouse");
        Assert.NotNull(mouse);
        Assert.Equal("Wireless ergonomic mouse", mouse.Description);
        Assert.Equal(29.99m, mouse.Price);
        Assert.Equal(200, mouse.Stock);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsKeyboardWithCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var keyboard = await context.Products.FirstOrDefaultAsync(p => p.Name == "Keyboard");
        Assert.NotNull(keyboard);
        Assert.Equal("Mechanical keyboard with RGB lighting", keyboard.Description);
        Assert.Equal(89.99m, keyboard.Price);
        Assert.Equal(100, keyboard.Stock);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsCorrectUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.Contains(users, u => u.Username == "admin");
        Assert.Contains(users, u => u.Username == "testuser");
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsAdminWithCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        Assert.NotNull(admin);
        Assert.Equal("admin@example.com", admin.Email);
        Assert.Equal("hashed_password_here", admin.Password);
        Assert.Equal("System Administrator", admin.FullName);
        Assert.True(admin.IsActive);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsTestUserWithCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var testUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(testUser);
        Assert.Equal("test@example.com", testUser.Email);
        Assert.Equal("hashed_password_here", testUser.Password);
        Assert.Equal("Test User", testUser.FullName);
        Assert.True(testUser.IsActive);
    }

    [Fact]
    public async Task SeedAsync_WithExistingProducts_DoesNotSeedProductsAgain()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Products.Add(new Product { Name = "Existing Product", Price = 10m, Stock = 5 });
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(1, context.Products.Count());
    }

    [Fact]
    public async Task SeedAsync_WithExistingUsers_DoesNotSeedUsersAgain()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Users.Add(new User { Username = "existing", Email = "existing@test.com", Password = "pass" });
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(1, context.Users.Count());
    }

    [Fact]
    public async Task SeedAsync_WithExistingProducts_StillSeedsUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Products.Add(new Product { Name = "Existing Product", Price = 10m, Stock = 5 });
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(1, context.Products.Count());
        Assert.Equal(2, context.Users.Count());
    }

    [Fact]
    public async Task SeedAsync_WithExistingUsers_StillSeedsProducts()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Users.Add(new User { Username = "existing", Email = "existing@test.com", Password = "pass" });
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(3, context.Products.Count());
        Assert.Equal(1, context.Users.Count());
    }

    [Fact]
    public async Task SeedAsync_CallTwice_DoesNotDuplicateData()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        await DataSeeder.SeedAsync(context);
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(3, context.Products.Count());
        Assert.Equal(2, context.Users.Count());
    }
}
