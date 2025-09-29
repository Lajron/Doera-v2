using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;

namespace Doera.Infrastructure.Repositories {
    internal class TodoItemTagRepository(
            ApplicationDbContext _db
        ) : BaseRepository<TodoItemTag>(_db), ITodoItemTagRepository { }
}