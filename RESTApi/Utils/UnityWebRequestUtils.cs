using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public static class UnityWebRequestUtils
    {
        public static UnityWebRequest CreateUnityWebRequest<TReq>(this TReq req, string method, JsonSerializerSettings jsonSettings = null)
            where TReq : RESTRequest
        {
            if (req == null) throw new GNException(Issue.InvalidRequest, "Request is null.");
            if (req.WebRequest != null) return req.WebRequest;

            EncodeBody(req, method, jsonSettings);

            foreach (RESTHeader header in req.GetHeaders())
            {
                if (req.IsLogEnabled) GNLog.Super("Header", $"{header.Name} : {header.Value}");
                req.WebRequest.SetRequestHeader(header);
            }

            return req.WebRequest;
        }

        private static void EncodeBody<TReq>(TReq req, string method, JsonSerializerSettings jsonSettings = null)
            where TReq : RESTRequest
        {
            string url = req.Endpoint;
            ContentType contentType = req.ContentType;

            if (contentType == ContentType.JSON)
            {
                DownloadHandler downloadHandler = req.DownloadMode switch
                {
                    DownloadMode.TextStream => new TextStreamHandlerBuffer(req.OnTextStreamReceived, req.OnProgressChanged),
                    DownloadMode.BinaryStream => new BinaryStreamHandlerBuffer(req.OnBinaryStreamReceived, req.OnProgressChanged),
                    _ => new DownloadHandlerBuffer()
                };

                req.WebRequest = new(url, method)
                {
                    timeout = req.TimeoutInSec,
                    downloadHandler = downloadHandler,
                };
                
                // Add body if method is not GET or DELETE
                if (method != UnityWebRequest.kHttpVerbGET && method != UnityWebRequest.kHttpVerbDELETE)
                {
                    if (req.HasBody)
                    {
                        byte[] body = req.ToJson(jsonSettings);
                        if (body == null) throw new GNException(Issue.InvalidRequest, "Failed to encode body of this request.");
                        req.WebRequest.uploadHandler = new UploadHandlerRaw(body);
                    }
                }
            }
            else if (contentType == ContentType.Form)
            {
                List<IMultipartFormSection> formData = req.ToForm();
                if (formData == null) throw new GNException(Issue.InvalidRequest, "Failed to encode form data of this request.");
                req.WebRequest = UnityWebRequest.Post(url, formData);
            }
            else
            {
                throw new GNException(Issue.InvalidRequest, "Unsupported content type.");
            }
        }
    }
}