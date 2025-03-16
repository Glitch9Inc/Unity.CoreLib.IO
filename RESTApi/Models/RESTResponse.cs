using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// The base class for all API objects returned by <see cref="RESTApiV3"/>.
    /// This class encapsulates common properties and methods for handling 
    /// various types of outputs from REST API responses.
    /// </summary>
    public class RESTResponse : Result
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// This ID is used to uniquely identify the API object.
        /// </summary>
        [JsonProperty("id")] public virtual string Id { get; set; }

        /// <summary>
        /// Text output from the API. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore]
        public string ResponseBody
        {
            get => TextOutput;
            set => TextOutput = value;
        }

        /// <summary>
        /// Whether the API response has an empty body.
        /// </summary>
        [JsonIgnore] public bool HasEmptyBody => string.IsNullOrEmpty(TextOutput);

        /// <summary>
        /// Binary data output from the API. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore] public virtual byte[] BinaryOutput { get; set; }

        /// <summary>
        /// Image data output from the API as a Texture2D object. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore] public virtual Texture2D ImageOutput { get; set; }

        /// <summary>
        /// Audio data output from the API as an AudioClip object. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore] public virtual AudioClip AudioOutput { get; set; }

        /// <summary>
        /// File output from the API as a UnityFile object. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore] public virtual UnityFile FileOutput { get; set; }

        /// <summary>
        /// The log object for the API response. This property is ignored during JSON serialization.
        /// </summary>
        [JsonIgnore] public virtual IRESTLog Log { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="RESTResponse"/> class with the success flag set to true.
        /// This method is used to quickly create a successful result.
        /// </summary>
        /// <returns>A new instance of <see cref="RESTResponse"/> with <see cref="Result.IsSuccess"/> set to true.</returns>
        public static RESTResponse Done() => new() { IsSuccess = true, IsDone = true };
    }
}
