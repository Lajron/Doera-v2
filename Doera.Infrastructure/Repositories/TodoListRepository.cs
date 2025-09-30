using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Repositories {
    internal class TodoListRepository(ApplicationDbContext _db)
        : BaseRepository<TodoList>(_db), ITodoListRepository {

        public async Task<int> GetCountForUserAsync(Guid userId) {
            return await _dbSet.CountAsync(l => l.UserId == userId);
        }
    }
}