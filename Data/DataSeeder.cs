using Microsoft.EntityFrameworkCore;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed products if none exist
        if (!await context.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for professionals",
                    Price = 1299.99m,
                    Stock = 50,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with precision tracking",
                    Price = 29.99m,
                    Stock = 150,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Mechanical Keyboard",
                    Description = "RGB mechanical keyboard with tactile switches",
                    Price = 99.99m,
                    Stock = 75,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "USB-C Hub",
                    Description = "Multi-port USB-C hub with HDMI and Ethernet",
                    Price = 49.99m,
                    Stock = 100,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Webcam HD",
                    Description = "1080p HD webcam with noise-canceling microphone",
                    Price = 79.99m,
                    Stock = 60,
                    CreatedDate = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        // Seed users if none exist
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    CreatedDate = DateTime.UtcNow
                },
                new User
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    CreatedDate = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
