using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (context.Products.Any() || context.Users.Any())
        {
            return; // Database already seeded
        }

        // Seed Products
        var products = new[]
        {
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop for professionals",
                Price = 1299.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with precision tracking",
                Price = 29.99m,
                Stock = 200,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 3,
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with tactile switches",
                Price = 89.99m,
                Stock = 100,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products);

        // Seed Users
        var users = new[]
        {
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "hashed_password_here",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Username = "user1",
                Email = "user1@example.com",
                PasswordHash = "hashed_password_here",
                Role = "User",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);

        await context.SaveChangesAsync();
    }
}
