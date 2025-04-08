using AIFinanceAdvisor.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Services
{
    public class UserService : IUserService
    {
        private static readonly List<User> Users = new List<User>(); // Giả lập cơ sở dữ liệu người dùng

        // Phương thức Authenticate để xác thực người dùng
        public User Authentic(string username, string password)
        {
            // Giả sử bạn có một danh sách người dùng được lưu trữ tạm thời
            var users = new List<User>
        {
            new User { Id = 1, Username = "admin", Password = "123456" },
            new User { Id = 2, Username = "user", Password = "password" }
        };

            // Kiểm tra thông tin đăng nhập
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public User Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }

        // Phương thức Register để đăng ký người dùng mới
        public bool Register(string username, string password)
        {
            if (Users.Any(u => u.Username == username))
                return false; // Nếu tên đăng nhập đã tồn tại, trả về false

            Users.Add(new User { Username = username, Password = password });
            return true; // Đăng ký thành công
        }
    }
}

