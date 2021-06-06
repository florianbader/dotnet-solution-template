using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Database
{
    public static class ApplicationDbContextExtensions
    {
        public static Task<TEntity> GetOrThrowAsync<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            where TEntity : class
        {
            var entity = dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
            if (entity is null)
            {
                throw new EntityNotFoundException();
            }

            return entity;
        }
    }
}
