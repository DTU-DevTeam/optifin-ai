using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password); // Phương thức đăng nhập
        bool Register(string username, string password); // Phương thức đăng ký
    }
}
