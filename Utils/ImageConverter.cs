using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO
{
    public static class ImageConverter
    {
        /// <summary>
        /// This can convert JPEG, PNG, BMP, TGA, and TIFF formats.
        /// GIF is not supported.
        /// </summary>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        public static Texture2D BinaryToTexture2D(byte[] binaryData)
        {
            Texture2D texture = new(2, 2); // doesn't matter what size
            texture.LoadImage(binaryData);  // this will auto-resize the texture dimensions.
            return texture;
        }

        public static Texture2D Base64ToTexture2D(string base64Encoded)
        {
            byte[] binaryData = Convert.FromBase64String(base64Encoded);
            return BinaryToTexture2D(binaryData);
        }

        public static string Texture2DToBase64(Texture2D texture)
        {
            byte[] binaryData = texture.EncodeToPNG();
            return Convert.ToBase64String(binaryData);
        }

        public static async UniTask<Texture2D> LoadTexture(string localFilePath)
        {
            string uri = "file://" + localFilePath;

            // UnityWebRequest를 사용하여 텍스처를 비동기적으로 로드합니다.
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
            {
                // SendWebRequest 대신 await를 사용합니다.
                await www.SendWebRequest().WithCancellation(CancellationToken.None);

                // 결과를 검사합니다.
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    // 에러가 발생한 경우, 콘솔에 에러 메시지를 출력합니다.
                    Debug.LogError(www.error);
                    return null;
                }
                else
                {
                    // 성공적으로 로드된 경우, 다운로드된 텍스처를 반환합니다.
                    return DownloadHandlerTexture.GetContent(www);
                }
            }
        }

    }
}