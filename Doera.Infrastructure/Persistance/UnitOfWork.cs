using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Persistance {
    internal class UnitOfWork(
            ApplicationDbContext _db,
            ITagRepository _tagRepository,
            ITodoItemRepository _todoItemRepository,
            ITodoItemTagRepository _todoItemTagRepository,
            ITodoListRepository _todoListRepository
        ) : IUnitOfWork {

        public ITagRepository Tags => _tagRepository;
        public ITodoItemRepository TodoItems => _todoItemRepository;
        public ITodoItemTagRepository TodoItemTags => _todoItemTagRepository;
        public ITodoListRepository TodoLists => _todoListRepository;

        public async Task<int> CompleteAsync() {
            return await _db.SaveChangesAsync();
        }

        public async Task ExecuteTransactionAsync(Func<Task> Try, Func<Exception, Task>? Catch = null, Func<Task>? Finally = null) {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try {
                await Try();
                await transaction.CommitAsync();
            } catch (Exception ex) {
                await transaction.RollbackAsync();
                if (Catch is not null) await Catch(ex);
                // Should have added throw exception here if catch is null
                // else throw ex;
            } finally {
                if (Finally is not null) await Finally();
            }
        }
    }
}
