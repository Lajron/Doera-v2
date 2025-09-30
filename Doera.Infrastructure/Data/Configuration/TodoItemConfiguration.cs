using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doera.Infrastructure.Data.Configuration {
    internal class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem> {
        public void Configure(EntityTypeBuilder<TodoItem> builder) {
            builder.Property(t => t.Title).HasMaxLength(200);
            builder.Property(t => t.Status).HasConversion<string>();
            builder.Property(t => t.Priority).HasConversion<string>();

            builder.HasOne(t => t.User)
                .WithMany(u => u.TodoItems)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.TodoList)
                .WithMany(l => l.TodoItems)
                .HasForeignKey(t => t.TodoListId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(t => t.TodoItemTags)
                .WithOne(tt => tt.TodoItem)
                .HasForeignKey(tt => tt.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => new { t.UserId, t.TodoListId });
        }
    }
}
