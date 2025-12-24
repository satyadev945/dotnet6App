using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

/// <summary>
/// Provides methods for seeding initial data into the database.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds the database with initial data asynchronously.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Sample Product 1", Price = 19.99m, Stock = 100 },
                new Product { Id = 2, Name = "Sample Product 2", Price = 29.99m, Stock = 50 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Id = 1, Username = "admin", Email = "admin@example.com" },
                new User { Id = 2, Username = "user", Email = "user@example.com" }
            );
            await context.SaveChangesAsync();
        }
    }
}
