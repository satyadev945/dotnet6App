using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    /// <summary>
    /// Data seeder for initial database data
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds the database with initial data
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        FirstName = "Admin",
                        LastName = "User",
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "jane.smith",
                        Email = "jane.smith@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                        FirstName = "Jane",
                        LastName = "Smith",
                        Role = "Manager",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "john.doe",
                        Email = "john.doe@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        FirstName = "John",
                        LastName = "Doe",
                        Role = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Laptop",
                        Description = "High-performance laptop for professionals",
                        Price = 1299.99m,
                        Category = "Electronics",
                        StockQuantity = 50,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Wireless Mouse",
                        Description = "Ergonomic wireless mouse",
                        Price = 29.99m,
                        Category = "Electronics",
                        StockQuantity = 200,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Office Chair",
                        Description = "Comfortable ergonomic office chair",
                        Price = 249.99m,
                        Category = "Furniture",
                        StockQuantity = 30,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Desk Lamp",
                        Description = "LED desk lamp with adjustable brightness",
                        Price = 39.99m,
                        Category = "Lighting",
                        StockQuantity = 100,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Notebook",
                        Description = "Premium quality notebook",
                        Price = 9.99m,
                        Category = "Stationery",
                        StockQuantity = 500,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
