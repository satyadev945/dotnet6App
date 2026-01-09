using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.99m, Category = "Category A" },
                    new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.99m, Category = "Category B" },
                    new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.99m, Category = "Category A" }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Id = 1, Username = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
                    new User { Id = 2, Username = "user2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
