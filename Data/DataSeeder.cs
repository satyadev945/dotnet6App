using Microsoft.EntityFrameworkCore;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var products = new[]
        {
            new Product { Name = "Product 1", Price = 10.99m, Description = "Sample product 1" },
            new Product { Name = "Product 2", Price = 20.99m, Description = "Sample product 2" },
            new Product { Name = "Product 3", Price = 30.99m, Description = "Sample product 3" }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
