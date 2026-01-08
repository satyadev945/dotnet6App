namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Product 1", Price = 10.99m },
                new Product { Name = "Product 2", Price = 20.99m },
                new Product { Name = "Product 3", Price = 30.99m }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Username = "user1", Email = "user1@example.com" },
                new User { Username = "user2", Email = "user2@example.com" }
            );
            await context.SaveChangesAsync();
        }
    }
}
