using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using AIFinanceAdvisor.Core.Services;
using AIFinanceAdvisor.UI.Controllers;
using System.Net.Http;

namespace AIFinanceAdvisor.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            // Khởi tạo mock cho IUserService
            _mockUserService = new Mock<IUserService>();

            // Khởi tạo mock cho IHttpClientFactory
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            // Khởi tạo controller với mock IHttpClientFactory và mock IUserService
            _controller = new AccountController(_mockHttpClientFactory.Object, _mockUserService.Object);
        }

        // Test Login thành công
        [Fact]
        public void Login_TrảVềRedirectToActionResult_KhiThôngTinĐăngNhậpHợpLệ()
        {
            var username = "validUser";
            var password = "validPassword";
            var user = new User { Username = username, Password = password };
            var loginModel = new LoginModel { Username = username, Password = password };

            // Mock Authenticate để trả về một user hợp lệ
            _mockUserService.Setup(service => service.Authenticate(username, password)).Returns(user);

            var result = _controller.Login(loginModel);

            // Kiểm tra kết quả trả về là RedirectToActionResult
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirectResult.ActionName); // Kiểm tra có chuyển hướng tới Dashboard
        }

        // Test Login thất bại
        [Fact]
        public void Login_TrảVềUnauthorizedObjectResult_KhiThôngTinĐăngNhậpKhôngHợpLệ()
        {
            var username = "validUser";
            var password = "wrongPassword";
            var loginModel = new LoginModel { Username = username, Password = password };

            // Mock Authenticate trả về null (không tìm thấy user)
            _mockUserService.Setup(service => service.Authenticate(username, password)).Returns((User)null!);

            var result = _controller.Login(loginModel);

            // Kiểm tra kết quả trả về là UnauthorizedObjectResult
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            // Kiểm tra giá trị trả về
            var responseValue = unauthorizedResult.Value;
            Assert.Equal("Invalid credentials", responseValue?.GetType().GetProperty("Message")?.GetValue(responseValue));
        }


        // Test Register thành công
        [Fact]
        public void Register_TrảVềRedirectToActionResult_KhiĐăngKýThànhCông()
        {
            var username = "newUser";
            var password = "validPassword";
            var registerModel = new RegisterModel { Username = username, Password = password };

            // Mock Register trả về true (đăng ký thành công)
            _mockUserService.Setup(service => service.Register(username, password)).Returns(true);

            var result = _controller.Register(registerModel);

            // Kiểm tra kết quả trả về là RedirectToActionResult
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName); // Kiểm tra có chuyển hướng tới Login action
        }

        // Test Register thất bại
        [Fact]
        public void Register_TrảVềBadRequestObjectResult_KhiTênNgườiDùngĐãTồnTại()
        {
            var username = "existingUser";
            var password = "validPassword";
            var registerModel = new RegisterModel { Username = username, Password = password };

            // Mock Register trả về false (tên người dùng đã tồn tại)
            _mockUserService.Setup(service => service.Register(username, password)).Returns(false);

            var result = _controller.Register(registerModel);

            // Kiểm tra kết quả trả về là BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseValue = badRequestResult.Value;
            Assert.Equal("Username already exists", responseValue?.GetType().GetProperty("Message")?.GetValue(responseValue));
        }





    }
}
