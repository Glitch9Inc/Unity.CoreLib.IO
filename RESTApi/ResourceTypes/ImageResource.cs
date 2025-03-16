using System;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class ImageResource : UnityImageFile, IResource
    {
        public string Purpose { get; set; }
        public ImageResource()
        {
        }
        public ImageResource(string filePath) : base(filePath)
        {
        }
    }
}