using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// Custom <see cref="JsonConverter"/> for API enum values.
    /// </summary>
    public class ApiEnumConverter : StringEnumConverter
    {
        public static TEnum Parse<TEnum>(string value) where TEnum : Enum
        {
            if (string.IsNullOrEmpty(value))
                throw new JsonSerializationException("Null or empty string value has been passed to Parse<TEnum> method.");

            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException("Generic parameter TEnum must be an enum type.");

            foreach (string name in Enum.GetNames(enumType))
            {
                System.Reflection.FieldInfo field = enumType.GetField(name);
                if (field == null)
                    continue;

                ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(field);
                if (attribute != null && value.Equals(attribute.ApiName, StringComparison.OrdinalIgnoreCase))
                {
                    return (TEnum)Enum.Parse(enumType, name);
                }

                if (value.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return (TEnum)Enum.Parse(enumType, name);
                }
            }

            throw new JsonSerializationException($"Unknown enum value: {value}");
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (!enumType.IsEnum)
                throw new JsonSerializationException($"Type {enumType.Name} is not an enum.");

            string value = reader.Value?.ToString();
            if (string.IsNullOrEmpty(value))
                throw new JsonSerializationException("Null or empty string value has been passed to ReadJson method.");

            foreach (var name in Enum.GetNames(enumType))
            {
                var field = enumType.GetField(name);
                if (field == null)
                    continue;

                var attribute = AttributeCache<ApiEnumAttribute>.Get(field);
                if (attribute != null && value.Equals(attribute.ApiName, StringComparison.OrdinalIgnoreCase))
                {
                    return Enum.Parse(enumType, name);
                }

                if (value.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new JsonSerializationException($"Unknown enum value: {reader.Value}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type enumType = value?.GetType();
            if (enumType == null) throw new ArgumentNullException(nameof(value));

            string name = enumType.GetEnumName(value);
            if (string.IsNullOrEmpty(name)) throw new JsonSerializationException("Invalid enum value");

            ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(enumType.GetField(name));

            string outputValue = attribute != null ? attribute.ApiName : name;
            writer.WriteValue(outputValue);
        }
    }
}