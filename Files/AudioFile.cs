using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class AudioFile : BaseFile<AudioClip>
    {
        protected override async UniTask<AudioClip> LoadFileAsync() => await AudioConverter.LoadAudioClip(Path);
        protected override byte[] ToBytes() => Value.ToBytes();
    }
}