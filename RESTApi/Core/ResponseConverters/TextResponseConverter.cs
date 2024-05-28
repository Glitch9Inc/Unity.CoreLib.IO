using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.IO.RESTApi
{
    public static class TextResponseConverter
    {
        public static async UniTask<TRes> ConvertAsync<TRes>(
            byte[] binaryResult, 
            string textResult, 
            string filePath, 
            ContentType contentType, 
            JsonSerializerSettings jsonSettings = null) where TRes : RESTResult, new()
        {
            // resultAsString cannot be null at this point. no need to check for null
            TRes response = new() { BinaryResult = binaryResult, TextResult = textResult };

            if (contentType == ContentType.Json || contentType == ContentType.WWWForm)
            {
                response = JsonConvert.DeserializeObject<TRes>(textResult, jsonSettings);
            }
            else if (contentType == ContentType.Xml)
            {
                // XDocument or XmlDocument?
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.Csv)
            {
                // Probably just good as Text
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.Html)
            {
                // HtmlAgilityPack or 
                // DOM(Document Object Model) => XPath or LINQ
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.MultipartForm)
            {
                // System.Web.HttpUtility.ParseQueryString
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.Multipart)
            {
                // MultipartFormDataParser?
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }
            else if (contentType == ContentType.Mpeg)
            {
                response.AudioResult = await AudioConverter.MPEGToAudioClip(textResult, filePath);
            }
            else if (contentType == ContentType.Wav)
            {
                response.AudioResult = await AudioConverter.WAVToAudioClip(textResult, filePath);
            }
            else if (contentType != ContentType.PlainText)
            {
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }

            response.OnResponseConverted(textResult);
            return response;
        }
    }
}