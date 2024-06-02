using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.IO.RESTApi
{
    internal static class TextResponseConverter
    {
        internal static async UniTask<TRes> ConvertAsync<TRes>(
            string textResult, 
            UnityFilePath downloadPath, 
            JsonSerializerSettings jsonSettings = null) where TRes : RESTObject, new()
        {
            // resultAsString cannot be null at this point. no need to check for null
            TRes response = new() { TextResult = textResult };

            if (downloadPath == null)
            {
                response = JsonConvert.DeserializeObject<TRes>(textResult, jsonSettings);
            }
            else
            {
                switch (downloadPath.Type)
                {
                    case ContentType.Json or ContentType.WWWForm:
                        response = JsonConvert.DeserializeObject<TRes>(textResult, jsonSettings);
                        break;
                    case ContentType.Xml:
                    // Probably just good as Text
                    case ContentType.Csv:
                    // HtmlAgilityPack or 
                    // DOM(Document Object Model) => XPath or LINQ
                    case ContentType.Html:
                    // System.Web.HttpUtility.ParseQueryString
                    case ContentType.MultipartForm:
                        // MultipartFormDataParser?
                        // XDocument or XmlDocument?
                        RESTLog.ResponseError($"{downloadPath.Type} is not supported. Result object will not be created.");
                        break;
                    case ContentType.Mpeg:
                        response.AudioResult = await AudioConverter.MPEGToAudioClip(textResult, downloadPath.GetLocalPath());
                        break;
                    case ContentType.Wav:
                        response.AudioResult = await AudioConverter.WAVToAudioClip(textResult, downloadPath.GetLocalPath());
                        break;
                    default:
                    {
                        if (downloadPath.Type != ContentType.PlainText)
                        {
                            RESTLog.ResponseError($"{downloadPath.Type} is not supported. Result object will not be created.");
                        }
                        break;
                    }
                }
            }

            return response;
        }
    }
}