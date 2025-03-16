using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class UnityImageFile : UnityFileBase<Texture2D>
    {
        protected override async UniTask<Texture2D> LoadFileAsync()
        {
            if (Path == null || !Path.IsValid) return null;
            return await ImageConverter.LoadTexture(Path);
        }
        public override byte[] ToByteArray() => Value.ToByteArray();

        public UnityImageFile()
        {
        }
        public UnityImageFile(string filePath) : base(filePath)
        {
        }

        public async UniTask<Sprite> GetSpriteAsync()
        {
            Texture2D tex = await GetValue();
            if (tex == null)
            {
                Debug.LogError("Failed to load image file: " + Path);
                return null;
            }
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}