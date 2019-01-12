using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Validators
{
    public class LoginValidator
    {
        public bool ValidateUserName(string userName)
        {
            return userName.Length >= 5 && !String.IsNullOrWhiteSpace(userName); 
        }

        public bool ValidatePassword(string password)
        {
            return password.Length >= 8 && !String.IsNullOrWhiteSpace(password); 
        }
    }
}
