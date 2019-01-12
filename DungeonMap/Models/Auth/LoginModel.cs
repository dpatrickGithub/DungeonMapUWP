using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models.Auth
{
    public class LoginModel
    {
        [DisplayName("username")]
        public string UserName { get; set; }

        [DisplayName("password")]
        public string Password { get; set; }

        [DisplayName("grant_type")]
        public string GrantType { get; set; } = "password";
    }
}
