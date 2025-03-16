using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;

namespace Glitch9.IO.RESTApi
{
    internal static class BinaryResponseConverter
    {
        internal static async UniTask<TRes> ConvertAsync<TRes>(
            byte[] result,
            UnityFilePath downloadPath,
            RESTClient client) where TRes : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(downloadPath);

            // Result cannot be null at this point. No need to check for null
            TRes response = new() { BinaryOutput = result };

            // Converting the byte array to the appropriate type
            switch (downloadPath.Type)
            {
                case ContentType.PNG or ContentType.JPEG:
                    response.ImageOutput = ImageConverter.BinaryToTexture2D(result);
                    break;
                case ContentType.GIF:
                    client.InternalLogger.ResponseError("GIF is not supported. Texture2D will not be created.");
                    break;
                case ContentType.MPEG:
                    response.AudioOutput = await AudioConverter.MPEGToAudioClip(result, downloadPath.ResolveFilePath());
                    break;
                case ContentType.WAV:
                    response.AudioOutput = await AudioConverter.WAVToAudioClip(result, downloadPath.ResolveFilePath());
                    break;
                default:
                    response.FileOutput = await BinaryUtils.ToBinaryFile(result, downloadPath.ResolveFilePath());
                    break;
            }

            await UniTask.Delay(1000);
            return response;
        }
    }
}