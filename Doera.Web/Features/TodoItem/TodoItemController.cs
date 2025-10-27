using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Web.Extensions;
using Doera.Web.Features.TodoItem.ViewModels;
using Doera.Web.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System.Threading;
using System.Threading.Tasks;

namespace Doera.Web.Features.TodoItem {
    [Route("[controller]")]
    [Authorize]
    public class TodoItemController(
            ITodoItemService _todoItemService,
            IQueryDispatcher _queryDispatcher,
            IToastNotification _toastNotification,
            ILogger<TodoItemController> _logger
        ) : Controller {

        [HttpGet("{id:guid}")]
        public IActionResult Index(Guid id) {
            _logger.LogDebug("TodoItem Index placeholder Id={ItemId}", id);
            return View();
        }

        [HttpGet("Create")]
        public IActionResult Create(Guid listId) {
            _logger.LogDebug("GET Create TodoItem ListId={ListId}", listId);
            var model = new CreateTodoItemVM { TodoListId = listId };
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateTodoItemVM model) {
            if (!ModelState.IsValid) {
                _logger.LogWarning("Create TodoItem validation failed ListId={ListId}", model.TodoListId);
                return View(nameof(Create), model);
            }

            var request = new CreateTodoItemRequest {
                TodoListId = model.TodoListId,
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Priority = model.Priority,
                StartDate = model.StartDate,
                DueDate = model.DueDate,
                TagNames = model.TagNames?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? []
            };

            var result = await _todoItemService.CreateAsync(request);

            if (!result.Succeeded) {
                _logger.LogWarning("Create TodoItem failed (service errors) ListId={ListId} Title={Title} Errors={Errors}",
                    model.TodoListId,
                    model.Title,
                    string.Join("; ", result.Errors.Select(e => e.Code)));
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Create), model);
            }

            _logger.LogInformation("Created TodoItem Id={ItemId} ListId={ListId}", result.Value, model.TodoListId);
            _toastNotification.AddSuccessToastMessage("Item created.");
            return RedirectToAction("Index", "TodoList", new { id = model.TodoListId });
        }

        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken) {
            _logger.LogDebug("GET Edit TodoItem Id={ItemId}", id);

            var dtoResult = await _queryDispatcher.DispatchAsync<GetTodoItemByIdRequest, TodoItemDto>(
                new GetTodoItemByIdRequest { Id = id }, cancellationToken);

            if (!dtoResult.Succeeded) {
                _logger.LogWarning("Edit TodoItem not found Id={ItemId}", id);
                return NotFound();
            }

            var vm = dtoResult.Value!.ToEditVM();
            _logger.LogDebug("Loaded TodoItem for edit Id={ItemId} ListId={ListId}", vm.Id, vm.TodoListId);
            return View(vm);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] EditTodoItemVM model) {
            if (id != model.Id) {
                _logger.LogWarning("Edit TodoItem bad route/body mismatch RouteId={RouteId} BodyId={BodyId}", id, model.Id);
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                _logger.LogWarning("Edit TodoItem validation failed Id={ItemId}", id);
                return View(nameof(Edit), model);
            }

            var request = model.ToUpdateDto();
            var result = await _todoItemService.UpdateAsync(request);
            if (!result.Succeeded) {
                _logger.LogWarning("Edit TodoItem failed (service errors) Id={ItemId} Errors={Errors}",
                    id,
                    string.Join("; ", result.Errors.Select(e => e.Code)));
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Edit), model);
            }

            _logger.LogInformation("Updated TodoItem Id={ItemId} ListId={ListId}", id, model.TodoListId);
            _toastNotification.AddSuccessToastMessage("Item updated.");
            return RedirectToAction("Index", "TodoList", new { id = model.TodoListId });
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid listId, Guid itemId) {
            _logger.LogDebug("Delete TodoItem requested Id={ItemId} ListId={ListId}", itemId, listId);

            var result = await _todoItemService.DeleteAsync(itemId);

            if (!result.Succeeded) {
                _logger.LogWarning("Delete TodoItem failed Id={ItemId} ListId={ListId} Errors={Errors}",
                    itemId,
                    listId,
                    string.Join("; ", result.Errors.Select(e => e.Code)));
                _toastNotification.Error(result);
            } else {
                _logger.LogInformation("Deleted TodoItem Id={ItemId} ListId={ListId}", itemId, listId);
                _toastNotification.AddSuccessToastMessage("Item deleted.");
            }

            return RedirectToAction("Index", "TodoList", new { id = listId });
        }
    }
}
