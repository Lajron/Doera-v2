using Doera.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Doera.Core.Interfaces.Repositories {
    public interface ITodoListRepository : IRepository<TodoList> {
        Task<int> GetCountForUserAsync(Guid userId, IEnumerable<Guid>? todoListIds = null);
        Task<IEnumerable<TodoList>> GetAllForUserAsync(Guid userId, IEnumerable<Guid>? todoListIds = null);
    }
}