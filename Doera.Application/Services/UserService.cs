using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class UserService(IUnitOfWork uof, IQueryDispatcher queryDispatcher) : IUserService { }
}