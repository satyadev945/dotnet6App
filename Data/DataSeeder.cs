using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for developers",
                    Price = 1299.99m,
                    Stock = 50
                },
                new Product
                {
                    Name = "Mouse",
                    Description = "Wireless ergonomic mouse",
                    Price = 29.99m,
                    Stock = 200
                },
                new Product
                {
                    Name = "Keyboard",
                    Description = "Mechanical keyboard with RGB lighting",
                    Price = 89.99m,
                    Stock = 100
                }
            };

            await context.Products.AddRangeAsync(products);
        }

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = "hashed_password_here",
                    FullName = "System Administrator",
                    IsActive = true
                },
                new User
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    Password = "hashed_password_here",
                    FullName = "Test User",
                    IsActive = true
                }
            };

            await context.Users.AddRangeAsync(users);
        }

        await context.SaveChangesAsync();
    }
}
