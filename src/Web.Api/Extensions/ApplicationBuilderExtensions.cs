using Infrastructure.Database;
using Scalar.AspNetCore;

namespace Web.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseScalarUi(this WebApplication app)
    {
        app.MapScalarApiReference(options =>
        {
            options.Title = "Multi-Tier Caching";
            options.Theme = ScalarTheme.DeepSpace;
            options.Layout = ScalarLayout.Modern;
            options.HideClientButton = true;
        });

        return app;
    }

    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        try
        {
            ApplicationDbContext context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            await context.SeedDataAsync();
        }
        catch (Exception ex)
        {
            ILogger<Program> logger = scope.ServiceProvider
                .GetRequiredService<ILogger<Program>>();

            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
