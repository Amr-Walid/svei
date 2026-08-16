using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SVEI.Web.Services;

namespace SVEI.Web.Controllers
{
    /// <summary>Sign-in / sign-out for the admin panel (no public registration).</summary>
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;
        private readonly ISettings _cfg;

        public AccountController(SignInManager<IdentityUser> signIn,
                                 UserManager<IdentityUser> users,
                                 ISettings cfg)
        {
            _signIn = signIn; _users = users; _cfg = cfg;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return LocalRedirect(SafeReturn(returnUrl));

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        [EnableRateLimiting(RateLimiting.Login)]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false,
                                               string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = email;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "من فضلك أدخل البريد الإلكتروني وكلمة المرور.");
                return View();
            }

            var user = await _users.FindByEmailAsync(email.Trim());

            // A missing account used to return immediately, while a real account
            // went on to a password hash verification. That timing difference is
            // a username oracle: it tells an attacker which addresses exist so a
            // password spray can be aimed only at those. Burn a comparable amount
            // of work on the miss so both paths look the same from outside.
            if (user is null)
            {
                await Task.Delay(Random.Shared.Next(180, 320));
                ModelState.AddModelError("", "بيانات الدخول غير صحيحة.");
                return View();
            }

            var result = await _signIn.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
                return LocalRedirect(SafeReturn(returnUrl));

            if (result.IsLockedOut)
                ModelState.AddModelError("", "تم قفل الحساب مؤقتاً بسبب محاولات دخول متكررة.");
            else
                ModelState.AddModelError("", "بيانات الدخول غير صحيحة.");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Denied() => View();

        private string SafeReturn(string? returnUrl) =>
            !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/Admin";
    }
}
