using Microsoft.EntityFrameworkCore;

namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Products.AnyAsync())
        {
            await context.Products.AddRangeAsync(
                new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Price = 1299.99m, Stock = 10 },
                new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Stock = 50 },
                new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 89.99m, Stock = 25 }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            await context.Users.AddRangeAsync(
                new User { Id = 1, Username = "admin", Email = "admin@example.com", PasswordHash = "hashed_password" },
                new User { Id = 2, Username = "user1", Email = "user1@example.com", PasswordHash = "hashed_password" }
            );
            await context.SaveChangesAsync();
        }
    }
}
