using Doera.Web.Features.Error.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Doera.Web.Features.Error {
    [Route("[controller]")]
    public class ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment env) : Controller {

        [HttpGet, HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Handle() {
            var exFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            const int status = StatusCodes.Status500InternalServerError;
            var vm = new ErrorVM(
                StatusCode: status,
                Title: "An unexpected error occurred",
                Detail: "Please try again later.",
                TraceId: HttpContext.TraceIdentifier,
                Path: exFeature?.Path,
                QueryString: HttpContext.Request?.QueryString.ToString(),
                ExceptionMessage: env.IsDevelopment() ? exFeature?.Error.Message : null,
                StackTrace: env.IsDevelopment() ? exFeature?.Error.StackTrace : null
            );

            if (exFeature?.Error is not null)
                logger.LogError(exFeature.Error,
                    "Unhandled exception Path={Path} TraceId={TraceId} UserId={UserId}",
                    exFeature.Path, vm.TraceId, userId
                );

            Response.StatusCode = status;
            return View("Error", vm);
        }

        [HttpGet("{code:int}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusRoute(int code) {
            var reexec = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var title = code switch {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                500 => "Server Error",
                _ => "Error"
            };

            var vm = new ErrorVM(
                StatusCode: code,
                Title: title,
                Detail: null,
                TraceId: HttpContext.TraceIdentifier,
                Path: reexec?.OriginalPath,
                QueryString: reexec?.OriginalQueryString,
                ExceptionMessage: null,
                StackTrace: null
            );

            // Only log here for non-500 (500 already logged in Handle)
            if (code != 500) {
                var level = code switch {
                    401 => LogLevel.Information,
                    404 => LogLevel.Warning,
                    403 => LogLevel.Warning,
                    409 => LogLevel.Warning,
                    400 => LogLevel.Warning,
                    _ => LogLevel.Warning
                };

                if (logger.IsEnabled(level)) {
                    logger.Log(level,
                        "Status error Code={Code} Path={Path} Query={Query} TraceId={TraceId} UserId={UserId}",
                        code, vm.Path, vm.QueryString, vm.TraceId, userId);
                }
            }

            Response.StatusCode = code;
            return View("Error", vm);
        }
    }
}
