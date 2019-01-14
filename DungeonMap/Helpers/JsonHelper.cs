using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMap.Helpers
{
    public class JsonHelper<T> where T : new()
    {
        public string ConvertToJson(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            return json;
        }

        public T ConvertToModel(string json)
        {
            var model = JsonConvert.DeserializeObject<T>(json); 

            return model;
        }
    }
}
