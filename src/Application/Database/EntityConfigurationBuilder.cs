using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Database;

public static class EntityConfigurationBuilder
{
    public static void IsKey<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, object?>> keyExpression)
        where TEntity : class
    {
        builder.HasKey(keyExpression);

        builder.Property(keyExpression)
            .IsUnicode(false)
            .HasMaxLength(36)
            .ValueGeneratedOnAdd();
    }
}
