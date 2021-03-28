using Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Features.Todo
{
    public class TodoEntityConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.IsKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(256);
        }
    }
}
