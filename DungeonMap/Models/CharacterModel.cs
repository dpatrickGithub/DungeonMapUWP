using DungeonMap.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models
{
    public class CharacterModel
    {
        public int Id { get; set; }
        public RoleType RoleType { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public int UserId { get; set; }
        public string CharacterName { get; set; }
    }
}
