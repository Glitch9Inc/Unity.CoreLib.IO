using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    public static class RESTApiExtensions
    {
        public static byte[] ToJson<TReq>(this TReq req, JsonSerializerSettings settings)
            where TReq : RESTRequest
        {
            if (req == null) return null;
            string bodyString = JsonConvert.SerializeObject(req, settings);
            if (string.IsNullOrEmpty(bodyString)) return null;
            //if (req.IsLogEnabled) 
            GNLog.Log($"<color=blue>[ApiRequest Body]</color> {bodyString}");
            return Encoding.UTF8.GetBytes(bodyString);
        }
    }
}