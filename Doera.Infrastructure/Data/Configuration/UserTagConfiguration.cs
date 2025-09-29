using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Data.Configuration {
    internal class UserTagConfiguration : IEntityTypeConfiguration<UserTag> {
        public void Configure(EntityTypeBuilder<UserTag> builder) {
            builder.HasIndex(ut => new { ut.UserId, ut.TagId })
                .IsUnique();

            builder.HasOne(ut => ut.User)
                .WithMany(u => u.UserTags)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ut => ut.Tag)
                .WithMany(t => t.UserTags)
                .HasForeignKey(ut => ut.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // SQL Server does not support cascade delete cycles or multiple cascade paths.
            // Therefore, we set the delete behavior to Restrict for the relationship between UserTag and TodoItemTag.
            // Manual deletion of related TodoItemTag entities will be required when a UserTag is deleted.
            builder.HasMany(ut => ut.TodoItemTags)
                .WithOne(tt => tt.UserTag)
                .HasForeignKey(tt => tt.UserTagId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
