using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoList;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.DTOs.TodoList.Responses;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Web.Extensions;
using Doera.Web.Features.TodoList.ViewModels;
using Doera.Web.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Threading;
using System.Threading.Tasks;

namespace Doera.Web.Features.TodoList {
    [Route("[controller]")]
    [Authorize]
    public class TodoListController(
            ITodoListService _todoListService,
            IQueryDispatcher _queryDispatcher,
            IToastNotification _toastNotification
        ) : Controller {

        [HttpGet]
        [HttpGet("Search")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> Index(Guid? id, [FromQuery] TodoItemFilter filter, CancellationToken cancellationToken) {
            var listId = id ?? filter.TodoListId;
            if (listId is null) return RedirectToAction("Index", "Home");

            var request = new GetTodoListByIdRequest { Id = listId.Value };

            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest, TodoListDto>(request, cancellationToken);

            if (result.Succeeded is false) return NotFound();

            var effectiveFilter = filter with { TodoListId = listId };

            var vm = new TodoListPageVM(result.Value!, effectiveFilter);

            return View(vm);
        }

        [HttpGet("Create")]
        public IActionResult Create() {
            return View();
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateTodoListVM model) {
            if (!ModelState.IsValid) {
                return View(nameof(Create), model);
            }

            var request = model.ToDto();

            var result = await _todoListService.CreateAsync(request);

            if (!result.Succeeded) {
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Create), model);
            }

            _toastNotification.AddSuccessToastMessage("List created.");
            return RedirectToAction(nameof(Index), new { id = result.Value } );
        }

        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken) {
            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest, TodoListDto>(
                new GetTodoListByIdRequest { Id = id }, cancellationToken);

            if (!result.Succeeded) return NotFound();

            var vm = new EditTodoListVM {
                Id = result.Value!.Id,
                Name = result.Value.Name
            };
            return View(vm);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] EditTodoListVM model) {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(nameof(Edit), model);

            var request = new UpdateTodoListRequest { Id = model.Id, Name = model.Name };
            var result = await _todoListService.UpdateAsync(request);

            if (!result.Succeeded) {
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Edit), model);
            }

            _toastNotification.AddSuccessToastMessage("List updated.");
            return RedirectToAction("Index", "TodoList", new { id = model.Id });
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string? returnUrl) { 
            var result = await _todoListService.DeleteAsync(id);

            if (!result.Succeeded) {
                _toastNotification.Warn(result);
            } else {
                _toastNotification.AddSuccessToastMessage("List deleted.");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
