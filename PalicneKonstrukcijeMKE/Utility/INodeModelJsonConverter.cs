using Newtonsoft.Json;
using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using PalicneKonstrukcijeMKE.Palicje.Models;
using System;

namespace PalicneKonstrukcijeMKE.Utility
{
    public class INodeModelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(INodeModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(NodeModel));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, typeof(NodeModel));
        }
    }
}
