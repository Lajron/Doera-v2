using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Data.Configuration {
    internal class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem> {
        public void Configure(EntityTypeBuilder<TodoItem> builder) {
            builder.Property(t => t.Title)
                .HasMaxLength(200);

            builder.HasOne(t => t.User)
                .WithMany(u => u.TodoItems)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TodoItemTags)
                .WithOne(tt => tt.TodoItem)
                .HasForeignKey(tt => tt.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
