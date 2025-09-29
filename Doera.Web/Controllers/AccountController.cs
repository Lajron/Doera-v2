using Doera.Application.Interfaces.Identity;
using Doera.Web.Extensions;
using Doera.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doera.Web.Controllers {
    [Route("[controller]")]
    public class AccountController(IIdentityService _identityService) : Controller {

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index(string? returnUrl = null) {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterAccountVM model, string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) {
                return View(nameof(Register), model);
            }

            var result = await _identityService.RegisterAsync(model.Email, model.Password);
            if (!result.Succeeded) {
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Register), model);
            }

            await _identityService.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginAccountVM model, string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) {
                return View(nameof(Login), model);
            }

            var result = await _identityService.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded) {
                ModelState.AddResultErrors(result.Errors);
                return View(nameof(Login), model);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _identityService.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied() {
            return View();
        }
    }
}
