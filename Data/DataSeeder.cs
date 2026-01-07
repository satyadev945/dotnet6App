using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Products
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for development",
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
                    Price = 149.99m,
                    Stock = 100
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed Users
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_password_here",
                    IsActive = true
                },
                new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "hashed_password_here",
                    IsActive = true
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}
