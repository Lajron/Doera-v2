using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class AdminService(IUnitOfWork uof, IQueryDispatcher queryDispatcher) : IAdminService { }
}