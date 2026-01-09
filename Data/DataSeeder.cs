using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Products if none exist
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Sample Product 1",
                    Description = "This is a sample product for testing",
                    Price = 29.99m,
                    Stock = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Sample Product 2",
                    Description = "Another sample product",
                    Price = 49.99m,
                    Stock = 50
                },
                new Product
                {
                    Id = 3,
                    Name = "Sample Product 3",
                    Description = "Yet another sample product",
                    Price = 19.99m,
                    Stock = 200
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
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_password_here", // In real app, use proper password hashing
                    FirstName = "Admin",
                    LastName = "User"
                },
                new User
                {
                    Id = 2,
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = "hashed_password_here",
                    FirstName = "Test",
                    LastName = "User"
                }
            };

            await context.Users.AddRangeAsync(users);
        }

        await context.SaveChangesAsync();
    }
}
