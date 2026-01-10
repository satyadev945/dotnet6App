namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Products.Any())
        {
            return;
        }

        var products = new[]
        {
            new Product
            {
                Name = "Sample Product 1",
                Description = "Description for product 1",
                Price = 29.99m,
                Stock = 100,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Sample Product 2",
                Description = "Description for product 2",
                Price = 49.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Sample Product 3",
                Description = "Description for product 3",
                Price = 19.99m,
                Stock = 200,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);

        var users = new[]
        {
            new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);

        await context.SaveChangesAsync();
    }
}
