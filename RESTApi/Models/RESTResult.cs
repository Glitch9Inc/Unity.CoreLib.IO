using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    public class RESTResult : Result
    {
        [JsonIgnore] public byte[] BinaryResult { get; set; }
        [JsonIgnore] public string TextResult { get; set; }
        [JsonIgnore] public Texture2D ImageResult { get; set; }
        [JsonIgnore] public AudioClip AudioResult { get; set; }
        public virtual void OnResponseConverted(string resultAsString) { }
        
        public static RESTResult Done() => new RESTResult { IsSuccess = true };
    }
}