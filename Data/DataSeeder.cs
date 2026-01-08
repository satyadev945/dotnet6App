namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product 1", Price = 99.99m },
                new Product { Id = 2, Name = "Product 2", Price = 149.99m }
            );
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Id = 1, Username = "admin", Email = "admin@example.com" },
                new User { Id = 2, Username = "user", Email = "user@example.com" }
            );
        }

        await context.SaveChangesAsync();
    }
}
