using System.Diagnostics.CodeAnalysis;
using Application.Features.Todo;
using Microsoft.EntityFrameworkCore;

namespace Application.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext([NotNull] DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Todo> Todo => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly, t => t.Name.EndsWith("EntityConfiguration"));
}
