using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models.Auth
{
    public class TokenModel
    {
        [DisplayName("access_token")]
        public string Token { get; set; }

        [DisplayName("user_id")]
        public int UserId { get; set; }
    }
}
