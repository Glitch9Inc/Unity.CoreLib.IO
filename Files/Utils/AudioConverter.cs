using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Files
{
    public static class AudioConverter
    {
        public static async UniTask<AudioClip> MPEGToAudioClip(byte[] binaryData, string filePath)
        {
            return await HandleMPEGDecodingAsync(binaryData, filePath);
        }

        public static async UniTask<AudioClip> MPEGToAudioClip(string base64Encoded, string filePath)
        {
            byte[] binaryData = Convert.FromBase64String(base64Encoded);
            return await HandleMPEGDecodingAsync(binaryData, filePath);
        }

        public static async UniTask<AudioClip> WAVToAudioClip(byte[] binaryData, string filePath)
        {
            AudioClip clip = WavUtils.ToAudioClip(binaryData);
            if (clip != null && string.IsNullOrWhiteSpace(filePath))
            {
                await WriteAudioFile(binaryData, filePath);
            }
            return clip;
        }

        public static async UniTask<AudioClip> WAVToAudioClip(string base64Encoded, string filePath)
        {
            byte[] binaryData = Convert.FromBase64String(base64Encoded);
            return await WAVToAudioClip(binaryData, filePath);
        }

        public static async UniTask<AudioClip> WavToAudioClip(string localFilePath)
        {
            // use UnityWebRequestMultimedia.GetAudioClip with UniTask
            string uri = "file://" + localFilePath;

            // UnityWebRequest를 사용하여 오디오 클립을 비동기적으로 로드합니다.
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV);
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
                // 성공적으로 로드된 경우, 다운로드된 오디오 클립을 반환합니다.
                return DownloadHandlerAudioClip.GetContent(www);
            }
        }

        public static byte[] ToBytes(this AudioClip clip)
        {
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);

            MemoryStream stream = new();
            BinaryWriter writer = new(stream);

            int length = samples.Length;
            writer.Write(length);

            foreach (float sample in samples)
            {
                writer.Write(sample);
            }

            return stream.ToArray();
        }

        public static async UniTask WriteAudioFile(AudioClip clip, string localFilePath, AudioType audioType = AudioType.MPEG)
        {
            if (clip == null)
            {
                throw new Exception("AudioClip is null.");
            }

            if (string.IsNullOrEmpty(localFilePath))
            {
                throw new Exception("Local file path is null or empty.");
            }

            byte[] bytes;

            if (audioType == AudioType.WAV)
            {
                bytes = WavUtils.FromAudioClip(clip);
            }
            else
            {
                bytes = clip.ToBytes();
            }

            string dir = Path.GetDirectoryName(localFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await File.WriteAllBytesAsync(localFilePath, bytes);
        }

        public static async UniTask WriteAudioFile(float[] floatArray, string localFilePath)
        {
            if (floatArray == null)
            {
                throw new Exception("Failed to write audio file. Audio bytes are null.");
            }

            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                throw new Exception("Failed to write audio file. Local file path is null or empty.");
            }

            // check if filepath has a valid extension (e.g. .wav, .mp3...)
            string extension = Path.GetExtension(localFilePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new Exception("Failed to write audio file. Local file path does not have a valid extension.");
            }

            string directory = Path.GetDirectoryName(localFilePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            MemoryStream stream = new();
            BinaryWriter writer = new(stream);

            int length = floatArray.Length;
            writer.Write(length);

            foreach (float sample in floatArray)
            {
                writer.Write(sample);
            }

            byte[] bytes = stream.ToArray();

            await File.WriteAllBytesAsync(localFilePath, bytes);
            Debug.Log($"Audio file downloaded to {localFilePath}");
        }


        public static async UniTask WriteAudioFile(byte[] audioBytes, string localFilePath)
        {
            if (audioBytes == null)
            {
                throw new Exception("Failed to write audio file. Audio bytes are null.");
            }

            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                throw new Exception("Failed to write audio file. Local file path is null or empty.");
            }

            // check if filepath has a valid extension (e.g. .wav, .mp3...)
            string extension = Path.GetExtension(localFilePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new Exception("Failed to write audio file. Local file path does not have a valid extension.");
            }

            string directory = Path.GetDirectoryName(localFilePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(localFilePath, audioBytes);
            Debug.Log($"Audio file downloaded to {localFilePath}");
        }

        private static async UniTask<AudioClip> HandleMPEGDecodingAsync(byte[] audioBytes, string filePath)
        {
            Debug.Log("Trying to decode MPEG audio file from byte array. This is not supported in WebGL builds.");
            // Generate a temporary path for the audio file.
            // Ensure the file name includes the correct directory separator and a unique identifier to avoid conflicts.
            bool tempPath = false;
            const string EXT = ".mp3";

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    filePath = Path.Combine(Application.persistentDataPath, Path.GetRandomFileName() + EXT);
                    tempPath = true;
                }
                else if (!filePath.EndsWith(EXT))
                {
                    filePath += EXT;
                }

                // Write the audio bytes to a temporary file asynchronously.
                await WriteAudioFile(audioBytes, filePath);

                using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG);
                await request.SendWebRequest();

                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    throw new Exception($"Error while loading audio clip: {request.error}");
                }
                else
                {
                    // Successfully loaded the audio clip.
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                    if (clip == null) throw new Exception("Failed to load audio clip.");

                    return clip;
                }
            }
            finally
            {
                // Ensure the temporary file is deleted after loading the audio clip or if an error occurs.
                if (tempPath && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public static async UniTask<AudioClip> LoadAudioClip(UnityFilePath filePath)
        {
            if (filePath == null) return null;
            return await LoadAudioClip(filePath.Path, filePath.PathType);
        }

        public static async UniTask<AudioClip> LoadAudioClip(string filePath, PathType pathType = PathType.DataPath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            if (pathType == PathType.Resources)
            {
                ResourceRequest request = Resources.LoadAsync<AudioClip>(filePath);
                await request.ToUniTask();
                if (request.asset is AudioClip audioClip) return audioClip;
                return null; // 혹은 오류 처리
            }

            if (pathType == PathType.WebUrl)
            {
                return await LoadAudioClipFromFilePath(filePath);
            }

            filePath = PathResolver.ResolveUnityWebRequestPath(pathType, filePath);
            if (!await PathResolver.DelayedExists(filePath)) return null;

            if (!filePath.StartsWith("file://")) filePath = "file://" + filePath;
            filePath = filePath.Replace("\\", "/");

            return await LoadAudioClipFromFilePath(filePath);
        }

        private static async UniTask<AudioClip> LoadAudioClipFromFilePath(string filePath)
        {
            AudioType audioType = AudioUtils.GetAudioTypeFromFilePath(filePath);
            Debug.Log($"Loading audio clip from file path: {filePath} AudioType: {audioType}");

            // UnityWebRequest를 사용하여 텍스처를 비동기적으로 로드합니다.
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, audioType);

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
                return DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}
