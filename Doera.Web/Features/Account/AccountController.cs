using Doera.Application.Interfaces.Identity;
using Doera.Web.Extensions;
using Doera.Web.Features.Account.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Doera.Web.Features.Account {
    [Route("[controller]")]
    public class AccountController(
        IIdentityService _identityService,
        ILogger<AccountController> _logger
    ) : Controller {

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index(string? returnUrl = null) {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null) {
            if (User.Identity?.IsAuthenticated is true) {
                _logger.LogDebug("Register GET while already authenticated. Redirecting to Home.");
                return RedirectToAction("Index", "Home");
            }
            _logger.LogDebug("Register GET (ReturnUrl={ReturnUrl})", returnUrl);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterAccountVM model, string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) {
                _logger.LogWarning("Register POST validation failed for Email={Email}", model.Email);
                return View(nameof(Register), model);
            }

            var result = await _identityService.RegisterAsync(model.Email, model.Password);
            if (!result.Succeeded) {
                _logger.LogWarning("Register POST failed for Email={Email}. Errors={Errors}",
                    model.Email,
                    string.Join("; ", result.Errors.Select(e => e.Code)));
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Register), model);
            }

            _logger.LogInformation("User registered Email={Email} UserId={UserId}", model.Email, result.Value);
            await _identityService.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                _logger.LogDebug("Register redirecting to ReturnUrl={ReturnUrl}", returnUrl);
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null) {
            if (User.Identity?.IsAuthenticated is true) {
                _logger.LogDebug("Login GET while already authenticated. Redirecting to Home.");
                return RedirectToAction("Index", "Home");
            }
            _logger.LogDebug("Login GET (ReturnUrl={ReturnUrl})", returnUrl);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginAccountVM model, string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) {
                _logger.LogWarning("Login POST validation failed for Email={Email}", model.Email);
                return View(nameof(Login), model);
            }

            var result = await _identityService.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded) {
                _logger.LogWarning("Login failed for Email={Email}. Errors={Errors}",
                    model.Email,
                    string.Join("; ", result.Errors.Select(e => e.Code)));
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Login), model);
            }

            _logger.LogInformation("Login succeeded Email={Email} RememberMe={Remember}", model.Email, model.RememberMe);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                _logger.LogDebug("Login redirecting to ReturnUrl={ReturnUrl}", returnUrl);
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            _logger.LogDebug("Logout requested by UserId={UserId}", User?.Identity?.Name);
            await _identityService.SignOutAsync();
            _logger.LogInformation("Logout succeeded");
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied() {
            _logger.LogWarning("AccessDenied view served");
            return View();
        }
    }
}
