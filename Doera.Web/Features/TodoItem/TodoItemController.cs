using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Web.Extensions;
using Doera.Web.Features.TodoItem.ViewModels;
using Doera.Web.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Threading;
using System.Threading.Tasks;

namespace Doera.Web.Features.TodoItem {
    [Route("[controller]")]
    [Authorize]
    public class TodoItemController(
            ITodoItemService _todoItemService,
            IQueryDispatcher _queryDispatcher,
            IToastNotification _toastNotification
        ) : Controller {

        [HttpGet("{id:guid}")]
        public IActionResult Index(Guid id) {
            return View();
        }

        [HttpGet("Create")]
        public IActionResult Create(Guid listId) {
            var model = new CreateTodoItemVM { TodoListId = listId };
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateTodoItemVM model) {
            if (!ModelState.IsValid)
                return View(nameof(Create), model);

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
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Create), model);
            }

            _toastNotification.AddSuccessToastMessage("Item created.");
            return RedirectToAction("Index", "TodoList", new { id = model.TodoListId });
        }


        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken) {
            var dtoResult = await _queryDispatcher.DispatchAsync<GetTodoItemByIdRequest, TodoItemDto>(
                new GetTodoItemByIdRequest { Id = id }, cancellationToken);

            if (!dtoResult.Succeeded)
                return NotFound();

            var vm = dtoResult.Value!.ToEditVM();
            return View(vm);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] EditTodoItemVM model) {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
                return View(nameof(Edit), model);

            var request = model.ToUpdateDto();
            var result = await _todoItemService.UpdateAsync(request);
            if (!result.Succeeded) {
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Edit), model);
            }

            _toastNotification.AddSuccessToastMessage("Item updated.");
            return RedirectToAction("Index", "TodoList", new { id = model.TodoListId });
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid listId, Guid itemId) {
            var result = await _todoItemService.DeleteAsync(itemId);

            if (!result.Succeeded) {
                _toastNotification.Error(result);
            } else {
                _toastNotification.AddSuccessToastMessage("Item deleted.");
            }

            return RedirectToAction("Index", "TodoList", new { id = listId });
        }
    }
}
