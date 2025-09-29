using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class TagService(IUnitOfWork uof, IQueryDispatcher queryDispatcher) : ITagService { }
}