using SampleDotNet6App.Models;
using System.Security.Cryptography;
using System.Text;

namespace SampleDotNet6App.Data
{
    public static class DataSeeder
    {
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
                        PasswordHash = HashPassword("Admin123!"),
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
                        PasswordHash = HashPassword("Manager123!"),
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
                        PasswordHash = HashPassword("User123!"),
                        FirstName = "John",
                        LastName = "Doe",
                        Role = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Users.AddRangeAsync(users);
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Laptop Pro 15",
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
                        Description = "Ergonomic wireless mouse with long battery life",
                        Price = 49.99m,
                        Category = "Accessories",
                        StockQuantity = 200,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "USB-C Hub",
                        Description = "7-in-1 USB-C hub with HDMI, USB 3.0, and SD card reader",
                        Price = 79.99m,
                        Category = "Accessories",
                        StockQuantity = 150,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Mechanical Keyboard",
                        Description = "Compact mechanical keyboard with RGB backlight",
                        Price = 129.99m,
                        Category = "Accessories",
                        StockQuantity = 75,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "4K Monitor",
                        Description = "27-inch 4K IPS monitor with HDR support",
                        Price = 599.99m,
                        Category = "Electronics",
                        StockQuantity = 30,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Products.AddRangeAsync(products);
            }

            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
