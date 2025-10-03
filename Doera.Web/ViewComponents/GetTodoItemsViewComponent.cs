using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Doera.Web.ViewComponents {
    public class GetTodoItemsViewComponent(
        IQueryDispatcher _queryDispatcher
    ) : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(TodoItemFilter filter) {
            var result = await _queryDispatcher.DispatchAsync<TodoItemFilter, IEnumerable<TodoItemDto>>(filter);

            return View(result.Value);
        }
    }
}
