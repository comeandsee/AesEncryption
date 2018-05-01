using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSK1
{
    public class RegisterOutput
    {
        public RegisterOutput(bool isInputCorrect, string email, string password, string errorMsg)
        {
            IsInputCorrect = isInputCorrect;
            Email = email;
            Password = password;
            ErrorMessage = errorMsg;
        }

        public bool IsInputCorrect { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
    }
}
