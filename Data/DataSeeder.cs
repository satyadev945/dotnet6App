using SampleDotNet6App.Models;
using System.Security.Cryptography;
using System.Text;

namespace SampleDotNet6App.Data;

/// <summary>
/// Data seeder for initial application data
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seed initial data
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed users if none exist
        if (!context.Users.Any())
        {
            var users = new[]
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = HashPassword("Admin123!"),
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new User
                {
                    Username = "jane.smith",
                    Email = "jane.smith@example.com",
                    PasswordHash = HashPassword("Manager123!"),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Role = "Manager",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new User
                {
                    Username = "john.doe",
                    Email = "john.doe@example.com",
                    PasswordHash = HashPassword("User123!"),
                    FirstName = "John",
                    LastName = "Doe",
                    Role = "User",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        // Seed products if none exist
        if (!context.Products.Any())
        {
            var products = new[]
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for professionals",
                    Price = 1299.99m,
                    Category = "Electronics",
                    StockQuantity = 50,
                    Sku = "LAP-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse",
                    Price = 29.99m,
                    Category = "Electronics",
                    StockQuantity = 200,
                    Sku = "MOU-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Office Chair",
                    Description = "Comfortable ergonomic office chair",
                    Price = 249.99m,
                    Category = "Furniture",
                    StockQuantity = 30,
                    Sku = "CHR-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Desk Lamp",
                    Description = "LED desk lamp with adjustable brightness",
                    Price = 39.99m,
                    Category = "Lighting",
                    StockQuantity = 100,
                    Sku = "LMP-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Notebook",
                    Description = "Premium quality notebook",
                    Price = 9.99m,
                    Category = "Stationery",
                    StockQuantity = 500,
                    Sku = "NOT-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Hash password using SHA256
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
