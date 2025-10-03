using System.Diagnostics;
using Doera.Application.DTOs.TodoItem;
using Doera.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Doera.Web.Features.Home
{
    [Authorize]
    [Route("[controller]")]
    public class HomeController(ILogger<HomeController> _logger) : Controller
    {
        [HttpGet("/")]
        [HttpGet("")]
        [HttpGet("Search")]
        public IActionResult Index(TodoItemFilter filter)
        {
            ViewData["Filter"] = filter;
            return View(filter);
        }

        [HttpGet("Privacy")]
        public IActionResult Privacy() => View();

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
