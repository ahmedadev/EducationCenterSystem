using EducationCenterSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Api;

public static class WarmUpExtensions
{
    public static async Task WarmUpDatabaseAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // 1. Establish connection and load PostgreSQL type catalogs
            if (await dbContext.Database.CanConnectAsync())
            {
                // 2. Pre-compile EF Core query plan and model metadata
                _ = await dbContext.Students.AsNoTracking().Take(1).ToListAsync();
                _ = await dbContext.Teachers.AsNoTracking().Take(1).ToListAsync();
            }
        }
        catch
        {
            // Silently ignore warmup failure so server still boots
        }
    }
}
