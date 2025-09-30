using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Doera.Infrastructure.Repositories {
    internal class TodoItemRepository(
            ApplicationDbContext _db
        ) : BaseRepository<TodoItem>(_db), ITodoItemRepository {
        public async Task<int> GetCountForListAsync(Guid listId) {
            return await _dbSet.CountAsync(i => i.TodoListId == listId);
        }
    }
}