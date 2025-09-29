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
            IUserRepository _userRepository,
            IUserTagRepository _userTagRepository
        ) : IUnitOfWork {

        public ITagRepository Tags => _tagRepository;
        public ITodoItemRepository TodoItems => _todoItemRepository;
        public ITodoItemTagRepository TodoItemTags => _todoItemTagRepository;
        public IUserRepository Users => _userRepository;
        public IUserTagRepository UserTags => _userTagRepository;

        public async Task<int> CompleteAsync() {
            return await _db.SaveChangesAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> Try, Func<Exception, Task>? Catch = null, Func<Task>? Finally = null) {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try {
                await Try();
                await transaction.CommitAsync();
            } catch (Exception ex) {
                await transaction.RollbackAsync();
                if (Catch is not null)
                    await Catch(ex);
                throw;
            } finally {
                if (Finally is not null)
                    await Finally();
            }
        }
    }
}
