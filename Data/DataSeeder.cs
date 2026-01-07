using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                var products = new[]
                {
                    new Product { Id = 1, Name = "Product 1", Price = 10.99m, Description = "Sample product 1" },
                    new Product { Id = 2, Name = "Product 2", Price = 20.99m, Description = "Sample product 2" },
                    new Product { Id = 3, Name = "Product 3", Price = 30.99m, Description = "Sample product 3" }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var users = new[]
                {
                    new User { Id = 1, Username = "admin", Email = "admin@example.com", IsActive = true },
                    new User { Id = 2, Username = "user", Email = "user@example.com", IsActive = true }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
