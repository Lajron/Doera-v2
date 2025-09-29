using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doera.Infrastructure.Data.Configuration {
    internal class TodoListConfiguration : IEntityTypeConfiguration<TodoList> {
        public void Configure(EntityTypeBuilder<TodoList> builder) {
            builder.Property(l => l.Name)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasOne(l => l.User)
                   .WithMany(u => u.TodoLists)
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}