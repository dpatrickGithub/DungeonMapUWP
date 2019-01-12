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
            var stream = new MemoryStream();

            var serializer = new DataContractJsonSerializer(typeof(T));

            serializer.WriteObject(stream, model);

            byte[] json = stream.ToArray();

            stream.Close();

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public T ConvertToModel(string json)
        {
            var model = new T();

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var serializer = new DataContractJsonSerializer(typeof(T));
            
            model = (T)serializer.ReadObject(stream);

            stream.Close();

            return model;
        }
    }
}
