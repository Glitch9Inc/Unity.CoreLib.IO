using Glitch9.IO.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    internal static class RequestExtensions
    {
        internal static byte[] ToJson<TReq>(this TReq req, RESTClient client)
            where TReq : RESTRequest
        {
            if (req == null) return null;
            string bodyString = JsonConvert.SerializeObject(req, client.JsonSettings);
            if (string.IsNullOrEmpty(bodyString)) return null;
            if (client.LogRequestBody) client.InternalLogger.RequestBody(bodyString);
            return Encoding.UTF8.GetBytes(bodyString);
        }

        // internal static List<IMultipartFormSection> ToMultipartFormSections<TReq>(this TReq req, RESTClient client)
        //     where TReq : RESTRequest
        // {
        //     if (req == null) return null;

        //     List<IMultipartFormSection> formData = new();

        //     // use reflection to get all public properties of the request
        //     // ignore if it has a [JsonIgnore] attribute
        //     // rename the property if it has a [JsonProperty] attribute
        //     // add each property to the form data

        //     List<PropertyInfo> properties = PropertyInfoCache.Get<TReq>();

        //     foreach (PropertyInfo prop in properties)
        //     {
        //         JsonPropertyAttribute jsonProp = AttributeCache<JsonPropertyAttribute>.Get(prop);
        //         if (jsonProp == null) continue;
        //         string key = jsonProp.PropertyName;
        //         object value = prop.GetValue(req);
        //         if (value == null) continue;
        //         formData.Add(ValueToFormSection(key, value));
        //     }

        //     if (client.LogRequestBody)
        //     {
        //         using (StringBuilderPool.Get(out StringBuilder sb))
        //         {
        //             sb.AppendLine("Multipart Form Data:");
        //             foreach (IMultipartFormSection section in formData)
        //             {
        //                 sb.AppendLine($"{section.sectionName}: {section.sectionData}");
        //             }
        //             client.InternalLogger.RequestBody(sb.ToString());
        //         }
        //     }

        //     return formData;
        // }

        internal static List<IMultipartFormSection> ToMultipartFormSections<TReq>(this TReq req, RESTClient client)
            where TReq : RESTRequest
        {
            if (req == null) return null;

            List<IMultipartFormSection> formData = new();

            // use reflection to get all public properties of the request
            List<PropertyInfo> properties = PropertyInfoCache.Get<TReq>();

            JsonSerializer jsonSerializer = JsonSerializer.Create(client.JsonSettings);

            foreach (PropertyInfo prop in properties)
            {
                // Skip properties with [JsonIgnore]
                JsonIgnoreAttribute jsonIgnore = AttributeCache<JsonIgnoreAttribute>.Get(prop);
                if (jsonIgnore != null) continue;

                // Handle properties with [JsonProperty] for renaming
                JsonPropertyAttribute jsonProp = AttributeCache<JsonPropertyAttribute>.Get(prop);
                string key = jsonProp != null ? jsonProp.PropertyName : prop.Name;

                object value = prop.GetValue(req);
                if (value == null) continue;

                // if the value if 'FormFile' or 'byte[]' then add it as a file section
                if (value is FormFile || value is byte[])
                {
                    formData.Add(ValueToFormSection(key, value));
                    continue;
                }

                // Serialize the value using the provided JsonSettings
                string serializedValue;
                using (var writer = new StringWriter())
                {
                    jsonSerializer.Serialize(writer, value);
                    serializedValue = writer.ToString();

                    // Remove quotes from serialized string
                    serializedValue = serializedValue.Trim('"');
                }
                formData.Add(ValueToFormSection(key, serializedValue));
            }

            if (client.LogRequestBody)
            {
                using (StringBuilderPool.Get(out StringBuilder sb))
                {
                    sb.AppendLine("Multipart Form Data:");
                    foreach (IMultipartFormSection section in formData)
                    {
                        if (section is MultipartFormDataSection)
                        {
                            sb.AppendLine($"{section.sectionName}: {section.sectionData}");
                        }
                        else
                        {
                            sb.AppendLine($"{section.sectionName}: <file>");
                        }
                    }
                    client.InternalLogger.RequestBody(sb.ToString());
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