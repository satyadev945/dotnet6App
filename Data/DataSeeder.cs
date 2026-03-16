using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    /// <summary>
    /// Data seeder for initial data
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds initial data into the database
        /// </summary>
        /// <param name="context">Database context</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        Role = "Admin",
                        FirstName = "Admin",
                        LastName = "User"
                    },
                    new User
                    {
                        Username = "jane.smith",
                        Email = "jane.smith@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                        Role = "Manager",
                        FirstName = "Jane",
                        LastName = "Smith"
                    },
                    new User
                    {
                        Username = "john.doe",
                        Email = "john.doe@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        Role = "User",
                        FirstName = "John",
                        LastName = "Doe"
                    }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Seed products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Laptop",
                        Description = "High-performance laptop",
                        Price = 1299.99m,
                        Category = "Electronics",
                        StockQuantity = 50
                    },
                    new Product
                    {
                        Name = "Smartphone",
                        Description = "Latest smartphone model",
                        Price = 799.99m,
                        Category = "Electronics",
                        StockQuantity = 100
                    },
                    new Product
                    {
                        Name = "Desk Chair",
                        Description = "Ergonomic office chair",
                        Price = 299.99m,
                        Category = "Furniture",
                        StockQuantity = 25
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
