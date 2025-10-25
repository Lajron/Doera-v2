using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Doera.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Doera.Infrastructure.Repositories {
    internal class TagRepository(
            ApplicationDbContext _db,
            ISlugGenerator _slugGenerator,
            ICurrentUser _currentUser
        ) : BaseRepository<Tag>(_db), ITagRepository {

        public async Task ExecuteDeleteUnusedTagsAsync() {
            // Should have added a parameter (Guid? TodoItemId = null, Guid? userId = null)
            // This scans through the whole table, wtf was I thinking
            await _dbSet
                .Where(tag => !tag.TodoItemTags.Any())
                .ExecuteDeleteAsync();
        }

        public async Task<ICollection<TodoItemTag>> ResolveTagsAsync(IEnumerable<string> itemTags) {
            
            if (itemTags.Any() is false)
                return [];

            var userId = _currentUser.RequireUserId();

            var incomingMap = itemTags
                .Select(t => t.TrimStart('#').Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .GroupBy(t => _slugGenerator.GenerateTagSlug(t))
                .ToDictionary(
                    g => g.Key,
                    g => g.First(),
                    StringComparer.OrdinalIgnoreCase // this is redundant cuz of slugGenerator in this
                );

            // If the itemTags were originally [ " ", "", "     "]
            if (incomingMap.Count == 0)
                return [];

            // EF Core can't translate .ContainsKeys directly, so we convert to a list
            var slugs = incomingMap.Keys.ToList();

            var existingTags = await _dbSet
                .Where(tag => tag.UserId == userId && slugs.Contains(tag.NormalizedName))
                .ToDictionaryAsync(tag => tag.NormalizedName, tag => tag);

            var newTags = incomingMap
                .Where(map => !existingTags.ContainsKey(map.Key))
                .Select(map => new Tag {
                    NormalizedName = map.Key,
                    DisplayName = map.Value,
                    UserId = userId
                })
                .ToList();

            if (newTags.Count > 0) {
                await _dbSet.AddRangeAsync(newTags);
                // Maybe i could add SaveChangesAsync here, so the newTags get a valid Id
            }

            var resolvedTags = existingTags.Values.Concat(newTags);

            return resolvedTags
                .Select(tag => new TodoItemTag { Tag = tag })
                .ToList();
        }

    }
}