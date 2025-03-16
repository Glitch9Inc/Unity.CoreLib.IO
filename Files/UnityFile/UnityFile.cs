using System;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class UnityFile : UnityFileBase<byte[]>
    {
        protected override async UniTask<byte[]> LoadFileAsync() => await BinaryUtils.LoadBytes(Path);
        public override byte[] ToByteArray() => Value;
        public UnityFile()
        {
        }
        public UnityFile(string filePath) : base(filePath)
        {
        }
    }
}