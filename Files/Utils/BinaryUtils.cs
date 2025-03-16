using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Files
{
    public static class BinaryUtils
    {
        public static async UniTask<byte[]> LoadBytes(UnityFilePath filePath)
        {
            string path = filePath.ResolveFilePath();
            return await UniTask.RunOnThreadPool(() => System.IO.File.ReadAllBytes(path));
        }

        public static async UniTask<UnityFile> ToBinaryFile(byte[] fileBytes, string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await File.WriteAllBytesAsync(filePath, fileBytes);

            PathType pathType = PathResolver.ResolvePathType(filePath);
            UnityFilePath path = new(pathType, filePath);
            return new UnityFile() { Path = path, Value = fileBytes };
        }
        
        public static bool IsImage(this byte[] fileBytes)
        {
            List<byte[]> headers = new()
            {
                Encoding.ASCII.GetBytes("BM"),      // BMP
                Encoding.ASCII.GetBytes("GIF"),     // GIF
                new byte[] { 137, 80, 78, 71 },     // PNG
                new byte[] { 73, 73, 42 },          // TIFF
                new byte[] { 77, 77, 42 },          // TIFF
                new byte[] { 255, 216, 255, 224 },  // JPEG
                new byte[] { 255, 216, 255, 225 }   // JPEG CANON
            };

            return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
        }
    }
}