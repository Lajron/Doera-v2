using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Doera.Web.ViewComponents {
    public class GetTodoListsViewComponent(
        IQueryDispatcher _queryDispatcher
    ) : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync() {
            var result = await _queryDispatcher.DispatchAsync<GetTodoListsRequest, IEnumerable<TodoListDto>>(new GetTodoListsRequest());

            return View(result.Value);
        }
    }
}
