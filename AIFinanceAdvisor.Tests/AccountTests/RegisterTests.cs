using Xunit;
using Moq;
using AIFinanceAdvisor.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AIFinanceAdvisor.UI.Controllers;
using System.Net.Http;

namespace AIFinanceAdvisor.Tests.AccountTests
{
    public class RegisterTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly AccountController _controller;

        public RegisterTests()
        {
            // Initialize mocks
            _mockUserService = new Mock<IUserService>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            // Initialize controller with mocks
            _controller = new AccountController(_mockHttpClientFactory.Object, _mockUserService.Object);
        }

        // Test Case 1: Đăng ký thành công với thông tin hợp lệ
        [Fact]
        public void Register_Successful_With_Valid_Credentials()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "newUser", Password = "validPassword" };
            _mockUserService.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = _controller.Register(registerModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
        }

        // Test Case 2: Đăng ký thất bại khi tên đăng nhập đã tồn tại
        [Fact]
        public void Register_Fails_When_Username_Exists()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "existingUser", Password = "validPassword" };
            _mockUserService.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = _controller.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseValue = badRequestResult.Value;
            Assert.Equal("Username already exists", responseValue?.GetType().GetProperty("Message")?.GetValue(responseValue));
        }

        // Test Case 3: Đăng ký thất bại khi mật khẩu không đủ mạnh
        [Fact]
        public void Register_Fails_When_Password_Too_Weak()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "newUser", Password = "12345" };

            // Act
            var result = _controller.Register(registerModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Password is too weak", _controller.ViewData["ErrorMessage"]);
        }

        // Test Case 4: Đăng ký thất bại khi thiếu tên đăng nhập hoặc mật khẩu
        [Fact]
        public void Register_Fails_When_Username_Or_Password_Is_Empty()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "", Password = "" };

            // Act
            var result = _controller.Register(registerModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Username and password are required", _controller.ViewData["ErrorMessage"]);
        }
    }
}



