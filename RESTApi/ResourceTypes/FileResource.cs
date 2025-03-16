using System;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class FileResource : UnityFile, IResource
    {
        public string Purpose { get; set; }
        public FileResource()
        {
        }
        public FileResource(string filePath) : base(filePath)
        {
        }
    }
}