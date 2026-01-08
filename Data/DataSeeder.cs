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
                new Product { Id = 1, Name = "Product 1", Price = 10.99m, Description = "Sample product 1" },
                new Product { Id = 2, Name = "Product 2", Price = 20.99m, Description = "Sample product 2" },
                new Product { Id = 3, Name = "Product 3", Price = 30.99m, Description = "Sample product 3" }
            };
            context.Products.AddRange(products);
        }

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2" }
            };
            context.Users.AddRange(users);
        }

        await context.SaveChangesAsync();
    }
}
