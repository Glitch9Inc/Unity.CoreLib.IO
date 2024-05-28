using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// This object can be either a string, T, or an array of T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringOr<T>
    {
        public static implicit operator StringOr<T>(string stringValue) => new(stringValue);
        public static implicit operator StringOr<T>(T objectValue) => new(objectValue);
        public static implicit operator StringOr<T>(T[] arrayValue) => new(arrayValue);
        public object Value;

        public StringOr(bool isArray = false)
        {
            IsArray = isArray;
        }
  
        public StringOr(string stringValue)
        {
            Value = stringValue;
            IsArray = false;
        }

        public StringOr(T objectValue)
        {
            Value = objectValue;
            IsArray = false;
        }

        public StringOr(IEnumerable arrayValue)
        {
            Value = arrayValue;
            IsArray = true;
        }

        public bool HasValue => Value != null;
        public bool IsString => typeof(T) == typeof(string);
        public bool IsObject => typeof(T) != typeof(string) && !IsArray;
        public bool IsArray { get; private set; }

        /// <summary>
        /// Returns the length of the array if the value is an array,
        /// returns the length of the string if the value is a string,
        /// otherwise returns 0.
        /// </summary>
        public int Length => Value switch
        {
            string s => s.Length,
            IEnumerable enumerable => enumerable.Cast<object>().Count(),
            _ => 0
        };

        public override string ToString()
        {
            if (IsString) return Value as string;
            if (IsArray) return string.Join(", ", ((Value as T[]) ?? Array.Empty<T>()).Select(x => x.ToString()));
            
            return base.ToString();
        }

        public T[] ToArray()
        {
            //if (!IsArray)
            //{
            //    Debug.LogError("Cannot convert a non-array StringOr to an array.");
            //    return null;
            //}

            return Value as T[];
        }

        public void Add(T value)
        {
            if (!IsArray)
            {
                Debug.LogError("Cannot add to a non-array StringOr.");
                return;
            }

            Value = (Value as T[] ?? Array.Empty<T>()).Append(value).ToArray();
        }
    }

    public class StringOrConverter<T> : JsonConverter<StringOr<T>>
    {
        public override void WriteJson(JsonWriter writer, StringOr<T> value, JsonSerializer serializer)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var localSerializer = JsonSerializer.Create(settings);

            // Handle the serialization of the StringOr
            if (value.IsString)
            {
                localSerializer.Serialize(writer, value.ToString());
            }
            else if (value.IsArray)
            {
                localSerializer.Serialize(writer, value.ToArray());
            }
            else if (value.IsObject)
            {
                localSerializer.Serialize(writer, value.Value);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override StringOr<T> ReadJson(JsonReader reader, Type objectType, StringOr<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return new StringOr<T>(token.ToObject<string>());
            }
            else if (token.Type == JTokenType.Object)
            {
                T obj = token.ToObject<T>();
                return new StringOr<T>(obj);
            }
            else if (token.Type == JTokenType.Array)
            {
                T[] array = token.ToObject<T[]>();
                return new StringOr<T>(array);
            }
            else
            {
                throw new JsonSerializationException("Unexpected token type when parsing StringOr.");
            }
        }
    }
}