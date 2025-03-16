using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class UnityAudioFile : UnityFileBase<AudioClip>
    {
        [SerializeField] private float length;

        public float Length
        {
            get => length;
            set => length = value;
        }

        protected override async UniTask<AudioClip> LoadFileAsync()
        {
            if (Path == null || !Path.IsValid) return null;
            AudioClip clip = await AudioConverter.LoadAudioClip(Path);
            if (length != 0f && clip != null) length = clip.length;
            return clip;
        }
        
        public override byte[] ToByteArray() => Value.ToBytes();

        public UnityAudioFile()
        {
        }
        public UnityAudioFile(string filePath) : base(filePath)
        {
        }
    }
}