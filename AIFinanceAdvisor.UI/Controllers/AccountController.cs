using AIFinanceAdvisor.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AIFinanceAdvisor.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IUserService UserService { get; set; }

        public AccountController(IHttpClientFactory httpClientFactory, IUserService userService)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] LoginModel model)
        {
            if (UserService == null)
            {
                throw new NullReferenceException("UserService is not initialized.");
            }

            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewData["ErrorMessage"] = "Username and password are required";
                return View(model);
            }

            var user = UserService.Authenticate(model.Username, model.Password);
            if (user != null)
            {
                // Logic for successful login
                return RedirectToAction("Dashboard", "OptiFin");
            }
            else
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromForm] RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewData["ErrorMessage"] = "Username and password are required";
                return View();
            }

            if (model.Password.Length < 6)
            {
                ViewData["ErrorMessage"] = "Password is too weak";
                return View();
            }

            if (UserService.Register(model.Username, model.Password))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return BadRequest(new { Message = "Username already exists" });
            }
        }
    }
}