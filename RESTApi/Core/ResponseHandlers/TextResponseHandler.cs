using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Glitch9.IO.RESTApi
{
    public static class TextResponseHandler
    {
        public static async UniTask<TRes> HandleAsync<TRes>(byte[] binaryResult, string textResult, string filePath, ContentType contentType, JsonSerializerSettings jsonSettings = null)
            where TRes : RESTResponse, new()
        {
            // resultAsString cannot be null at this point. no need to check for null
            TRes response = new() { BinaryResult = binaryResult, TextResult = textResult };

            if (contentType == ContentType.JSON)
            {
                response = JsonConvert.DeserializeObject<TRes>(textResult, jsonSettings);
            }
            else if (contentType == ContentType.XML)
            {
                // XDocument or XmlDocument?
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.CSV)
            {
                // Probably just good as Text
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.HTML)
            {
                // HtmlAgilityPack or 
                // DOM(Document Object Model) => XPath or LINQ
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.Form)
            {
                // System.Web.HttpUtility.ParseQueryString
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.Multipart)
            {
                // MultipartFormDataParser?
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.MPEG)
            {
                response.AudioResult = await AudioConverter.MPEGToAudioClip(textResult, filePath);
            }
            else if (contentType == ContentType.WAV)
            {
                response.AudioResult = await AudioConverter.WAVToAudioClip(textResult, filePath);
            }
            else if (contentType != ContentType.PlainText)
            {
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }

            response.OnResponseConverted(textResult);
            return response;
        }
    }
}