using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    /// <param name="context">The database context</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed products if none exist
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for development",
                    Price = 1299.99m,
                    QuantityInStock = 50,
                    Category = "Electronics",
                    IsActive = true
                },
                new Product
                {
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse",
                    Price = 29.99m,
                    QuantityInStock = 200,
                    Category = "Accessories",
                    IsActive = true
                },
                new Product
                {
                    Name = "Mechanical Keyboard",
                    Description = "RGB mechanical keyboard",
                    Price = 89.99m,
                    QuantityInStock = 100,
                    Category = "Accessories",
                    IsActive = true
                },
                new Product
                {
                    Name = "Monitor 27\"",
                    Description = "4K UHD monitor",
                    Price = 399.99m,
                    QuantityInStock = 75,
                    Category = "Electronics",
                    IsActive = true
                },
                new Product
                {
                    Name = "USB-C Hub",
                    Description = "Multi-port USB-C hub",
                    Price = 49.99m,
                    QuantityInStock = 150,
                    Category = "Accessories",
                    IsActive = true
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed users if none exist
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_password_here", // In production, use proper password hashing
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsActive = true
                },
                new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "hashed_password_here",
                    FirstName = "John",
                    LastName = "Doe",
                    Role = "User",
                    IsActive = true
                },
                new User
                {
                    Username = "user2",
                    Email = "user2@example.com",
                    PasswordHash = "hashed_password_here",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Role = "User",
                    IsActive = true
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}
