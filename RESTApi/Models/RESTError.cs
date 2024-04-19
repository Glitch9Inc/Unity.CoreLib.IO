using Newtonsoft.Json;
using System;

namespace Glitch9.IO.RESTApi
{
    public class RESTError : IRESTResult
    {
        [JsonIgnore] public Issue Issue { get; set; }
        [JsonIgnore] public string StackTrace { get; set; }
        [JsonProperty("message")] public string Message { get; set; }

        public RESTError() { }
        public RESTError(Exception e, string message = null)
        {
            Issue = e.Convert();
            Message = string.IsNullOrEmpty((message)) ? message : $"{message}\n{e.Message}";
            StackTrace = e.StackTrace;
        }

        public RESTError(Issue issue, string message = null)
        {
            Issue = issue;
            Message = message;
        }
    }
}