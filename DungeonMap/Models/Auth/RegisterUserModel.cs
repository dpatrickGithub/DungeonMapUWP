using DungeonMap.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models.Auth
{
    public class RegisterUserModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string PreferredCharacterName { get; set; }
        public string PhoneNumber { get; set; }
        public string Biography { get; set; }
        public string SkypeHandle { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
    }
}
