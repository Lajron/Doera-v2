using Doera.Core.Entities;
using Doera.Core.Enums;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Doera.Infrastructure.Repositories {
    internal class TodoItemRepository(
            ApplicationDbContext _db
        ) : BaseRepository<TodoItem>(_db), ITodoItemRepository {
        public async Task<int> ExecuteUpdateStatusAsync(Guid todoItemId, Guid userId, TodoStatus status) {
            return await _dbSet
                .Where(i => i.Id == todoItemId && i.UserId == userId)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(i => i.Status, status)
                    .SetProperty(i => i.UpdatedAt, DateTimeOffset.UtcNow)
                );
        }

        public async Task<IEnumerable<TodoItem>> GetAllForUserAsync(Guid userId, Guid? todoListId = null, IEnumerable<Guid>? requestIds = null) {
            var query = _dbSet.AsQueryable();

            query = query.Where(i => i.UserId == userId);

            if (todoListId.HasValue)
                query = query.Where(i => i.TodoListId == todoListId.Value);

            if (requestIds is not null && requestIds.Any())
                query = query.Where(i => requestIds.Contains(i.Id));

            return await query.ToListAsync();
        }

        public async Task<int> GetCountForListAsync(Guid listId) {
            return await _dbSet.CountAsync(i => i.TodoListId == listId);
        }
        public async Task<int> ExecuteDeleteByListAsync(Guid todoListId) =>
            await _dbSet.Where(i => i.TodoListId == todoListId).ExecuteDeleteAsync();
    }
}