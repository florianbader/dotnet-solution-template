using Application.Database;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public static partial class EntityFramework
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        try
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Your local database could not be migrated. Fix the problems or delete your local database to proceed.", ex);
        }
    }
}
