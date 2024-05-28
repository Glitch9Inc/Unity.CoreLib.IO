using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{

    public static class FilePathResolver
    {
        private const int FILE_CHECK_INTERVAL_IN_MILLIS = 3000;

        public static string ResolveFilePath(this FilePath filePath)
        {
            return ResolveUnityWebRequestLocalPath(filePath.UnityPath, filePath.Path);
        }

        public static string ResolveUnityWebRequestLocalPath(UnityPath type, string filePath, bool validatePath = true)
        {
            string fullPath = type switch
            {
                UnityPath.Assets => ResolveAssetsPath(filePath, validatePath),
                UnityPath.StreamingAsset => ResolveStreamingAssetsPath(filePath, validatePath),
                UnityPath.PersistentData => ResolvePersistentDataPath(filePath, validatePath),
                UnityPath.URL => filePath,
                _ => string.Empty
            };
            return fullPath;
        }

        public static string ResolveAssetsPath(string filePath, bool checkIfFileExists = true)
        {
            const string PREFIX = "Assets/";
            if (filePath.StartsWith(PREFIX)) filePath = filePath.Substring(PREFIX.Length); // 접두어 제거
            string fullPath = Path.Combine(Application.dataPath, filePath); // Path.Combine을 사용하여 경로를 안전하게 조합
            return checkIfFileExists ? ValidateLocalPath(fullPath) : fullPath;
        }

        public static string ResolveStreamingAssetsPath(string filePath, bool checkIfFileExists = true)
        {
            const string PREFIX = "StreamingAssets/";
            if (filePath.StartsWith(PREFIX)) filePath = filePath.Substring(PREFIX.Length); // 접두어 제거
            string fullPath = Path.Combine(Application.streamingAssetsPath, filePath); // 경로 조합
            return checkIfFileExists ? ValidateLocalPath(fullPath) : fullPath;
        }

        public static string ResolvePersistentDataPath(string filePath, bool checkIfFileExists = true)
        {
            // 경로 구분자를 정확히 처리하고 절대 경로를 정확하게 검증
            string fullPath = Path.Combine(Application.persistentDataPath, "");
            if (filePath.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
                return checkIfFileExists ? ValidateLocalPath(filePath) : filePath;

            fullPath = Path.Combine(Application.persistentDataPath, filePath);
            return checkIfFileExists ? ValidateLocalPath(fullPath) : fullPath;
        }

        private static string ValidateLocalPath(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"File not found: {fullPath}");
                throw new FileNotFoundException($"File not found: {fullPath}");
            }

            return fullPath;
        }

        public static async UniTask<bool> Exists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            if (!File.Exists(filePath)) // check if the file exists
            {
                await UniTask.Delay(FILE_CHECK_INTERVAL_IN_MILLIS); // 3초뒤 다시 확인
            }

            if (!File.Exists(filePath)) // check if the file exists
            {
                Debug.LogError($"File not found: {filePath}");
                return false;
            }

            return true;
        }
    }
}