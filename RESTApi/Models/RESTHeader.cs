
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public struct RESTHeader
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public readonly bool IsValid => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Value);
        public RESTHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public static class RESTHeaderExtensions
    {
        public static void SetRequestHeader(this UnityWebRequest request, RESTHeader header)
        {
            request.SetRequestHeader(header.Name, header.Value);
        }
    }
}