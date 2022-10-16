using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Application.Database;

public static class ApplicationDbContextExtensions
{
    public static async Task<TEntity> GetOrThrowAsync<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        where TEntity : class
    {
        var entity = await dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        if (entity is null)
        {
            throw new EntityNotFoundException();
        }

        return entity;
    }
}
