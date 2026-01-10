using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Description = "Description for Product 1",
                    Price = 99.99m,
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Description = "Description for Product 2",
                    Price = 149.99m,
                    IsActive = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Description = "Description for Product 3",
                    Price = 199.99m,
                    IsActive = true
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_password_here",
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "hashed_password_here",
                    IsActive = true
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
