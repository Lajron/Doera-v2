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
using System.Threading.Tasks;

namespace Doera.Web.Features.TodoList {
    [Route("[controller]")]
    [Authorize]
    public class TodoListController(
            ITodoListService _todoListService,
            IQueryDispatcher _queryDispatcher
        ) : Controller {

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> Index(Guid id) {
            var request = new GetTodoListByIdRequest { Id = id };

            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest,TodoListDto>(request);

            if (result.Succeeded is false) return NotFound();

            // Note: Map to ViewModel
            // Here we assume a mapping extension method exists

            return View(result.Value);
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

            return RedirectToAction(nameof(Index), new { id = result.Value } );
        }

        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id) {
            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest, TodoListDto>(
                new GetTodoListByIdRequest { Id = id });

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

            return RedirectToAction("Index", "TodoList", new { id = model.Id });
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string? returnUrl) { // removed [FromForm]
            var result = await _todoListService.DeleteAsync(id);

            if (!result.Succeeded) {
                // Toaster notificaiton here
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
