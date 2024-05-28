using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public static class UnityWebRequestUtils
    {
        public static UnityWebRequest CreateUnityWebRequest<TReq>(this TReq req, string method, bool logBody, bool logHeaders, JsonSerializerSettings jsonSettings = null)
            where TReq : RESTRequest
        {
            if (req == null) throw new IssueException(Issue.InvalidRequest, "Request is null.");
            if (req.WebRequest != null) return req.WebRequest;

            EncodeBody(req, method, logBody, jsonSettings);

            foreach (RESTHeader header in req.GetHeaders())
            {
                if (logHeaders) RESTLog.RequestHeader($"{header.Name} : {header.Value}");
                req.WebRequest.SetRequestHeader(header);
            }

            return req.WebRequest;
        }

        private static void EncodeBody<TReq>(TReq req, string method, bool logBody, JsonSerializerSettings jsonSettings = null)
            where TReq : RESTRequest
        {
            string url = req.Endpoint;
            ContentType contentType = req.ContentType;

            if (contentType == ContentType.Json)
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
                        byte[] body = req.ToJson(logBody, jsonSettings);
                        if (body == null) throw new IssueException(Issue.InvalidRequest, "Failed to encode body of this request.");
                        req.WebRequest.uploadHandler = new UploadHandlerRaw(body);
                    }
                }
            }
            else if (contentType == ContentType.WWWForm)
            {
                WWWForm form = req.Form;
                if (form == null) throw new IssueException(Issue.InvalidRequest, "Failed to encode form data of this request.");
                req.WebRequest = UnityWebRequest.Post(url, form);
            }
            else if (contentType == ContentType.MultipartForm)
            {
                List<IMultipartFormSection> formData = req.ToForm();
                if (formData == null) throw new IssueException(Issue.InvalidRequest, "Failed to encode form data of this request.");
                req.WebRequest = UnityWebRequest.Post(url, formData);
            }
            else
            {
                throw new IssueException(Issue.InvalidRequest, "Unsupported content type.");
            }
        }

        private static byte[] ToJson<TReq>(this TReq req, bool logBody, JsonSerializerSettings settings)
            where TReq : RESTRequest
        {
            if (req == null) return null;
            string bodyString = JsonConvert.SerializeObject(req, settings);
            if (string.IsNullOrEmpty(bodyString)) return null;
            if (logBody) RESTLog.RequestBody(bodyString);
            return Encoding.UTF8.GetBytes(bodyString);
        }
    }
}