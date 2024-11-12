using Newtonsoft.Json;
using System;

namespace Glitch9.IO.RESTApi
{
    [JsonConverter(typeof(RESTRequestGenericConverter<>))]
    public class RESTRequest<T> : RESTRequest where T : new()
    {
        public T Data
        {
            get
            {
                _data ??= new T();
                return _data;
            }
            set => _data = value;
        }
        
        private T _data;
    }

    public class RESTRequestGenericConverter<T> : JsonConverter<T> where T : new()
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            // T Data가 RESTRequest의 property가 아니라 RESTRequest 자체인 것으로 가정하여 직렬화
            RESTRequest<T> request = new() { Data = value };
            serializer.Serialize(writer, request);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // T Data가 RESTRequest의 property가 아니라 RESTRequest 자체인 것으로 가정하여 역직렬화
            RESTRequest<T> request = serializer.Deserialize<RESTRequest<T>>(reader);
            return request.Data;
        }
    }
}