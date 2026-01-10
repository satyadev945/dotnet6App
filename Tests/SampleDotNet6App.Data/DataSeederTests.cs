using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Data.Tests;

public class DataSeederTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SeedAsync_WhenProductsEmpty_AddsThreeProducts()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.Equal(3, products.Count);
        Assert.Contains(products, p => p.Name == "Product 1");
        Assert.Contains(products, p => p.Name == "Product 2");
        Assert.Contains(products, p => p.Name == "Product 3");
    }

    [Fact]
    public async Task SeedAsync_WhenProductsExist_DoesNotAddMoreProducts()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 99, Name = "Existing Product", Price = 1.0m, Stock = 1 });
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.Single(products);
        Assert.Equal("Existing Product", products[0].Name);
    }

    [Fact]
    public async Task SeedAsync_WhenUsersEmpty_AddsTwoUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.Equal(2, users.Count);
        Assert.Contains(users, u => u.Username == "admin");
        Assert.Contains(users, u => u.Username == "user1");
    }

    [Fact]
    public async Task SeedAsync_WhenUsersExist_DoesNotAddMoreUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Users.Add(new User { Id = 99, Username = "existing", Email = "test@test.com", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.Single(users);
        Assert.Equal("existing", users[0].Username);
    }

    [Fact]
    public async Task SeedAsync_SeedsCorrectProductDetails()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var product1 = await context.Products.FirstOrDefaultAsync(p => p.Id == 1);
        Assert.NotNull(product1);
        Assert.Equal("Product 1", product1.Name);
        Assert.Equal("Description 1", product1.Description);
        Assert.Equal(10.99m, product1.Price);
        Assert.Equal(100, product1.Stock);
    }

    [Fact]
    public async Task SeedAsync_SeedsCorrectUserDetails()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Id == 1);
        Assert.NotNull(admin);
        Assert.Equal("admin", admin.Username);
        Assert.Equal("admin@example.com", admin.Email);
        Assert.Equal("hashed_password", admin.PasswordHash);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyContext_SeedsBothProductsAndUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.Equal(3, await context.Products.CountAsync());
        Assert.Equal(2, await context.Users.CountAsync());
    }
}
