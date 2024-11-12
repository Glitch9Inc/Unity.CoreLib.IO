using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class PathResolver
    {
        /*
         
        Path Types Examples

        1. Application.dataPath                : E:/UnityProjects/Glitch9/AI Development Kit/Assets        
        2. Application.persistentDataPath      : C:/Users/Codeqo/AppData/LocalLow/Glitch9/Routina        
        3. Application.streamingAssetsPath     : E:/UnityProjects/Glitch9/AI Development Kit/Assets/StreamingAssets        
        4. Application.temporaryCachePath      : C:/Users/Codeqo/AppData/Local/Temp/Glitch9/Routina        
        5. Application.consoleLogPath          : C:/Users/Codeqo/AppData/Local/Unity/Editor/Editor.log        
        6. Application.dataPath + "/Resources" : E:/UnityProjects/Glitch9/AI Development Kit/Assets/Resources         
         
         */

        private const int FILE_CHECK_INTERVAL_IN_MILLIS = 3000;

        /// <summary>
        /// Gets the full local path (starting with C:/ or D:/) for a UnityFilePath object.
        /// </summary>
        /// <param name="filePath">The UnityFilePath object</param>
        /// <returns>The resolved local path as a string</returns>
        public static string ResolveFilePath(this UnityFilePath filePath)
        {
            return ResolveUnityWebRequestPath(filePath.PathType, filePath.Path, false);
        }

        /// <summary>
        /// Resolves the local path for a Unity web request based on the given UnityPath type and file path.
        /// </summary>
        /// <param name="type">The UnityPath type</param>
        /// <param name="filePath">The file path as a string</param>
        /// <param name="checkIfFileExists">Whether to check if the file exists</param>
        /// <returns>The resolved local path as a string</returns>
        public static string ResolveUnityWebRequestPath(PathType type, string filePath, bool checkIfFileExists = true)
        {
            if (type == PathType.Unknown) type = ResolvePathType(filePath);

            string fullPath = type switch
            {
                PathType.DataPath => ResolveAssetsPath(filePath, checkIfFileExists),
                PathType.StreamingAssetsPath => ResolveStreamingAssetsPath(filePath, checkIfFileExists),
                PathType.PersistentDataPath => ResolvePersistentDataPath(filePath, checkIfFileExists),
                PathType.WebUrl => filePath,
                _ => filePath
            };

            GNLog.Info($"Resolved file path: {fullPath} ({type})");

            return fullPath;
        }

        public static string ExtractLastTwoSegments(string fullPath)
        {
            int lastSlashIndex = fullPath.LastIndexOf('/');
            if (lastSlashIndex == -1) return fullPath;

            int secondLastSlashIndex = fullPath.LastIndexOf('/', lastSlashIndex - 1);
            if (secondLastSlashIndex == -1) return fullPath;

            return fullPath.Substring(secondLastSlashIndex);
        }

        /// <summary>
        /// Determines the UnityPath type based on the given path.
        /// </summary>
        /// <param name="partialPath">The file path as a string</param>
        /// <returns>The resolved UnityPath type</returns>
        public static PathType ResolvePathType(string partialPath)
        {
            if (string.IsNullOrWhiteSpace(partialPath))
            {
                Debug.LogError("Partial path is null or empty.");
                return PathType.Unknown;
            }

            partialPath = partialPath.FixSlashes();

            if (partialPath.Contains("/StreamingAssets/")) return PathType.StreamingAssetsPath;
            if (partialPath.Contains("/Resources/")) return PathType.Resources;
            if (partialPath.StartsWith("http://") || partialPath.StartsWith("https://")) return PathType.WebUrl;

            const string NOT_PROJECT_PATH = ":/Users/";
            if (partialPath.Contains(NOT_PROJECT_PATH))
            {
                if (partialPath.Contains("/Temp/")) return PathType.TemporaryCachePath;
                if (partialPath.Contains("/Unity/Editor/")) return PathType.ConsoleLogPath;
                return PathType.PersistentDataPath;
            }

            string lastTwoSegsOfDataPath = ExtractLastTwoSegments(Application.dataPath);
            if (partialPath.Contains(lastTwoSegsOfDataPath)) return PathType.DataPath;
            if (partialPath.Contains(":/")) return PathType.Local;
            
            return PathType.Unknown;
        }

        /// <summary>
        /// Resolves the assets path.
        /// </summary>
        /// <param name="filePath">The file path as a string</param>
        /// <param name="checkIfFileExists">Whether to check if the file exists or not</param>
        /// <returns>The resolved assets path as a string</returns>
        public static string ResolveAssetsPath(string filePath, bool checkIfFileExists = true)
        {
            string fullPath = Path.Combine(Application.dataPath, filePath); // Path.Combine을 사용하여 경로를 안전하게 조합
            fullPath = ResolveFilePath(fullPath);
            return checkIfFileExists ? CheckIfFileExists(fullPath) : fullPath;
        }

        public static string ResolveFilePath(string filePath)
        {
            filePath = filePath.FixSlashes();
            if (filePath.Contains("Assets/Assets")) filePath = filePath.Replace("Assets/Assets", "Assets");
            return filePath;
        }

        /// <summary>
        /// Resolves the streaming assets path.
        /// </summary>
        /// <param name="filePath">The file path as a string</param>
        /// <param name="checkIfFileExists">Whether to check if the file exists or not</param>
        /// <returns>The resolved streaming assets path as a string</returns>
        public static string ResolveStreamingAssetsPath(string filePath, bool checkIfFileExists = true)
        {
            const string PREFIX = "StreamingAssets/";
            if (filePath.StartsWith(PREFIX)) filePath = filePath.Substring(PREFIX.Length); // 접두어 제거
            string fullPath = Path.Combine(Application.streamingAssetsPath, filePath); // 경로 조합
            return checkIfFileExists ? CheckIfFileExists(fullPath) : fullPath;
        }

        /// <summary>
        /// Resolves the persistent data path.
        /// </summary>
        /// <param name="filePath">The file path as a string</param>
        /// <param name="checkIfFileExists">Whether to check if the file exists or not</param>
        /// <returns>The resolved persistent data path as a string</returns>
        public static string ResolvePersistentDataPath(string filePath, bool checkIfFileExists = true)
        {
            // 경로 구분자를 정확히 처리하고 절대 경로를 정확하게 검증
            string fullPath = Path.Combine(Application.persistentDataPath, "");
            if (filePath.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
                return checkIfFileExists ? CheckIfFileExists(filePath) : filePath;

            fullPath = Path.Combine(Application.persistentDataPath, filePath);
            return checkIfFileExists ? CheckIfFileExists(fullPath) : fullPath;
        }

        /// <summary>
        /// Validates the local path and ensures the file exists.
        /// </summary>
        /// <param name="fullPath">The full file path as a string</param>
        /// <returns>The validated full path as a string</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found</exception>
        private static string CheckIfFileExists(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"File not found: {fullPath}");
                throw new FileNotFoundException($"File not found: {fullPath}");
            }

            return fullPath;
        }

        /// <summary>
        /// Checks if the file exists asynchronously, with a delay.
        /// </summary>
        /// <param name="filePath">The file path as a string</param>
        /// <returns>True if the file exists, false otherwise</returns>
        public static async UniTask<bool> DelayedExists(string filePath)
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

        /// <summary>
        /// In case of multiple images,
        /// ensure the file path is unique by appending an index to the file path.
        /// </summary>
        /// <param name="localPath">The file path to ensure uniqueness</param>
        /// <returns>Unique file path format</returns>
        public static string GetUniqueFilePathFormat(string localPath)
        {
            string ext = Path.GetExtension(localPath);
            string fileName = Path.GetFileNameWithoutExtension(localPath);
            string dir = Path.GetDirectoryName(localPath);
            if (string.IsNullOrEmpty(dir)) dir = Application.persistentDataPath;
            return Path.Combine(dir, $"{fileName}_{{0}}{ext}");
        }
        
    }
}