using Doera.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doera.Infrastructure.Data.Configuration {
    internal class TagConfiguration : IEntityTypeConfiguration<Tag> {
        public void Configure(EntityTypeBuilder<Tag> builder) {
            builder.Property(t => t.NormalizedName)
                   .IsRequired()
                   .HasMaxLength(128);
            builder.Property(t => t.DisplayName)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.HasIndex(t => new { t.UserId, t.NormalizedName })
                   .IsUnique();

            builder.HasOne(t => t.User)
                   .WithMany(u => u.Tags)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TodoItemTags)
                   .WithOne(tt => tt.Tag)
                   .HasForeignKey(tt => tt.TagId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
