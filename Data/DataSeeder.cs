using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data
{
    /// <summary>
    /// Provides seed data for the application database.
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds the database with initial data asynchronously.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Sample Product 1",
                        Description = "A sample product for testing",
                        Price = 9.99m,
                        StockQuantity = 100,
                        Category = "Electronics",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Sample Product 2",
                        Description = "Another sample product",
                        Price = 19.99m,
                        StockQuantity = 50,
                        Category = "Books",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Sample Product 3",
                        Description = "Yet another sample product",
                        Price = 29.99m,
                        StockQuantity = 25,
                        Category = "Clothing",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Products.AddRangeAsync(products);
            }

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = BCryptHash("Admin@123"),
                        FirstName = "Admin",
                        LastName = "User",
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "testuser",
                        Email = "testuser@example.com",
                        PasswordHash = BCryptHash("Test@123"),
                        FirstName = "Test",
                        LastName = "User",
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
        /// Simple password hashing placeholder (replace with proper BCrypt in production).
        /// </summary>
        private static string BCryptHash(string password)
        {
            // Simple hash for seeding purposes; use BCrypt.Net or similar in production
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
