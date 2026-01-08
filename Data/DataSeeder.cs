namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.99m, Stock = 100 },
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.99m, Stock = 50 },
                new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.99m, Stock = 75 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Id = 1, Username = "admin", Email = "admin@example.com", PasswordHash = "hash1" },
                new User { Id = 2, Username = "user", Email = "user@example.com", PasswordHash = "hash2" }
            );
            await context.SaveChangesAsync();
        }
    }
}
