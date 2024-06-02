using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class UnityImageFile : UnityFileBase<Texture2D>
    {
        protected override async UniTask<Texture2D> LoadFileAsync() => await ImageConverter.LoadTexture(Path);
        protected override byte[] ToBytes() => Value.ToBytes();
    }
}