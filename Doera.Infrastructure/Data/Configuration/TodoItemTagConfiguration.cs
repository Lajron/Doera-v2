using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Doera.Infrastructure.Data.Configuration {
    internal class TodoItemTagConfiguration : IEntityTypeConfiguration<TodoItemTag> {
        public void Configure(EntityTypeBuilder<TodoItemTag> builder) {
            builder.HasIndex(tt => new { tt.TodoItemId, tt.TagId }).IsUnique();

            builder.HasOne(tt => tt.TodoItem)
                .WithMany(t => t.TodoItemTags)
                .HasForeignKey(tt => tt.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tt => tt.Tag)
                .WithMany(t => t.TodoItemTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
