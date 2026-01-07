namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            var products = new[]
            {
                new Product { Name = "Product 1", Price = 10.99m, Description = "Sample product 1" },
                new Product { Name = "Product 2", Price = 20.99m, Description = "Sample product 2" },
                new Product { Name = "Product 3", Price = 30.99m, Description = "Sample product 3" }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var users = new[]
            {
                new User { Username = "admin", Email = "admin@example.com", PasswordHash = "hash1" },
                new User { Username = "user1", Email = "user1@example.com", PasswordHash = "hash2" }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
