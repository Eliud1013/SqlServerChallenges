using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Data;

namespace SqlServerChallenges.Web.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();
        
        return app;
    }
}