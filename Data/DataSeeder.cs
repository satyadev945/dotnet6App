using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "Sample Product 1", Description = "Description 1", Price = 9.99m, Stock = 100 },
                    new Product { Name = "Sample Product 2", Description = "Description 2", Price = 19.99m, Stock = 50 },
                    new Product { Name = "Sample Product 3", Description = "Description 3", Price = 29.99m, Stock = 25 }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
