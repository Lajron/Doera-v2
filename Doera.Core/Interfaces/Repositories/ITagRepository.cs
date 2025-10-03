using Doera.Core.Entities;

namespace Doera.Core.Interfaces.Repositories {
    public interface ITagRepository : IRepository<Tag> {
        Task ExecuteDeleteUnusedTagsAsync();
        Task<ICollection<TodoItemTag>> ResolveTagsAsync(IEnumerable<string> tagNames);
    }
}