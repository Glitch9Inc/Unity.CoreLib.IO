using Newtonsoft.Json;
using System;

namespace Glitch9.IO.RESTApi
{
    [JsonConverter(typeof(RESTResponseGenericConverter<>))]
    public class RESTResponse<T> : RESTResponse
    {
        public T Data { get; set; }
    }

    public class RESTResponseGenericConverter<T> : JsonConverter<T>
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            // T Data가 RESTResponse의 property가 아니라 RESTResponse 자체인 것으로 가정하여 직렬화
            RESTResponse<T> response = new() { Data = value };
            serializer.Serialize(writer, response);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // T Data가 RESTResponse의 property가 아니라 RESTResponse 자체인 것으로 가정하여 역직렬화
            RESTResponse<T> response = serializer.Deserialize<RESTResponse<T>>(reader);
            return response.Data;
        }
    }
}