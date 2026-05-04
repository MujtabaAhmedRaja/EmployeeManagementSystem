using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for managing user accounts, including login, signup, and logout.
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Temporary in-memory user storage.
        /// </summary>
        private static readonly Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase)
        {
            { "admin", "Admin@123" }
        };

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to after successful login.</param>
        /// <returns>The login view.</returns>
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Processes the login request.
        /// </summary>
        /// <param name="username">The username provided by the user.</param>
        /// <param name="password">The password provided by the user.</param>
        /// <param name="returnUrl">The URL to redirect to after successful login.</param>
        /// <returns>A redirect to the return URL or the login view with errors.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            if (!_users.TryGetValue(username, out var storedPassword) || storedPassword != password)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var role = username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Employee";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(returnUrl);
        }

        /// <summary>
        /// Displays the signup page.
        /// </summary>
        /// <returns>The signup view.</returns>
        [AllowAnonymous]
        public IActionResult Signup()
        {
            return View();
        }

        /// <summary>
        /// Processes the signup request.
        /// </summary>
        /// <param name="fullName">The full name of the user.</param>
        /// <param name="username">The desired username.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The desired password.</param>
        /// <returns>A redirect to the home page or the signup view with errors.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(string fullName, string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError(string.Empty, "Full name, username and password are required.");
                return View();
            }

            if (_users.ContainsKey(username))
            {
                ModelState.AddModelError(string.Empty, "This username is already taken.");
                return View();
            }

            _users[username] = password;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Role, username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Employee")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Processes the logout request.
        /// </summary>
        /// <returns>A redirect to the login page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Provides a GET endpoint for logout (convenience).
        /// </summary>
        /// <returns>A redirect to the login page.</returns>
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
    }
}
