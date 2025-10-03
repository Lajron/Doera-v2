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
using Microsoft.Extensions.Logging;
using NToastNotify;
using System.Threading;
using System.Threading.Tasks;

namespace Doera.Web.Features.TodoList {
    [Route("[controller]")]
    [Authorize]
    public class TodoListController(
            ITodoListService _todoListService,
            IQueryDispatcher _queryDispatcher,
            IToastNotification _toastNotification,
            ILogger<TodoListController> _logger
        ) : Controller {

        [HttpGet]
        [HttpGet("Search")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> Index(Guid? id, [FromQuery] TodoItemFilter filter, CancellationToken cancellationToken) {
            _logger.LogDebug("GET TodoList Index (RouteId={RouteId}, FilterListId={FilterListId})", id, filter.TodoListId);

            var listId = id ?? filter.TodoListId;
            if (listId is null) {
                _logger.LogDebug("No list id provided, redirecting to Home.");
                return RedirectToAction("Index", "Home");
            }

            var request = new GetTodoListByIdRequest { Id = listId.Value };
            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest, TodoListDto>(request, cancellationToken);

            if (!result.Succeeded) {
                _logger.LogWarning("TodoList not found Id={ListId}", listId);
                return NotFound();
            }

            var effectiveFilter = filter with { TodoListId = listId };
            var vm = new TodoListPageVM(result.Value!, effectiveFilter);

            _logger.LogDebug("Loaded TodoList Id={ListId}", listId);
            return View(vm);
        }

        [HttpGet("Create")]
        public IActionResult Create() {
            _logger.LogDebug("GET Create TodoList");
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateTodoListVM model) {
            if (!ModelState.IsValid) {
                _logger.LogWarning("Create TodoList model validation failed");
                return View(nameof(Create), model);
            }

            var request = model.ToDto();
            var result = await _todoListService.CreateAsync(request);

            if (!result.Succeeded) {
                _logger.LogWarning("Create TodoList failed Errors={Errors}", string.Join("; ", result.Errors.Select(e => e.Code)));
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Create), model);
            }

            _toastNotification.AddSuccessToastMessage("List created.");
            _logger.LogInformation("Created TodoList Id={ListId}", result.Value);
            return RedirectToAction(nameof(Index), new { id = result.Value });
        }

        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken) {
            _logger.LogDebug("GET Edit TodoList Id={ListId}", id);

            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest, TodoListDto>(
                new GetTodoListByIdRequest { Id = id }, cancellationToken);

            if (!result.Succeeded) {
                _logger.LogWarning("Edit TodoList not found Id={ListId}", id);
                return NotFound();
            }

            var vm = new EditTodoListVM {
                Id = result.Value!.Id,
                Name = result.Value.Name
            };

            _logger.LogDebug("Loaded TodoList for edit Id={ListId}", id);
            return View(vm);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] EditTodoListVM model) {
            if (id != model.Id) {
                _logger.LogWarning("Edit TodoList id mismatch RouteId={RouteId} BodyId={BodyId}", id, model.Id);
                return BadRequest();
            }
            if (!ModelState.IsValid) {
                _logger.LogWarning("Edit TodoList validation failed Id={ListId}", id);
                return View(nameof(Edit), model);
            }

            var request = new UpdateTodoListRequest { Id = model.Id, Name = model.Name };
            var result = await _todoListService.UpdateAsync(request);

            if (!result.Succeeded) {
                _logger.LogWarning("Edit TodoList failed Id={ListId} Errors={Errors}", id, string.Join("; ", result.Errors.Select(e => e.Code)));
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Edit), model);
            }

            _toastNotification.AddSuccessToastMessage("List updated.");
            _logger.LogInformation("Updated TodoList Id={ListId}", id);
            return RedirectToAction("Index", "TodoList", new { id = model.Id });
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string? returnUrl) {
            _logger.LogDebug("Delete TodoList requested Id={ListId}", id);

            var result = await _todoListService.DeleteAsync(id);

            if (result.Succeeded) {
                _toastNotification.AddSuccessToastMessage("List deleted.");
                _logger.LogInformation("Deleted TodoList Id={ListId}", id);
            } else {
                _logger.LogWarning("Delete TodoList failed Id={ListId} Errors={Errors}", id, string.Join("; ", result.Errors.Select(e => e.Code)));
                _toastNotification.Warn(result);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                _logger.LogDebug("Redirecting back to ReturnUrl={ReturnUrl}", returnUrl);
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
