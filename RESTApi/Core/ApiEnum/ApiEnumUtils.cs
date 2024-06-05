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
                ApiEnumAttribute attribute = CachedAttribute<ApiEnumAttribute>.Get(field);
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
                ApiEnumAttribute attribute = CachedAttribute<ApiEnumAttribute>.Get(field);

                if (attribute != null)
                {
                    if (attribute.ApiName == apiName)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }

            //throw new ArgumentException($"'{apiName}' is not a valid value for {typeof(TEnum).Name}.");
            return default;    // return default value instead
        }
    }
}