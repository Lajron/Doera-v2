using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Repositories {
    internal class TodoListRepository(ApplicationDbContext _db)
        : BaseRepository<TodoList>(_db), ITodoListRepository {
        public async Task<IEnumerable<TodoList>> GetAllForUserAsync(Guid userId, IEnumerable<Guid>? todoListIds = null) {
            var query = _dbSet.AsQueryable();

            query = query.Where(l => l.UserId == userId);

            if (todoListIds is not null && todoListIds.Any())
                query = query.Where(l => todoListIds.Contains(l.Id));

            return await query.ToListAsync();
        }

        public async Task<int> GetCountForUserAsync(Guid userId, IEnumerable<Guid>? todoListIds = null) {
            var query = _dbSet.AsQueryable();

            query = query.Where(l => l.UserId == userId);

            if (todoListIds is not null && todoListIds.Any())
                query = query.Where(l => todoListIds.Contains(l.Id));

            return await query.CountAsync();
        }

    }
}