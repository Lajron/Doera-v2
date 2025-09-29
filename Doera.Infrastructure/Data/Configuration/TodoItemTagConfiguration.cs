using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Data.Configuration {
    internal class TodoItemTagConfiguration : IEntityTypeConfiguration<TodoItemTag> {
        public void Configure(EntityTypeBuilder<TodoItemTag> builder) {
            builder.HasKey(tt => new { tt.TodoItemId, tt.UserTagId });

            builder.HasOne(tt => tt.TodoItem)
                .WithMany(t => t.TodoItemTags)
                .HasForeignKey(tt => tt.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tt => tt.UserTag)
                .WithMany(ut => ut.TodoItemTags)
                .HasForeignKey(tt => tt.UserTagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
