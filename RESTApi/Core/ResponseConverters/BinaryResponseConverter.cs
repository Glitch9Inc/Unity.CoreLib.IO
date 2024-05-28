using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;

namespace Glitch9.IO.RESTApi
{
    public static class BinaryResponseConverter
    {
        public static async UniTask<TRes> ConvertAsync<TRes>(
            byte[] result, 
            string resultAsString, 
            string filePath, 
            ContentType contentType) where TRes : RESTResult, new()
        {
            // result cannot be null at this point. no need to check for null
            TRes response = new() { BinaryResult = result, TextResult = resultAsString };

            // TODO: Convert the byte array to the appropriate type
            if (contentType is ContentType.Png or ContentType.Jpeg)
            {
                RESTLog.ResponseError($"{contentType} will be supported soon.");
            }
            else if (contentType == ContentType.Gif)
            {
                RESTLog.ResponseError("GIF is not supported. Texture2D will not be created.");
            }
            else if (contentType == ContentType.Mpeg)
            {
                response.AudioResult = await AudioConverter.MPEGToAudioClip(result, filePath);
            }
            else if (contentType == ContentType.Wav)
            {
                response.AudioResult = await AudioConverter.WAVToAudioClip(result, filePath);
            }
            else
            {
                RESTLog.ResponseError($"{contentType} is not supported. Result object will not be created.");
            }

            await UniTask.Delay(1000);
            response.OnResponseConverted(resultAsString);
            return response;
        }
    }
}