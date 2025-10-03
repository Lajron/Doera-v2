using Doera.Application.DTOs.Tags;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Doera.Web.ViewComponents {
    public class GetTagsViewComponent(
        IQueryDispatcher _queryDispatcher
    ) : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(TagFilter filter) {
            var result = await _queryDispatcher.DispatchAsync<TagFilter, IEnumerable<TagDto>>(filter);

            return View(result.Value);
        }
    }
}
