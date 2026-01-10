namespace SampleDotNet6App.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds sample data
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Products.Any())
        {
            return; // Database already seeded
        }

        var products = new[]
        {
            new Product
            {
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 1299.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 29.99m,
                Stock = 200,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 89.99m,
                Stock = 100,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);

        var users = new[]
        {
            new User
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "hashed_password_here",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password_here",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
