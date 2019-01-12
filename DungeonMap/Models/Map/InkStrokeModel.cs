using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Models
{
    public class InkStrokeModel
    {
        public uint Id { get; set; }
        public ICollection<InkPointModel> InkPoints { get; set; } = new List<InkPointModel>();
        public ColorModel Color { get; set; }
    }
}
