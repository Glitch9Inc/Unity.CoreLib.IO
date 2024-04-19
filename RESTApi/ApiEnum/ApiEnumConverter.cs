using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;

namespace Glitch9.IO.RESTApi
{
    public class ApiEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type enumType = value.GetType();
            string name = enumType.GetEnumName(value);
            System.Reflection.MemberInfo member = enumType.GetMember(name)[0];
            ApiEnumAttribute attribute = member.GetCustomAttributes(typeof(ApiEnumAttribute), false)
                .Cast<ApiEnumAttribute>()
                .FirstOrDefault();

            string outputValue = attribute != null ? attribute.ApiName : name;
            writer.WriteValue(outputValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type enumType = objectType;
            if (Nullable.GetUnderlyingType(objectType) != null)
            {
                enumType = Nullable.GetUnderlyingType(objectType);
            }

            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                System.Reflection.MemberInfo member = enumType.GetMember(name)[0];
                ApiEnumAttribute attribute = member.GetCustomAttributes(typeof(ApiEnumAttribute), false)
                    .Cast<ApiEnumAttribute>()
                    .FirstOrDefault();

                if ((attribute != null && reader.Value.ToString().Equals(attribute.ApiName)) ||
                    reader.Value.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new JsonSerializationException($"Unknown enum value: {reader.Value}");
        }
    }
}