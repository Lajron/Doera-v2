using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace Doera.Web.ViewComponents {
    public class GetTodoListsViewComponent(
        IQueryDispatcher _queryDispatcher
    ) : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken) {
            var result = await _queryDispatcher.DispatchAsync<GetTodoListsRequest, IEnumerable<TodoListDto>>(new GetTodoListsRequest(), cancellationToken);
            return View(result.Value);
        }
    }
}
