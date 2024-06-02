using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Files
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

        public static async UniTask<Texture2D> LoadTexture(UnityFilePath filePath)
        {
            if (filePath == null) return null;
            return await LoadTexture(filePath.Path, filePath.UnityPath);
        }

        public static async UniTask<Texture2D> LoadTexture(string filePath, UnityPath path = UnityPath.Assets)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            if (path == UnityPath.Resources)
            {
                ResourceRequest request = Resources.LoadAsync<Texture2D>(filePath);
                await request.ToUniTask();
                if (request.asset is Texture2D texture) return texture;
                return null; // 혹은 오류 처리
            }

            if (path == UnityPath.URL)
            {
                return await LoadTextureFromUrl(filePath);
            }

            filePath = FilePathResolver.ResolveUnityWebRequestLocalPath(path, filePath);

            try
            {
                return await LoadTextureFromLocal(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load texture from {filePath}. Error: {e.Message}");
                return null;
            }
        }

        public static byte[] ToBytes(this Texture2D texture)
        {
            return texture.EncodeToPNG();
        }

        private static async UniTask<Texture2D> LoadTextureFromLocal(string filePath)
        {
            if (!await FilePathResolver.DelayedExists(filePath)) return null;
            byte[] fileData = await File.ReadAllBytesAsync(filePath);
            Texture2D texture = new(2, 2);
            texture.LoadImage(fileData);
            return texture;
        }

        private static async UniTask<Texture2D> LoadTextureFromUrl(string url)
        {
            // UnityWebRequest를 사용하여 텍스처를 비동기적으로 로드합니다.
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            // SendWebRequest 대신 await를 사용합니다.
            await www.SendWebRequest().WithCancellation(CancellationToken.None);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception($"Failed to load texture from {url}. Error: {www.error}");
            }
            else
            {
                // 성공적으로 로드된 경우, 다운로드된 텍스처를 반환합니다.
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}