using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO
{
    public class Downloader
    {
        private const int MAX_DL_FAIL_COUNT = 5;

        public static async UniTask<bool> DownloadFileAsync(string uri, string dlLocalPath)
        {
            if (!TryCreateUri(uri, out Uri result)) return false;
            return await DownloadWithRetryAsync(result, dlLocalPath);
        }

        private static async UniTask<bool> DownloadWithRetryAsync(Uri uri, string dlPath)
        {
            // Idk why but I make this mistake a lot.
            dlPath = dlPath.Replace("//", "/");

            string dir = Path.GetDirectoryName(dlPath);
            GNLog.Log($"Download Dir: {dir} | Download Path: {dlPath}");
            
            if (!Directory.Exists(dir))
            {
                GNLog.Log($"Creating directory: {dir}");
                Directory.CreateDirectory(dir);
            }

            int attempt = 0;
            while (attempt < MAX_DL_FAIL_COUNT)
            {
                if (await TryDownloadAsync(uri, dlPath)) return true;
                else attempt++;
            }

            DisplayEditorDialogue($"Failed to download the file after {MAX_DL_FAIL_COUNT} attempts from <color=blue>{uri}</color>.");
            return false;
        }

        private static async UniTask<bool> TryDownloadAsync(Uri uri, string dlLocalPath)
        {
            using UnityWebRequest www = UnityWebRequest.Get(uri);

            // check if the file already exists and add suffix if it does
            if (File.Exists(dlLocalPath))
            {
                string ext = Path.GetExtension(dlLocalPath);
                string fileName = Path.GetFileNameWithoutExtension(dlLocalPath);
                string dir = Path.GetDirectoryName(dlLocalPath);

                string[] files = Directory.GetFiles(dir, $"{fileName}*{ext}");
                if (files.Length > 0)
                {
                    int max = files.Select(f => f.Split('_').Last()).Select(f => int.TryParse(f, out int i) ? i : 0).Max();
                    dlLocalPath = Path.Combine(dir, $"{fileName}_{max + 1}{ext}");
                }
            }

            www.downloadHandler = new DownloadHandlerFile(dlLocalPath);

            // Convert UnityWebRequest's coroutine pattern into a Task
            TaskCompletionSource<bool> completionSource = new();
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            operation.completed += _ =>
            {
                if (www == null)
                {
                    DisplayEditorDialogue("Download failed. UnityWebRequest is null.");
                    completionSource.SetResult(false);
                    return;
                }

                if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    DisplayEditorDialogue("Download failed: " + uri + "\nError:" + www.error);
                    completionSource.SetResult(false);
                }
                else
                {
                    DisplayEditorDialogue($"The file is successfully downloaded to <color=blue>{dlLocalPath}</color>.");
                    completionSource.SetResult(true);
                }
            };

            return await completionSource.Task;
        }

        private static bool TryCreateUri(string uri, out Uri result)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out result))
            {
                DisplayEditorDialogue($"Invalid Uri: {uri}", true);
                return false;
            }

            return true;
        }

        private static string GetExtensionString(Uri result, FileExt extension)
        {
            string extensionString = extension.ToString();

            if (extension == FileExt.Unset)
            {
                extensionString = result.Segments.Last();
                if (extensionString.Contains(".")) extensionString = extensionString.Split('.').Last();
            }

            return extensionString;
        }

        private static void DisplayEditorDialogue(string v2, bool isError = false)
        {
            if (isError) GNLog.Error(v2);
            else GNLog.Log(v2);
        }

        #region Audio Download

        private static AudioType GetAudioType(string extensionString)
        {
            switch (extensionString)
            {
                case "wav": return AudioType.WAV;
                case "mp3": return AudioType.MPEG;
                case "ogg": return AudioType.OGGVORBIS;
                case "aif":
                case "aiff": return AudioType.AIFF;
                case "acc": return AudioType.ACC;
                case "it": return AudioType.IT;
                case "mod": return AudioType.MOD;
                case "s3m": return AudioType.S3M;
                case "xm": return AudioType.XM;
                case "xma": return AudioType.XMA;
                case "vag": return AudioType.VAG;
                default:
                    DisplayEditorDialogue($"Unsupported Audio Format: {extensionString}", true);
                    return AudioType.UNKNOWN;
            }
        }

        public static async UniTask<AudioClip> DownloadAudioClipAsync(string uri, string dlLocalPath, FileExt extension = FileExt.Unset)
        {
            if (!TryCreateUri(uri, out Uri result)) return null;

            string extensionString = GetExtensionString(result, extension);
            AudioType audioType = GetAudioType(extensionString);

            if (await DownloadWithRetryAsync(result, dlLocalPath))
            {
                return await LoadAudioClipAsync(dlLocalPath, audioType);
            }
            else
            {
                return null;
            }
        }

        public static async UniTask<AudioClip> DownloadLiveAudioClipAsync(string httpUrl, FileExt extension = FileExt.Unset)
        {
            if (!TryCreateUri(httpUrl, out Uri result)) return null;

            string extensionString = GetExtensionString(result, extension);
            AudioType audioType = GetAudioType(extensionString);

            StringBuilder dlPathSb = new();
            dlPathSb.Append(Application.persistentDataPath);
            dlPathSb.Append("/temp_audio.");
            dlPathSb.Append(extensionString);
            string dlPath = dlPathSb.ToString();

            if (await DownloadWithRetryAsync(result, dlPath))
            {
                return await LoadAudioClipAsync(dlPath, audioType);
            }
            else
            {
                return null;
            }
        }

        private static async UniTask<AudioClip> LoadAudioClipAsync(string dlLocalPath, AudioType audioType)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + dlLocalPath, audioType);
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await UniTask.Yield();
            }

            if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                DisplayEditorDialogue("Download failed: " + dlLocalPath + "\nError:" + www.error);
                return null;
            }
            else
            {
                DisplayEditorDialogue($"The file is successfully loaded from <color=blue>{dlLocalPath}</color>.");
                return DownloadHandlerAudioClip.GetContent(www);
            }
        }

        #endregion

        #region Image Download

        public static async UniTask<Texture2D> DownloadTextureAsync(string uri, string dlLocalPath)
        {
            if (!TryCreateUri(uri, out Uri result)) return null;

            if (await DownloadWithRetryAsync(result, dlLocalPath))
            {
                return await LoadTextureAsync(dlLocalPath);
            }
            else
            {
                return null;
            }
        }

        private static async UniTask<Texture2D> LoadTextureAsync(string dlLocalPath)
        {
            byte[] fileData = await File.ReadAllBytesAsync(dlLocalPath);
            Texture2D texture = new(2, 2);
            texture.LoadImage(fileData);

            DisplayEditorDialogue($"The file is successfully loaded from <color=blue>{dlLocalPath}</color>.");
            return texture;
        }

        #endregion
    }
}