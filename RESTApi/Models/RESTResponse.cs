using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    public class RESTResponse : IRESTResult
    {
        public static RESTResponse Empty => new();
        [JsonIgnore] public byte[] BinaryResult { get; set; }
        [JsonIgnore] public string TextResult { get; set; }
        [JsonIgnore] public Texture2D TextureResult { get; set; }
        [JsonIgnore] public AudioClip AudioResult { get; set; }
        public virtual void OnResponseConverted(string resultAsString) { }
    }
}