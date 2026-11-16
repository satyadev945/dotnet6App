using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Models;
using System.Security.Cryptography;
using System.Text;

namespace SampleDotNet6App.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Products.AnyAsync())
            {
                var products = new[]
                {
                    new Product { Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Category = "Electronics", StockQuantity = 50, IsAvailable = true },
                    new Product { Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Category = "Electronics", StockQuantity = 200, IsAvailable = true },
                    new Product { Name = "Keyboard", Description = "Mechanical keyboard", Price = 89.99m, Category = "Electronics", StockQuantity = 100, IsAvailable = true },
                    new Product { Name = "Monitor", Description = "27-inch 4K monitor", Price = 499.99m, Category = "Electronics", StockQuantity = 30, IsAvailable = true },
                    new Product { Name = "Desk Chair", Description = "Ergonomic office chair", Price = 349.99m, Category = "Furniture", StockQuantity = 25, IsAvailable = true }
                };

                await context.Products.AddRangeAsync(products);
            }

            if (!await context.Users.AnyAsync())
            {
                var users = new[]
                {
                    new User { Username = "admin", PasswordHash = HashPassword("Admin123!"), Email = "admin@sample.com", FirstName = "Admin", LastName = "User", Role = "Admin", IsActive = true },
                    new User { Username = "jane.smith", PasswordHash = HashPassword("Manager123!"), Email = "jane.smith@sample.com", FirstName = "Jane", LastName = "Smith", Role = "Manager", IsActive = true },
                    new User { Username = "john.doe", PasswordHash = HashPassword("User123!"), Email = "john.doe@sample.com", FirstName = "John", LastName = "Doe", Role = "User", IsActive = true }
                };

                await context.Users.AddRangeAsync(users);
            }

            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
