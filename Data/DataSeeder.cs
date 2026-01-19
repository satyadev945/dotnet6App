namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static Task SeedAsync(ApplicationDbContext context)
    {
        return Task.CompletedTask;
    }
}
