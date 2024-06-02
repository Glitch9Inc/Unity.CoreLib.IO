using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    public class RESTObject : Result
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }
        [JsonIgnore] public byte[] BinaryResult { get; set; }
        [JsonIgnore] public string TextResult { get; set; }
        [JsonIgnore] public Texture2D ImageResult { get; set; }
        [JsonIgnore] public AudioClip AudioResult { get; set; }
        [JsonIgnore] public UnityFile FileResult { get; set; }

        public static RESTObject Done() => new() { IsSuccess = true };
    }
}