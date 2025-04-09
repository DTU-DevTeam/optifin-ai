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

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var _httpClient = _httpClientFactory.CreateClient();
            string flaskApiUrl = "http://127.0.0.1:5000/login";

            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(flaskApiUrl, jsonContent);
            string responseBody = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<FlaskAPI>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data.message.Equals("Đăng nhập thành công"))
            {
                // 1. Tạo danh sách Claims
                var claims = new List<Claim>
                {
                   new Claim(ClaimTypes.Name, model.Username), // Lưu username vào Claim
                   new Claim(ClaimTypes.NameIdentifier, data.id.ToString()),
                   new Claim("UserRole", "User") // Thêm role nếu cần
                };



                // 2. Tạo ClaimsIdentity
                var identity = new ClaimsIdentity(claims, "CookieAuth");

                // 3. Tạo Principal & đăng nhập bằng Cookie
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("CookieAuth", principal);

                // 4. Điều hướng sau khi đăng nhập thành công

                return RedirectToAction("Dashboard", "OptiFin");
            }
            else
            {
                ViewBag.ErrorMessage = data.message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
               .SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage)
               .ToList();

                ViewBag.ErrorMessage = string.Join("; ", errorMessages);
                return View();
                
            }

            var _httpClient = _httpClientFactory.CreateClient();
            string flaskApiUrl = "http://127.0.0.1:5000/register";

            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(flaskApiUrl, jsonContent);
            string responseBody = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<FlaskAPI>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data.message.Equals("Đăng ký thành công"))
            {

                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ErrorMessage = data.message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Dashboard", "OptiFin");
        }
    }
}
