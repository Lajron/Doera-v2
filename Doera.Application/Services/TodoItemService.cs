using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class TodoItemService(IUnitOfWork uof, IQueryDispatcher queryDispatcher) : ITodoItemService { }
}