using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Doera.Web.ViewComponents {
    public class GetTodoItemsViewComponent(
        IQueryDispatcher _queryDispatcher
    ) : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(TodoItemFilter? filter, CancellationToken cancellationToken) {
            filter ??= new TodoItemFilter();
            var result = await _queryDispatcher.DispatchAsync<TodoItemFilter, IEnumerable<TodoItemDto>>(filter, cancellationToken);
            return View(result.Value);
        }
    }
}
