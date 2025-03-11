using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Services
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
