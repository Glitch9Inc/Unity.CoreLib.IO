using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;

namespace Glitch9.IO.RESTApi
{
    internal static class BinaryResponseConverter
    {
        internal static async UniTask<TRes> ConvertAsync<TRes>(
            byte[] result,
            UnityFilePath downloadPath,
            RESTClient client) where TRes : RESTObject, new()
        {
            Validate.Argument.NotNull(downloadPath);

            // Result cannot be null at this point. No need to check for null
            TRes response = new() { BinaryResult = result };

            // Converting the byte array to the appropriate type
            switch (downloadPath.Type)
            {
                case ContentType.Png or ContentType.Jpeg:
                    response.ImageResult = ImageConverter.BinaryToTexture2D(result);
                    break;
                case ContentType.Gif:
                    client.InternalLogger.ResponseError("GIF is not supported. Texture2D will not be created.");
                    break;
                case ContentType.Mpeg:
                    response.AudioResult = await AudioConverter.MPEGToAudioClip(result, downloadPath.GetLocalPath());
                    break;
                case ContentType.Wav:
                    response.AudioResult = await AudioConverter.WAVToAudioClip(result, downloadPath.GetLocalPath());
                    break;
                default:
                    response.FileResult = await BinaryConverter.ToBinaryFile(result, downloadPath.GetLocalPath());
                    break;
            }

            await UniTask.Delay(1000);
            return response;
        }
    }
}