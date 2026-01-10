using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product 1", Price = 10.99m, Description = "Sample product 1" },
                new Product { Id = 2, Name = "Product 2", Price = 20.99m, Description = "Sample product 2" }
            );
            await context.SaveChangesAsync();
        }
    }
}
