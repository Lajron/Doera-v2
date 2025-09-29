using Doera.Core.Entities;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;

namespace Doera.Infrastructure.Repositories {
    internal class UserRepository(
            ApplicationDbContext _db
        ) : BaseRepository<User>(_db), IUserRepository { }
}