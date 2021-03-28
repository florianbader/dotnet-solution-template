using System;
using Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi
{
    public static partial class EntityFramework
    {
        public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<ApplicationDbContext>(CreateOptionsAction(
                configuration["DatabaseConnectionString"] ?? throw new InvalidOperationException("Database connection string was not found. Set the DatabaseConnectionString environment variable")));

        private static Action<DbContextOptionsBuilder> CreateOptionsAction(string connectionString)
            => (DbContextOptionsBuilder options) =>
            {
                var shouldUseAccessToken = connectionString is not null
                    && !connectionString.Contains("Integrated Security", StringComparison.OrdinalIgnoreCase)
                    && !connectionString.Contains("User Id", StringComparison.OrdinalIgnoreCase);

                if (shouldUseAccessToken)
                {
                    options.AddInterceptors(new AadTokenInjectorDbInterceptor());
                }

                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            };
    }
}
