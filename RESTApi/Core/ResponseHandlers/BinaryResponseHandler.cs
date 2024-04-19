using Cysharp.Threading.Tasks;

namespace Glitch9.IO.RESTApi
{
    public static class BinaryResponseHandler
    {
        public static async UniTask<TRes> HandleAsync<TRes>(byte[] result, string resultAsString, string filePath, ContentType contentType) where TRes : RESTResponse, new()
        {
            // result cannot be null at this point. no need to check for null
            TRes response = new() { BinaryResult = result, TextResult = resultAsString };

            // TODO: Convert the byte array to the appropriate type
            if (contentType is ContentType.PNG or ContentType.JPEG)
            {
                GNLog.Error($"{contentType} will be supported soon.");
            }
            else if (contentType == ContentType.GIF)
            {
                GNLog.Error("GIF is not supported. Texture2D will not be created.");
            }
            else if (contentType == ContentType.MPEG)
            {
                response.AudioResult = await AudioConverter.MPEGToAudioClip(result, filePath);
            }
            else if (contentType == ContentType.WAV)
            {
                response.AudioResult = await AudioConverter.WAVToAudioClip(result, filePath);
            }
            else
            {
                GNLog.Error($"{contentType} is not supported. Result object will not be created.");
            }

            await UniTask.Delay(1000);
            response.OnResponseConverted(resultAsString);
            return response;
        }
    }
}