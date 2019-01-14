using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models.Auth
{
    [DataContract]
    [Serializable]
    public class TokenModel
    {
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public int UserId { get; set; }
    }
}
