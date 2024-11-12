using System;

namespace Glitch9.IO.Files
{
    [Serializable]
    public class TextResource : IResource
    {
        public static implicit operator TextResource(string value) => new(value);
        public static implicit operator string(TextResource value) => value.value;

        public string Purpose { get; set; }
        public string value;
        public TextResource() { }
        public TextResource(string value) => this.value = value;
        public override string ToString() => value;
    }
}