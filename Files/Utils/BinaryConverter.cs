using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Files
{
    public static class BinaryConverter
    {
        public static async UniTask<byte[]> LoadBytes(FilePath filePath)
        {
            string path = filePath.ResolveFilePath();
            return await UniTask.RunOnThreadPool(() => System.IO.File.ReadAllBytes(path));
        }
    }
}