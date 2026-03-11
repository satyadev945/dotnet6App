using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    /// <summary>
    /// Data seeder for initial data
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seed initial data
        /// </summary>
        /// <param name="context">Database context</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Laptop",
                        Description = "High-performance laptop",
                        Price = 1299.99m,
                        Stock = 50,
                        IsActive = true
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Mouse",
                        Description = "Wireless mouse",
                        Price = 29.99m,
                        Stock = 200,
                        IsActive = true
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Keyboard",
                        Description = "Mechanical keyboard",
                        Price = 89.99m,
                        Stock = 100,
                        IsActive = true
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            // Seed users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = "AQAAAAEAACcQAAAAEJ1234567890", // This is a placeholder
                        IsActive = true
                    },
                    new User
                    {
                        Id = 2,
                        Username = "user",
                        Email = "user@example.com",
                        PasswordHash = "AQAAAAEAACcQAAAAEJ0987654321", // This is a placeholder
                        IsActive = true
                    }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
