using System;
using System.Reflection;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    public static class ApiEnumUtils
    {
        public static string ToApiName(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            try
            {
                ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(field);
                if (attribute != null)
                {
                    return attribute.ApiName;
                }
                return value.ToString();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return string.Empty;
            }
        }

        public static TEnum ParseEnum<TEnum>(string apiName) 
            where TEnum : Enum
        {
            foreach (FieldInfo field in typeof(TEnum).GetFields())
            {
                ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(field);

                if (attribute != null)
                {
                    if (attribute.ApiName == apiName)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }

            // parse normally
            return (TEnum)Enum.Parse(typeof(TEnum), apiName);
        }

        public static bool TryParse<TEnum>(string apiName, out TEnum result, bool ignoreCase = false)
            where TEnum : struct, Enum
        {
            foreach (FieldInfo field in typeof(TEnum).GetFields())
            {
                ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(field);

                if (attribute != null)
                {
                    if (string.Equals(attribute.ApiName, apiName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        result = (TEnum)field.GetValue(null);
                        return true;
                    }
                }
            }

            // parse normally
            return Enum.TryParse(apiName, ignoreCase, out result);
        }

        public static bool TryParse(Type enumType, string apiName, out object result, bool ignoreCase = false)
        {
            foreach (FieldInfo field in enumType.GetFields())
            {
                ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(field);

                if (attribute != null)
                {
                    if (string.Equals(attribute.ApiName, apiName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        result = field.GetValue(null);
                        return true;
                    }
                }
            }

            // parse normally
            return Enum.TryParse(enumType, apiName, ignoreCase, out result);
        }
    }
}