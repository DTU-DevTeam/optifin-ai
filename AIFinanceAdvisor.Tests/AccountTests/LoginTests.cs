using Xunit;
using Moq;
using AIFinanceAdvisor.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AIFinanceAdvisor.UI.Controllers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Net.Http;

namespace AIFinanceAdvisor.Tests.AccountTests
{
    public class LoginTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly AccountController _controller;

        public LoginTests()
        {
            // Initialize mocks
            _mockUserService = new Mock<IUserService>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            // Initialize controller with mocks
            _controller = new AccountController(_mockHttpClientFactory.Object, _mockUserService.Object);
        }

        // Test Case 1: Đăng nhập thành công với thông tin hợp lệ
        [Fact]
        public void Login_Successful_With_Valid_Credentials()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "validUser", Password = "validPassword" };
            _mockUserService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(new User { Username = "validUser" });

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirectToActionResult.ActionName);
            Assert.Equal("OptiFin", redirectToActionResult.ControllerName);
        }

        // Test Case 2: Đăng nhập thất bại với mật khẩu sai
        [Fact]
        public void Login_Fails_With_Invalid_Password()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "validUser", Password = "wrongPassword" };
            _mockUserService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns((User)null!);

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var responseValue = unauthorizedResult.Value;
            Assert.Equal("Invalid credentials", responseValue?.GetType().GetProperty("Message")?.GetValue(responseValue));
        }

        // Test Case 3: Đăng nhập thất bại với tên đăng nhập sai
        [Fact]
        public void Login_Fails_With_Invalid_Username()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "invalidUser", Password = "validPassword" };
            _mockUserService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns((User)null!);

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var responseValue = unauthorizedResult.Value;
            Assert.Equal("Invalid credentials", responseValue?.GetType().GetProperty("Message")?.GetValue(responseValue));
        }

        // Test Case 4: Đăng nhập khi không nhập tên đăng nhập hoặc mật khẩu
        [Fact]
        public void Login_Fails_When_Username_Or_Password_Is_Empty()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "", Password = "" };

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Username and password are required", _controller.ViewData["ErrorMessage"]);
        }
    }
}



