using Doera.Core.Entities;
using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doera.Core.Interfaces.Repositories {
    public interface ITodoItemRepository : IRepository<TodoItem> {
        Task<int> ExecuteUpdateStatusAsync(Guid todoItemId, Guid userId, TodoStatus status);
        Task<IEnumerable<TodoItem>> GetAllForUserAsync(Guid userId, Guid? todoListId = null, IEnumerable<Guid>? requestIds = null);
        Task<int> GetCountForListAsync(Guid listId);
        Task<int> ExecuteDeleteByListAsync(Guid todoListId);
    }
}