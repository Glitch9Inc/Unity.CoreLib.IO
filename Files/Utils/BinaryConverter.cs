using System.IO;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Files
{
    public static class BinaryConverter
    {
        public static async UniTask<byte[]> LoadBytes(UnityFilePath filePath)
        {
            string path = filePath.GetLocalPath();
            return await UniTask.RunOnThreadPool(() => System.IO.File.ReadAllBytes(path));
        }

        public static async UniTask<UnityFile> ToBinaryFile(byte[] bytes, string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await File.WriteAllBytesAsync(filePath, bytes);

            UnityPath unityPath = FilePathResolver.ResolveUnityPath(filePath);
            UnityFilePath path = new(unityPath, filePath);
            return new UnityFile() { Path = path, Value = bytes };
        }
    }
}