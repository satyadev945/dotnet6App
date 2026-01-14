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
                new() { Name = "Product 1", Price = 10.99m, Category = "Category A", Description = "Test product", CreatedDate = DateTime.UtcNow },
                new() { Name = "Product 2", Price = 20.99m, Category = "Category B", Description = "Test product", CreatedDate = DateTime.UtcNow }
            };
            context.Products.AddRange(products);
        }

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new() { Username = "admin", Email = "admin@test.com", PasswordHash = "hash", Role = "Admin", CreatedDate = DateTime.UtcNow }
            };
            context.Users.AddRange(users);
        }

        await context.SaveChangesAsync();
    }
}
