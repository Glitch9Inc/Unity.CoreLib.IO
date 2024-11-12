using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.IO.RESTApi
{
    internal static class TextResponseConverter
    {
        private static TRes Deserialize<TRes>(string jsonString, RESTClient client)
        {
            //jsonString = RemoveAdditionalTextFromJson(jsonString);
            Type type = typeof(TRes);
            string typeName;
            if (type.IsGenericType)
            {
                string genericName = type.GetGenericArguments()[0].Name;
                typeName = $"{type.Name.Split('`')[0]}<{genericName}>";
            }
            else
            {
                typeName = type.Name;
            }
            client.InternalLogger.ResponseInfo($"Deserializing <color=yellow>{typeName}</color> from JSON.");
            return JsonConvert.DeserializeObject<TRes>(jsonString, client.JsonSettings);
        }
        
        internal static async UniTask<TRes> ConvertAsync<TRes>(
            string textResult, 
            UnityFilePath downloadPath, 
            RESTClient client) where TRes : RESTResponse, new()
        {
            if (string.IsNullOrEmpty(textResult)) return null;

            TRes response = new() { TextOutput = textResult };

            if (downloadPath == null)
            {
                response = Deserialize<TRes>(textResult, client);
            }
            else
            {
                switch (downloadPath.Type)
                {
                    case ContentType.Json or ContentType.WWWForm:
                        response = Deserialize<TRes>(textResult, client);
                        break;
                    case ContentType.Xml:
                    // Probably just good as Text
                    case ContentType.CSV:
                    // HtmlAgilityPack or 
                    // DOM(Document Object Model) => XPath or LINQ
                    case ContentType.HTML:
                    // System.Web.ttpUtility.ParseQueryString
                    case ContentType.MultipartForm:
                        // MultipartFormDataParser?
                        // XDocument or XmlDocument?
                        client.InternalLogger.ResponseError($"{downloadPath.Type} is not supported. Result object will not be created.");
                        break;
                    case ContentType.MPEG:
                        response.AudioOutput = await AudioConverter.MPEGToAudioClip(textResult, downloadPath.ResolveFilePath());
                        break;
                    case ContentType.WAV:
                        response.AudioOutput = await AudioConverter.WAVToAudioClip(textResult, downloadPath.ResolveFilePath());
                        break;
                    default:
                    {
                        if (downloadPath.Type != ContentType.PlainText)
                        {
                            client.InternalLogger.ResponseError($"{downloadPath.Type} is not supported. Result object will not be created.");
                        }
                        break;
                    }
                }
            }

            return response;
        }
    }
}