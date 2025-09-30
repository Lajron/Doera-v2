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

            var result = await _queryDispatcher.DispatchAsync<GetTodoListByIdRequest,GetTodoListByIdResponse?>(request);

            if (!result.Succeeded) {
                return NotFound();
            }

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
    }
}
