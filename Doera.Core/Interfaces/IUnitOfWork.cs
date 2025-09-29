using Doera.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Interfaces {
    public interface IUnitOfWork {
        IUserRepository Users { get; }
        IUserTagRepository UserTags { get; }
        ITodoItemRepository TodoItems { get; }
        ITodoItemTagRepository TodoItemTags { get; }
        Task<int> CompleteAsync();
        Task ExecuteInTransactionAsync(Func<Task> Try, Func<Exception, Task>? Catch = null, Func<Task>? Finally = null);

    }
}
