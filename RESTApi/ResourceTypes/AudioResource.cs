using System;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class AudioResource : UnityAudioFile, IResource
    {
        public string Purpose { get; set; }
        public AudioResource()
        {
        }
        public AudioResource(string filePath) : base(filePath)
        {
        }
    }
}