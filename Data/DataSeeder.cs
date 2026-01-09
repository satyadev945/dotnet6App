using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            var products = new[]
            {
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.99m, Stock = 100 },
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.99m, Stock = 50 },
                new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.99m, Stock = 75 }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var users = new[]
            {
                new User { Id = 1, Username = "admin", Email = "admin@example.com", PasswordHash = "hashed_password" },
                new User { Id = 2, Username = "user1", Email = "user1@example.com", PasswordHash = "hashed_password" }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}
