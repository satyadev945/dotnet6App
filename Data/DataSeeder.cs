using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.99m, CreatedDate = DateTime.UtcNow },
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.99m, CreatedDate = DateTime.UtcNow },
                new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.99m, CreatedDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Id = 1, Username = "user1", Email = "user1@example.com", CreatedDate = DateTime.UtcNow },
                new User { Id = 2, Username = "user2", Email = "user2@example.com", CreatedDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
        }
    }
}
