using Doera.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Interfaces {
    public interface IUnitOfWork {
        ITagRepository Tags { get; }
        ITodoItemRepository TodoItems { get; }
        ITodoItemTagRepository TodoItemTags { get; }
        ITodoListRepository TodoLists { get; }
        Task<int> CompleteAsync();
        Task ExecuteTransactionAsync(Func<Task> Try, Func<Exception, Task>? Catch = null, Func<Task>? Finally = null);

    }
}
