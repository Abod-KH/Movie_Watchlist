
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
 
using System.Security.Claims;


namespace Movie_Watchlist.Presintation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                result = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
            }

            if (!result.Succeeded || result.Principal == null)
            {
                TempData["ErrorMessage"] = "Google authentication failed.";
                return RedirectToAction(nameof(Login));
            }

            var principal = result.Principal;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue(ClaimTypes.Name) ?? email?.Split('@')[0] ?? "User";

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Could not retrieve email from Google account.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _accountService.GetOrCreateExternalUserAsync(email, name);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Failed to create or find user account.";
                return RedirectToAction(nameof(Login));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin login)
        {
            if (!ModelState.IsValid) return View(login);

            var user = await _accountService.ValidateUserAsync(login);

            if (user != null)
            {
                
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName ?? "User") 
            };

                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(login);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegister register)
        {
            if (!ModelState.IsValid) return View(register);

            var success = await _accountService.RegisterUserAsync(register);
            if (success)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Email is already taken or registration failed.");
            return View(register);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
