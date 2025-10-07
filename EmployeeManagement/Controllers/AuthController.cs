using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Data;
using EmployeeManagement.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;

        public AuthController(IUserRepository repo, IJwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _repo.GetUserByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("", "Email already exists");
                return View(model);
            }

            if (await _repo.GetUserByUserNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                Role = "User"
            };

            await _repo.AddUserAsync(user);

            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _repo.GetUserByUserNameAsync(model.UserName);
            if (user == null || user.PasswordHash != HashPassword(model.Password))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            // Sign-in with cookie authentication so MVC pages work
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect to Employee index (protected)
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Token()
        {
            // Only logged in users can request token
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();

            var username = User.Identity!.Name!;
            var user = await _repo.GetUserByUserNameAsync(username);
            if (user == null) return Forbid();

            var token = _jwtService.GenerateToken(user);
            return View(model: token);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
