using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public static class FormUtils
    {
        // Add property cache to reduce reflection overhead
        private static readonly Dictionary<Type, List<PropertyInfo>> _propertyCache = new();

        public static List<IMultipartFormSection> ToForm<TReq>(this TReq req)
            where TReq : RESTRequest
        {
            if (req == null) return null;
            List<IMultipartFormSection> formData = new();

            // use reflection to get all public properties of the request
            // ignore if it has a [JsonIgnore] attribute
            // rename the property if it has a [JsonProperty] attribute
            // add each property to the form data

            if (!_propertyCache.TryGetValue(req.GetType(), out List<PropertyInfo> properties))
            {
                properties = req.GetType().GetProperties().ToList();
                _propertyCache.Add(req.GetType(), properties);
            }

            foreach (PropertyInfo prop in properties)
            {
                if (prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0) continue;
                string key = prop.Name;
                object[] jsonProp = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
                if (jsonProp.Length > 0) key = ((JsonPropertyAttribute)jsonProp[0]).PropertyName;
                object value = prop.GetValue(req);
                if (value == null) continue;
                formData.Add(ValueToFormSection(key, value));
            }

            return formData;
        }

        public static IMultipartFormSection ValueToFormSection(string key, object value)
        {
            return value switch
            {
                string str => new MultipartFormDataSection(key, str),
                int i => new MultipartFormDataSection(key, i.ToString()),
                float f => new MultipartFormDataSection(key, f.ToString(CultureInfo.InvariantCulture)),
                double d => new MultipartFormDataSection(key, d.ToString(CultureInfo.InvariantCulture)),
                bool b => new MultipartFormDataSection(key, b.ToString()),
                FormFile file => new MultipartFormFileSection(key, file.Data, file.FileName, file.ContentType.ToMIME()),
                _ => throw new ArgumentException($"Unsupported type: {value.GetType()}")
            };
        }
    }
}