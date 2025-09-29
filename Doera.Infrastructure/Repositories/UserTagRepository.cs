using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;

namespace Doera.Infrastructure.Repositories {
    internal class UserTagRepository(
            ApplicationDbContext _db
        ) : BaseRepository<UserTag>(_db), IUserTagRepository { }
}