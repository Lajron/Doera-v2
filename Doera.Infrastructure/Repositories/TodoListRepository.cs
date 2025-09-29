using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;

namespace Doera.Infrastructure.Repositories {
    internal class TodoListRepository(ApplicationDbContext _db)
        : BaseRepository<TodoList>(_db), ITodoListRepository { }
}