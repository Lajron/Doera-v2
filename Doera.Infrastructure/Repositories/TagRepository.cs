using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;

namespace Doera.Infrastructure.Repositories {
    internal class TagRepository(
            ApplicationDbContext _db
        ) : BaseRepository<Tag>(_db), ITagRepository { }
}