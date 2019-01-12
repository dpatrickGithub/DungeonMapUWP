using DungeonMap.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models
{
    // TODO: All of these models need to match 1:1 with the API. In order to do that effectively while maintaining a client-server separation, we need to create a PCL containing all models and share it between client and server. 
    public class GameModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GameType GameType { get; set; }
        public bool IsActive { get; set; }

        public List<CharacterModel> Characters { get; set; }
    }
}
