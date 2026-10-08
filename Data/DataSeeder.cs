using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    /// <summary>
    /// Seeds initial data into the database.
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Products if none exist
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
                        SKU = "LAP-001",
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
                        SKU = "MOU-001",
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
                        SKU = "HUB-001",
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
                        SKU = "KEY-001",
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
                        SKU = "MON-001",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Products.AddRangeAsync(products);
            }

            // Seed Users if none exist
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = BCryptHash("Admin@123"),
                        FirstName = "System",
                        LastName = "Administrator",
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "john.doe",
                        Email = "john.doe@example.com",
                        PasswordHash = BCryptHash("User@123"),
                        FirstName = "John",
                        LastName = "Doe",
                        Role = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Users.AddRangeAsync(users);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Simple password hashing placeholder (use BCrypt or similar in production).
        /// </summary>
        private static string BCryptHash(string password)
        {
            // In production, use a proper password hashing library like BCrypt.Net
            // This is a simplified hash for demonstration purposes
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + "salt_value_here");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
