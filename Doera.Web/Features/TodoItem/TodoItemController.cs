using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Web.Extensions;
using Doera.Web.Features.TodoItem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Doera.Web.Features.TodoItem {
    [Route("[controller]")]
    [Authorize]
    public class TodoItemController(
            ITodoItemService _todoItemService,
            IQueryDispatcher _queryDispatcher
        ) : Controller {

        [HttpGet("{id:guid}")]
        public IActionResult Index(Guid id) => View();

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

            return RedirectToAction("Index", "TodoList", new { id = model.TodoListId });
        }
    }
}
