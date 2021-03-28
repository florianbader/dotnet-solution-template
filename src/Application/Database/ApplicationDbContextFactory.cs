using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Application.Database
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DatabaseConnectionString")
                ?? throw new InvalidOperationException("Database connection string was not found. Set the DatabaseConnectionString environment variable"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
