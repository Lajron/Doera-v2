using Doera.Application.DTOs.Tags;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Doera.Web.ViewComponents {
    public class GetTagsViewComponent(
        IQueryDispatcher _queryDispatcher
    ) : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(TagFilter? filter, CancellationToken cancellationToken) {
            filter ??= new TagFilter();
            var result = await _queryDispatcher.DispatchAsync<TagFilter, IEnumerable<TagDto>>(filter, cancellationToken);
            return View(result.Value);
        }
    }
}
