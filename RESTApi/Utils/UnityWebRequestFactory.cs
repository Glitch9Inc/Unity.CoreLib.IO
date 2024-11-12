using Glitch9.IO.Files;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    internal class UnityWebRequestFactory
    {
        internal static UnityWebRequest Create<TReq>(TReq req, string method, RESTClient client)
            where TReq : RESTRequest
        {
            if (req == null) throw new IssueException(Issue.InvalidRequest, "Request is null.");
            //if (req.WebRequest != null) return req.WebRequest;

            CreateUnityWebRequest(req, method, client);
            bool includeContentTypeHeader = req.ContentType == ContentType.Json;

            if (client.LogRequestHeaders)
            {
                using (StringBuilderPool.Get(out StringBuilder sb))
                {
                    foreach (RESTHeader header in req.GetHeaders(includeContentTypeHeader))
                    {
                        if (header.Name.Contains("Auth"))
                        {
                            sb.AppendLine($"{header.Name}: [ApiKey]");
                        }
                        else
                        {
                            sb.AppendLine($"{header.Name}: {header.Value}");
                        }
                 
                        req.WebRequest.SetRequestHeader(header);
                    }

                    client.InternalLogger.RequestHeaders(sb.ToString());
                }
            }
            else
            {
                foreach (RESTHeader header in req.GetHeaders(includeContentTypeHeader))
                {
                    req.WebRequest.SetRequestHeader(header);
                }
            }

            return req.WebRequest;
        }

        private static void CreateUnityWebRequest<TReq>(TReq req, string method, RESTClient client)
            where TReq : RESTRequest
        {
            string url = req.Endpoint;
            ContentType contentType = req.ContentType;

            if (contentType == ContentType.Json)
            {
                DownloadHandler downloadHandler = req.StreamMode switch
                {
                    StreamMode.TextStream => new TextStreamHandlerBuffer(client, req.OnStreamEvent, req.OnProgressChanged),
                    StreamMode.BinaryStream => new BinaryStreamHandlerBuffer(client, req.OnBinaryStream, req.OnProgressChanged),
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
                        byte[] body = req.ToJson(client);
                        if (body == null) throw new IssueException(Issue.InvalidRequest, "Failed to encode body of this request.");
                        req.WebRequest.uploadHandler = new UploadHandlerRaw(body);
                    }
                }
            }
            else if (contentType == ContentType.WWWForm)
            {
                WWWForm form = req.Form;
                if (form == null) throw new IssueException(Issue.InvalidRequest, "Failed to encode form data of this request.");
                //else if (logBody) RESTLog.RequestBody($"WWWForm: \r\n {form}");
                req.WebRequest = UnityWebRequest.Post(url, form);
                req.TimeoutInSec = Convert.ToInt32(client.Timeout.TotalSeconds);
            }
            else if (contentType == ContentType.MultipartForm)
            {
                List<IMultipartFormSection> formData = req.ToMultipartFormSections(client);
                if (formData == null) throw new IssueException(Issue.InvalidRequest, "Failed to encode form data of this request.");
                req.WebRequest = UnityWebRequest.Post(url, formData);
                req.TimeoutInSec = Convert.ToInt32(client.Timeout.TotalSeconds);
            }
            else
            {
                throw new IssueException(Issue.InvalidRequest, $"Unsupported content type: {contentType}");
            }
        }
    }
}