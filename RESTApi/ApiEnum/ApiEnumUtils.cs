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
                ApiEnumAttribute attribute = field.GetCustomAttribute<ApiEnumAttribute>();
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

        public static TEnum ParseEnumFromApiName<TEnum>(string rawName) where TEnum : Enum
        {
            foreach (FieldInfo field in typeof(TEnum).GetFields())
            {
                ApiEnumAttribute attribute = field.GetCustomAttribute<ApiEnumAttribute>();

                if (attribute != null)
                {
                    if (attribute.ApiName == rawName)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException($"'{rawName}' is not a valid value for {typeof(TEnum).Name}.");
        }
    }
}