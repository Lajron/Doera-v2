using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;

namespace Doera.Infrastructure.Repositories {
    internal class TodoItemRepository(
            ApplicationDbContext _db
        ) : BaseRepository<TodoItem>(_db), ITodoItemRepository { }
}