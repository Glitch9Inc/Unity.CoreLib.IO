using Glitch9.IO.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    internal static class RequestExtensions
    {
        internal static byte[] ToJson<TReq>(this TReq req, bool logBody, JsonSerializerSettings settings)
            where TReq : RESTRequest
        {
            if (req == null) return null;
            string bodyString = JsonConvert.SerializeObject(req, settings);
            if (string.IsNullOrEmpty(bodyString)) return null;
            if (logBody) RESTLog.RequestBody(bodyString);
            return Encoding.UTF8.GetBytes(bodyString);
        }

        internal static List<IMultipartFormSection> ToMultipartFormSections<TReq>(this TReq req, bool logBody)
            where TReq : RESTRequest
        {
            if (req == null) return null;

            List<IMultipartFormSection> formData = new();

            // use reflection to get all public properties of the request
            // ignore if it has a [JsonIgnore] attribute
            // rename the property if it has a [JsonProperty] attribute
            // add each property to the form data

            List<PropertyInfo> properties = CachedPropertyInfo.Get<TReq>();

            foreach (PropertyInfo prop in properties)
            {
                JsonPropertyAttribute jsonProp = CachedAttribute<JsonPropertyAttribute>.Get(prop);
                if (jsonProp == null) continue;
                string key = jsonProp.PropertyName;
                object value = prop.GetValue(req);
                if (value == null) continue;
                formData.Add(ValueToFormSection(key, value));
            }

            if (logBody)
            {
                using (StringBuilderPool.Get(out StringBuilder sb))
                {
                    sb.AppendLine("Multipart Form Data:");
                    foreach (IMultipartFormSection section in formData)
                    {
                        sb.AppendLine($"{section.sectionName}: {section.sectionData}");
                    }
                    RESTLog.RequestBody(sb.ToString());
                }
            }

            return formData;
        }

        private static IMultipartFormSection ValueToFormSection(string key, object value)
        {
            return value switch
            {
                byte[] bytes => new MultipartFormFileSection(key, bytes, "file", "application/octet-stream"),
                FormFile file => new MultipartFormFileSection(key, file.Data, file.FileName, file.ContentType.ToMIME()),
                _ => new MultipartFormDataSection(key, Convert.ToString(value, CultureInfo.InvariantCulture))
            };
        }
    }
}