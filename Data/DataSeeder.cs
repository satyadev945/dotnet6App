namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static Task SeedAsync(ApplicationDbContext context)
    {
        // Add seed data logic here if needed
        return Task.CompletedTask;
    }
}
