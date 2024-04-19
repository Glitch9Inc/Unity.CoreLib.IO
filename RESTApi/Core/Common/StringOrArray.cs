using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;

namespace Glitch9.IO.RESTApi
{
    public readonly struct StringOrArray<T>
    {
        public static implicit operator StringOrArray<T>(string stringValue) => new(stringValue);
        public static implicit operator StringOrArray<T>(T[] arrayValue) => new(arrayValue);

        private readonly object _value;

        public StringOrArray(string stringValue)
        {
            _value = stringValue;
        }

        public StringOrArray(IEnumerable input)
        {
            _value = input;
        }

        public bool IsString => _value is string;
        public bool IsArray => _value is T[];
        public int Length => IsArray ? (_value as T[])?.Length ?? 0 : 0;

        public void CopyTo(T[] array, int index)
        {
            if (IsArray)
            {
                (_value as T[])?.CopyTo(array, index);
            }
        }

        public override string ToString()
        {
            if (IsString)
            {
                return _value as string;
            }
            else if (IsArray)
            {
                return string.Join(", ", ((_value as T[]) ?? Array.Empty<T>()).Select(x => x.ToString()));
            }

            return base.ToString();
        }

        public T[] ToArray()
        {
            return IsArray ? _value as T[] : Array.Empty<T>();
        }
    }

    public class StringOrArrayConverter<T> : JsonConverter<StringOrArray<T>>
    {
        public override void WriteJson(JsonWriter writer, StringOrArray<T> value, JsonSerializer serializer)
        {
            // Handle the serialization of the StringOrArray
            if (value.IsString)
            {
                serializer.Serialize(writer, value.ToString());
            }
            else if (value.IsArray)
            {
                serializer.Serialize(writer, value.ToArray());
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override StringOrArray<T> ReadJson(JsonReader reader, Type objectType, StringOrArray<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Read JSON and convert to StringOrArray accordingly
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return new StringOrArray<T>(token.ToObject<string>());
            }
            else if (token.Type == JTokenType.Array)
            {
                T[] array = token.ToObject<T[]>();
                return new StringOrArray<T>(array);
            }
            else
            {
                throw new JsonSerializationException("Unexpected token type when parsing StringOrArray.");
            }
        }
    }
}